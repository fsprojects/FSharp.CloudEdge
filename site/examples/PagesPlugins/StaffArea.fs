module StaffArea

module Access = FSharp.CloudEdge.Runtime.PagesPluginCloudflareAccess

let onRequest =
    Access.Exports.pagesPluginCloudflareAccess(
        Access.PluginArgs.Create(
            aud = "your-application-aud-tag",
            domain = "https://your-team.cloudflareaccess.com"
        )
    )
