module SignOut

module Workers = FSharp.CloudEdge.Runtime.Workers
module Access = FSharp.CloudEdge.Runtime.PagesPluginCloudflareAccess

let onRequestGet (_: Workers.EventContext<obj, string, obj>) =
    let logout =
        Access.Api.Exports.generateLogoutURL(
            Access.Api.GenerateLogoutURL0.Create(domain = "https://your-team.cloudflareaccess.com"))
    Workers.Exports.Response.redirect(logout, 302.)
