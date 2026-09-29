module CloudEdgeAIComposition

open Fable.Core

module V4 = FSharp.CloudEdge.Support.AI.V4.Provider
module V3 = FSharp.CloudEdge.Support.AI.V3.Provider
module Compatible = FSharp.CloudEdge.Support.AI.V4.OpenAICompatible
module Gateway = FSharp.CloudEdge.Runtime.AIGatewayProvider
module Workers = FSharp.CloudEdge.Runtime.WorkersAIProvider
module Search = FSharp.CloudEdge.Runtime.AISearchProvider

let unifiedModel (modelId: string) : V4.LanguageModelV4 =
    let provider: Compatible.OpenAICompatibleProvider<string, string, string, string> =
        Gateway.Providers.Unified.Exports.createUnified ()
    provider.chatModel modelId

let throughGateway (settings: Gateway.AiGatewaySettings) (model: V4.LanguageModelV4) : V4.LanguageModelV4 =
    let gateway = Gateway.Exports.createAiGateway settings
    gateway.chat (U2.Case2 model)

let workersModel (settings: Workers.WorkersAISettings) (modelId: string) : V4.LanguageModelV4 =
    let provider = Workers.Exports.createWorkersAI settings
    provider.chat<string> (U2.Case1 modelId) :> V4.LanguageModelV4

let workersThroughGateway gatewaySettings workersSettings modelId =
    workersModel workersSettings modelId |> throughGateway gatewaySettings

let searchModel (settings: Search.AISearchNamespaceSettings) (instanceName: string) : V3.LanguageModelV3 =
    let provider = Search.Exports.createAISearchNamespace settings
    provider.get(instanceName).chat() :> V3.LanguageModelV3

let generate (model: V4.LanguageModelV4) (options: V4.LanguageModelV4CallOptions) : JS.Promise<V4.LanguageModelV4GenerateResult> =
    model.doGenerate options
