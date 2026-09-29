module FSharp.CloudEdge.ManagementIntegration

open System
open System.IO
open System.Net.Http
open System.Net.Http.Headers
open System.Text.Json
open System.Threading
open FSharp.CloudEdge.Core.Api.Types
open FSharp.CloudEdge.Core.Api.Http
open FSharp.CloudEdge.Management.Compute
open FSharp.CloudEdge.Management.Networking
open FSharp.CloudEdge.Management.Security

type Observation = { id: string; status: string; detail: string }

type ResponseMetadata(inner: HttpMessageHandler) =
    inherit DelegatingHandler(inner)
    member val Status = 0 with get, private set
    member val RetryAfter: string option = None with get, private set
    override this.SendAsync(request, cancellationToken) =
        let sending = base.SendAsync(request, cancellationToken)
        task {
            let! response = sending
            this.Status <- int response.StatusCode
            this.RetryAfter <-
                match response.Headers.TryGetValues "Retry-After" with
                | true, values -> values |> Seq.exactlyOne |> Some
                | _ -> None
            return response
        }

let complete (work: System.Threading.Tasks.Task<'a>) = work.GetAwaiter().GetResult()
let check condition message = if not condition then failwith message
let json (value: string) = use d = JsonDocument.Parse value in d.RootElement.Clone()
let field name (value: JsonElement) = value.GetProperty(name: string)
let str name value = (field name value).GetString()
let expectJsonException message action =
    let rejected =
        try
            action ()
            false
        with :? JsonException -> true
    check rejected message

[<EntryPoint>]
let main args =
    check (args.Length = 3) "Expected loopback URL, fixtures path and observations path"
    let endpoint = Uri args[0]
    check (endpoint.Scheme = "http" && endpoint.Host = "127.0.0.1") "Management fixtures require IPv4 loopback"
    use transport = new HttpClientHandler(AllowAutoRedirect = false, UseProxy = false)
    use handler = new ResponseMetadata(transport)
    use http = new HttpClient(handler, BaseAddress = endpoint, Timeout = TimeSpan.FromSeconds 10.)
    http.DefaultRequestHeaders.Authorization <- AuthenticationHeaderValue("Bearer", "fixture-only-token")
    let compute, networking, security = ComputeClient http, NetworkingClient http, SecurityClient http
    check (typeof<CloudflareTunnelCreateACloudflareTunnel>.Assembly = typeof<CreateApplication>.Assembly) "Compute and Networking lost their shared model owner"
    check (typeof<AccessPoliciesCreateAnAccessPolicy>.Assembly = typeof<CreateApplication>.Assembly) "Security lost its shared model owner"
    let fixtures = (json (File.ReadAllText args[1])).GetProperty("cases").EnumerateArray() |> Seq.map (fun c -> str "id" c, c.Clone()) |> Map.ofSeq
    let body id = (field "requestBody" fixtures[id]).GetRawText()
    let observations = ResizeArray<Observation>()
    let run (id: string) action =
        http.DefaultRequestHeaders.Remove "X-Integration-Case" |> ignore
        http.DefaultRequestHeaders.Add("X-Integration-Case", id)
        try
            action ()
            observations.Add { id = id; status = "passed"; detail = "Generated client contract passed" }
        with e ->
            // Authentic object responses must remain failures until the generated client can consume them.
            // In particular, a JsonException is evidence of a gap, never an accepted response substitute.
            observations.Add { id = id; status = "failed"; detail = e.GetType().FullName + ": " + e.Message }
    let account = "account-fixture"
    let tunnelId = Guid.Parse "11111111-1111-4111-8111-111111111111"
    let warpId = Guid.Parse "22222222-2222-4222-8222-222222222222"
    let policyId = "33333333-3333-4333-8333-333333333333"
    let appId = "44444444-4444-4444-8444-444444444444"
    let checkTunnelFailure expectedStatus expectedCount status (content: tunnel_api_u002D_response_u002D_common_u002D_failure) =
        check (status = expectedStatus && handler.Status = expectedStatus) "Actual HTTP failure status lost"
        let hasExplicitNullResult = match content.result with Some result -> isNull result | None -> false
        check (not content.success && hasExplicitNullResult) "Failure envelope changed success or explicit null result"
        let errors: tunnel_messages = content.errors
        check (errors.Length = expectedCount) "Error details lost"
        let error = errors[0]
        check (error.code = 19999) "Synthetic error code lost"
        let expectedMessage = if expectedStatus = 429 then "Rate limit exceeded (synthetic fixture)" else "Access denied (synthetic fixture)"
        check (error.message = expectedMessage) "Error message lost"
        check (content.messages.IsEmpty) "Informational messages changed"
        check (handler.RetryAfter = (if expectedStatus = 429 then Some "3" else None)) "Retry-After header lost or leaked from another response"
    let containerRequest =
        cc_ContainersCreateDurableObjectApplicationRequest.Create(
            cc_DurableObjectsConfiguration.FromJson(json "{\"namespace_id\":\"scope-native-services\"}"),
            "conclave-native", cc_DurableObjectApplicationSchedulingPolicy.DurableObject)
        |> cc_ContainersCreateApplicationRequest.Variant2
    run "compute.create-native" (fun () ->
        match complete (compute.CreateApplication(account, containerRequest)) with
        | CreateApplication.Created content ->
            check content.success "Container creation failed"
            match content.result with
            | cc_ContainersApplicationResponse.Variant2 application -> check (application.id = "native-application") "Container identity lost"
            | _ -> failwith "DO application decoded as a scheduled application"
        | response -> failwithf "Expected Created, got %A" response)
    run "compute.get-native" (fun () ->
        match complete (compute.GetApplication(account, "native-application")) with
        | GetApplication.OK content ->
            check content.success "Container read failed"
            match content.result with
            | cc_ContainersApplicationResponse.Variant2 application ->
                check (application.id = "native-application" && application.account_id = account) "DO application identity lost"
                check (application.scheduling_policy = cc_DurableObjectApplicationSchedulingPolicy.DurableObject) "Container ownership lost"
                check (application.durable_objects.namespace_id = "scope-native-services") "DO namespace identity lost"
            | _ -> failwith "DO application decoded as a scheduled application"
        | response -> failwithf "Expected OK, got %A" response)
    run "compute.get-scheduled" (fun () ->
        match complete (compute.GetApplication(account, "scheduled-application")) with
        | GetApplication.OK content ->
            check content.success "Scheduled container read failed"
            match content.result with
            | cc_ContainersApplicationResponse.Variant1 application ->
                check (application.id = "scheduled-application" && application.account_id = account) "Scheduled application identity lost"
                check (application.scheduling_policy = cc_SchedulingPolicy.Default) "Scheduled application policy lost"
                check (application.version = 2 && application.instances = 3) "Scheduled application version or instance count lost"
                check (application.configuration.image = "registry.example/conclave@sha256:" + String.replicate 64 "a") "Scheduled application image lost"
            | _ -> failwith "Scheduled application decoded as a DO application"
        | response -> failwithf "Expected OK, got %A" response)
    // These deliberately malformed bodies are negative controls, independently checked against the pinned schema.
    for id, reason in [
        "compute.application-missing-discriminator", "Missing application discriminator was accepted"
        "compute.application-unknown-discriminator", "Unknown application discriminator was accepted"
        "compute.application-null-discriminator", "Null application discriminator was accepted"
        "compute.application-missing-id", "Application without required identity was accepted"
        "compute.application-null-result", "Null application result was accepted" ] do
        run id (fun () ->
            expectJsonException reason (fun () -> complete (compute.GetApplication(account, "native-application")) |> ignore))
    run "compute.rollout-scheduled" (fun () ->
        // Rollouts belong to scheduler-backed applications, never to the DO-managed application above.
        let configuration = { cc_ModifyUserDeploymentConfiguration.Create() with image = Some ("registry.example/conclave@sha256:" + String.replicate 64 "a") }
        let rollout = { cc_ContainersCreateApplicationRolloutRequest.Create("Clef service image", cc_ContainersCreateApplicationRolloutRequestStrategy.Rolling, configuration) with step_percentage = Some Step_percentage.Step_percentage25 }
        match complete (compute.CreateApplicationRollout(account, "scheduled-application", rollout)) with
        | CreateApplicationRollout.Created content -> check (content.success && content.result.id = "rollout-fixture") "Rollout identity lost"
        | response -> failwithf "Expected Created, got %A" response)
    run "compute.forbidden" (fun () ->
        match complete (compute.CreateApplication(account, containerRequest)) with
        | CreateApplication.Forbidden payload -> check (not payload.success) "Forbidden response reported success"
        | response -> failwithf "Expected Forbidden, got %A" response)
    run "compute.delete-native" (fun () ->
        match complete (compute.DeleteApplication(account, "native-application")) with
        | DeleteApplication.OK content ->
            check content.success "Container deletion failed"
            check (content.result.message = "Application deletion started") "Asynchronous deletion acknowledgement lost"
        | response -> failwithf "Expected OK, got %A" response)
    run "compute.deleted-native" (fun () ->
        match complete (compute.GetApplication(account, "native-application")) with
        | GetApplication.NotFound payload -> check (not payload.success) "Deleted application still present"
        | response -> failwithf "Expected NotFound, got %A" response)
    run "compute.delete-worker" (fun () ->
        match complete (compute.WorkerScriptDeleteWorker(account, "ce-it-owned-fixture", force = false)) with
        | WorkerScriptDeleteWorker.OK payload -> check payload.success "Worker deletion failed"
        | response -> failwithf "Expected OK, got %A" response)
    run "tunnel.create" (fun () ->
        let request = { CloudflareTunnelCreateACloudflareTunnelPayload.Create("Conclave IoT & WREN") with config_src = Some tunnel_config_src.Cloudflare }
        match complete (networking.CloudflareTunnelCreateACloudflareTunnel(account, request)) with
        | CloudflareTunnelCreateACloudflareTunnel.OK payload -> check (payload.success && payload.result["id"].GetValue<string>() = string tunnelId) "Tunnel identity lost"
        | response -> failwithf "Expected OK, got %A" response)
    run "tunnel.list-filter" (fun () ->
        match complete (networking.CloudflareTunnelListCloudflareTunnels(account, name = "Conclave IoT & WREN", isDeleted = false, perPage = 10., page = 2.)) with
        | CloudflareTunnelListCloudflareTunnels.OK payload -> check (payload.result.AsArray().Count = 1) "Filtered tunnel collection lost"
        | response -> failwithf "Expected OK, got %A" response)
    let tunnelConfig = Serializer.deserialize<CloudflareTunnelConfigurationPutConfigurationPayload> (body "tunnel.configure")
    run "tunnel.configure" (fun () ->
        match complete (networking.CloudflareTunnelConfigurationPutConfiguration(account, tunnelId, tunnelConfig)) with
        | CloudflareTunnelConfigurationPutConfiguration.OK payload ->
            let config = payload.result.Value.config.Value
            check (config.``warp-routing``.Value.enabled = Some true) "WARP route enablement lost"
            check (config.ingress.Value.Head.service = "http://127.0.0.1:8787") "Ingress target lost"
        | response -> failwithf "Expected OK, got %A" response)
    run "tunnel.read-configuration" (fun () ->
        match complete (networking.CloudflareTunnelConfigurationGetConfiguration(account, tunnelId)) with
        | CloudflareTunnelConfigurationGetConfiguration.OK payload -> check (payload.result.Value.version = Some 7) "Configuration version lost"
        | response -> failwithf "Expected OK, got %A" response)
    run "tunnel.route-create" (fun () ->
        let request = { TunnelRouteCreateATunnelRoutePayload.Create("fd00:1234::/48", tunnelId) with comment = Some "Clef native IoT subnet" }
        match complete (networking.TunnelRouteCreateATunnelRoute(account, request)) with
        | TunnelRouteCreateATunnelRoute.OK payload -> check (payload.result["network"].GetValue<string>() = "fd00:1234::/48") "IPv6 CIDR lost"
        | response -> failwithf "Expected OK, got %A" response)
    run "tunnel.route-delete" (fun () ->
        match complete (networking.TunnelRouteDeleteATunnelRoute("route-fixture", account)) with
        | TunnelRouteDeleteATunnelRoute.OK payload -> check payload.success "Route delete failed"
        | response -> failwithf "Expected OK, got %A" response)
    run "tunnel.delete" (fun () ->
        match complete (networking.CloudflareTunnelDeleteACloudflareTunnel(account, tunnelId)) with
        | CloudflareTunnelDeleteACloudflareTunnel.OK payload -> check payload.success "Tunnel delete failed"
        | response -> failwithf "Expected OK, got %A" response)
    run "tunnel.rate-limit" (fun () ->
        match complete (networking.CloudflareTunnelGetACloudflareTunnel(account, tunnelId)) with
        | CloudflareTunnelGetACloudflareTunnel.Status4XX(status, content) -> checkTunnelFailure 429 1 status content
        | response -> failwithf "Expected Status4XX, got %A" response)
    run "tunnel.forbidden" (fun () ->
        match complete (networking.CloudflareTunnelGetACloudflareTunnel(account, tunnelId)) with
        | CloudflareTunnelGetACloudflareTunnel.Status4XX(status, content) -> checkTunnelFailure 403 1 status content
        | response -> failwithf "Expected Status4XX, got %A" response)
    run "tunnel.multiple-errors" (fun () ->
        match complete (networking.CloudflareTunnelGetACloudflareTunnel(account, tunnelId)) with
        | CloudflareTunnelGetACloudflareTunnel.Status4XX(status, content) ->
            checkTunnelFailure 403 2 status content
            check (content.errors[1].code = 19998) "Secondary error identity or ordering lost"
            check (content.errors[1].message = "Secondary route constraint (synthetic fixture): Δ") "Secondary error text lost"
        | response -> failwithf "Expected Status4XX, got %A" response)
    run "warp.rate-limit" (fun () ->
        match complete (networking.CloudflareTunnelGetAWarpConnectorTunnel(account, warpId)) with
        | CloudflareTunnelGetAWarpConnectorTunnel.Status4XX(status, content) -> checkTunnelFailure 429 1 status content
        | response -> failwithf "Expected Status4XX, got %A" response)
    run "tunnel.route-forbidden" (fun () ->
        match complete (networking.TunnelRouteGetTunnelRoute(account, "route-fixture")) with
        | TunnelRouteGetTunnelRoute.Status4XX(status, content) -> checkTunnelFailure 403 1 status content
        | response -> failwithf "Expected Status4XX, got %A" response)
    run "tunnel.virtual-network-rate-limit" (fun () ->
        match complete (networking.TunnelVirtualNetworkListVirtualNetworks(account)) with
        | TunnelVirtualNetworkListVirtualNetworks.Status4XX(status, content) -> checkTunnelFailure 429 1 status content
        | response -> failwithf "Expected Status4XX, got %A" response)
    run "tunnel.all-forbidden" (fun () ->
        match complete (networking.CloudflareTunnelListAllTunnels(account)) with
        | CloudflareTunnelListAllTunnels.Status4XX(status, content) -> checkTunnelFailure 403 1 status content
        | response -> failwithf "Expected Status4XX, got %A" response)
    run "tunnel.hostname-route-rate-limit" (fun () ->
        match complete (security.ZeroTrustNetworksRouteHostnameGet(account, Guid.Parse policyId)) with
        | ZeroTrustNetworksRouteHostnameGet.Status4XX(status, content) -> checkTunnelFailure 429 1 status content
        | response -> failwithf "Expected Status4XX, got %A" response)
    run "tunnel.warp-subnet-forbidden" (fun () ->
        match complete (security.ZeroTrustNetworksSubnetGetWarp(account, Guid.Parse appId)) with
        | ZeroTrustNetworksSubnetGetWarp.Status4XX(status, content) -> checkTunnelFailure 403 1 status content
        | response -> failwithf "Expected Status4XX, got %A" response)
    run "warp.create" (fun () ->
        let request = { CloudflareTunnelCreateAWarpConnectorTunnelPayload.Create("conclave-branch") with ha = Some true }
        match complete (networking.CloudflareTunnelCreateAWarpConnectorTunnel(account, request)) with
        | CloudflareTunnelCreateAWarpConnectorTunnel.OK payload -> check (payload.result["id"].GetValue<string>() = string warpId) "WARP identity lost"
        | response -> failwithf "Expected OK, got %A" response)
    run "warp.configure" (fun () ->
        let request = Serializer.deserialize<tunnel_mesh_configuration_request_body> (body "warp.configure")
        match complete (networking.CloudflareTunnelConfigurationUpdateWarpConnectorConfiguration(account, warpId, request)) with
        | CloudflareTunnelConfigurationUpdateWarpConnectorConfiguration.OK payload ->
            check (payload.result.Value.ha_mode = tunnel_mesh_ha_mode.Local) "WARP HA mode lost"
            check (payload.result.Value.configuration_version = 3) "WARP configuration version lost"
        | response -> failwithf "Expected OK, got %A" response)
    run "warp.read-configuration" (fun () ->
        match complete (networking.CloudflareTunnelConfigurationGetWarpConnectorConfiguration(account, warpId)) with
        | CloudflareTunnelConfigurationGetWarpConnectorConfiguration.OK payload -> check (payload.result.Value.tunnel_id = warpId) "WARP configuration belongs to another tunnel"
        | response -> failwithf "Expected OK, got %A" response)
    run "warp.delete" (fun () ->
        match complete (networking.CloudflareTunnelDeleteAWarpConnectorTunnel(account, warpId)) with
        | CloudflareTunnelDeleteAWarpConnectorTunnel.OK payload -> check payload.success "WARP delete failed"
        | response -> failwithf "Expected OK, got %A" response)
    run "access.create-policy" (fun () ->
        let request = access_app_policy_write_request.FromJson(json (body "access.create-policy"))
        match complete (security.AccessPoliciesCreateAnAccessPolicy(appId, account, request)) with
        | AccessPoliciesCreateAnAccessPolicy.Created payload ->
            let result = payload.result.Value.JsonValue
            check (str "id" result = policyId && str "decision" result = "allow") "Access admission policy lost"
            check ((field "require" result).GetArrayLength() = 1) "Access device requirement lost"
        | response -> failwithf "Expected Created, got %A" response)
    run "access.read-policy" (fun () ->
        match complete (security.AccessPoliciesGetAnAccessPolicy(appId, policyId, account)) with
        | AccessPoliciesGetAnAccessPolicy.OK payload -> check (str "session_duration" payload.result.Value.JsonValue = "1h") "Policy duration lost"
        | response -> failwithf "Expected OK, got %A" response)
    run "access.delete-policy" (fun () ->
        match complete (security.AccessPoliciesDeleteAnAccessPolicy(appId, policyId, account)) with
        | AccessPoliciesDeleteAnAccessPolicy.Accepted payload -> check (payload.success && payload.result.Value.id = Some policyId) "Accepted revocation identity lost"
        | response -> failwithf "Expected Accepted, got %A" response)
    for id, status in ["access.forbidden", 403; "access.rate-limit", 429] do
        run id (fun () ->
            match complete (security.AccessPoliciesGetAnAccessPolicy(appId, policyId, account)) with
            | AccessPoliciesGetAnAccessPolicy.Status4XX(actual, payload) -> check (actual = status && not payload.success) "Actual HTTP status lost"
            | response -> failwithf "Expected Status4XX, got %A" response)
    run "access.undeclared-status" (fun () ->
        let rejected =
            try complete (security.AccessPoliciesGetAnAccessPolicy(appId, policyId, account)) |> ignore; false
            with e -> e.Message.Contains "Unexpected HTTP status 599"
        check rejected "Undeclared HTTP status was accepted")
    run "access.malformed-response" (fun () ->
        let rejected =
            try complete (security.AccessPoliciesGetAnAccessPolicy(appId, policyId, account)) |> ignore; false
            with :? JsonException -> true
        check rejected "Malformed JSON response was accepted")
    run "access.cancel-inflight" (fun () ->
        use cancelled = new CancellationTokenSource(TimeSpan.FromMilliseconds 300.)
        let rejected =
            try complete (security.AccessPoliciesGetAnAccessPolicy(appId, policyId, account, cancellationToken = cancelled.Token)) |> ignore; false
            with :? OperationCanceledException -> true
        check rejected "Generated client ignored in-flight cancellation")
    File.WriteAllText(args[2], JsonSerializer.Serialize(observations, JsonSerializerOptions(WriteIndented = true)))
    printfn "%d management cases exercised (%d passing, %d failing)." observations.Count (observations |> Seq.filter(fun o -> o.status = "passed") |> Seq.length) (observations |> Seq.filter(fun o -> o.status = "failed") |> Seq.length)
    // The orchestrator combines transport assertions with these observations and decides exit status.
    0
