module Docs.Site

open System.IO
open Feliz.ViewEngine
open Nacara.Core
open Nacara.Plugins
open Partas.Nacara.Theme

let baseUrl = "/FSharp.CloudEdge/"

let private nugetUrl = "https://www.nuget.org/packages?q=FSharp.CloudEdge"

let private themeCss name =
    File.ReadAllText (Path.Combine (__SOURCE_DIRECTORY__, "theme", name))

let private fonts (t: Tokens) =
    { t with
        FontMono = "\"Cascadia Code\", \"JetBrains Mono\", ui-monospace, SFMono-Regular, Menlo, Consolas, monospace"
        ContentWidth = "48rem"
        SidebarWidth = "17rem" }

/// Cloudflare orange is the accent. F# blue marks notes; green, amber and red mark tips, warnings and dangers.
let private lightColours (t: Tokens) =
    { fonts t with
        Bg = "#FFFFFF"
        BgSubtle = "#F5F7FA"
        BgRaised = "#FFFFFF"
        Border = "#DCE2EB"
        Text = "#1B2230"
        TextMuted = "#566175"
        Heading = "#0E1420"
        Primary = "#B4570B"
        PrimaryContrast = "#FFFFFF"
        PrimarySubtle = "color-mix(in oklab, #F38020 12%, white)"
        Note = "#2A6F97"
        Tip = "#1F7A4D"
        Warning = "#9A6700"
        Danger = "#C62828"
        CodeInlineBg = "#F0F3F8"
        CodeInlineBorder = "#DCE2EB"
        CodeInlineText = "#1B2230" }

let private darkColours (t: Tokens) =
    { fonts t with
        Bg = "#0B0F16"
        BgSubtle = "#111724"
        BgRaised = "#172032"
        Border = "#26324A"
        Text = "#E4E9F2"
        TextMuted = "#93A0B8"
        Heading = "#F4F7FB"
        Primary = "#F38020"
        PrimaryContrast = "#160C02"
        PrimarySubtle = "color-mix(in oklab, #F38020 14%, #0B0F16)"
        Note = "#4FA3D9"
        Tip = "#4CC38A"
        Warning = "#FAAE40"
        Danger = "#F2726F"
        CodeInlineBg = "#172032"
        CodeInlineBorder = "#26324A"
        CodeInlineText = "#E4E9F2" }

/// Keywords take F# cyan and types F# blue. Functions take amber, numbers orange and strings green.
let private lightSyntax (s: SyntaxTokens) =
    { s with
        Comment = "#6B7686"
        String = "#1F7A4D"
        Number = "#B4570B"
        Constant = "#B4570B"
        Constructor = "#8A5300"
        Property = "#1B2230"
        Escape = "#B4570B"
        Keyword = "#1B7FA6"
        Operator = "#566175"
        Function = "#8A5300"
        Type = "#2A6F97"
        Namespace = "#2A6F97"
        Variable = "#1B2230"
        Parameter = "#1B2230"
        Punctuation = "#566175"
        Tag = "#1B7FA6"
        Attribute = "#6F42C1"
        Preprocessor = "#6F42C1"
        Invalid = "#C62828"
        Inserted = "#1F7A4D"
        Deleted = "#C62828" }

let private darkSyntax (s: SyntaxTokens) =
    { s with
        Comment = "#6E7C93"
        String = "#8BD5A0"
        Number = "#F79A52"
        Constant = "#F79A52"
        Constructor = "#F5B968"
        Property = "#E4E9F2"
        Escape = "#F79A52"
        Keyword = "#5EC4E6"
        Operator = "#9AA7BD"
        Function = "#F5B968"
        Type = "#7DB7E8"
        Namespace = "#7DB7E8"
        Variable = "#E4E9F2"
        Parameter = "#E4E9F2"
        Punctuation = "#9AA7BD"
        Tag = "#5EC4E6"
        Attribute = "#B9A2FA"
        Preprocessor = "#B9A2FA"
        Invalid = "#F2726F"
        Inserted = "#8BD5A0"
        Deleted = "#F2726F" }

/// The site opens in the dark scheme until the reader picks one.
let private darkByDefault =
    Html.script
        [
            prop.dangerouslySetInnerHTML
                "(()=>{try{if(localStorage.getItem('nacara-theme'))return}catch{}const r=document.documentElement;r.dataset.theme='dark';r.dataset.themeSetting='dark';addEventListener('DOMContentLoaded',()=>{document.querySelectorAll('[data-nacara-theme]').forEach(s=>{s.value='dark'})})})();"
        ]

let private nativeThemes =
    let metadata =
        Render.htmlView
            [
                Html.meta [ prop.name "darkreader-lock" ]
                Html.meta [ prop.name "color-scheme"; prop.content "light dark" ]
            ]

    let addMetadata (html: string) =
        if html.Contains("<head>" + metadata) then html
        else html.Replace("<head>", "<head>" + metadata)

    { new IPlugin with
        member _.Name = "cloudedge.native-themes"

        member _.Configure registry =
            // Build hooks write the default 404 directly, bypassing asset transforms.
            { registry with
                BuildCompleteHooks =
                    registry.BuildCompleteHooks
                    |> List.map (fun hook context ->
                        hook
                            { context with
                                Write = fun path content ->
                                    let html = if Path.GetExtension path = ".html" then addMetadata content else content
                                    context.Write path html
                            })
            }
            |> Registry.assetTransform
                {
                    Name = "native-theme-metadata"
                    Extensions = [ ".html" ]
                    // Theme.headExtra comes after CSS; Dark Reader must see the lock before loading styles.
                    Transform = fun context -> addMetadata context.Content
                }
    }

let theme =
    Theme.defaults
    |> Theme.navbar
        [
            NavbarSection ("Start here", "guide", "/guide/")
            NavbarDropdown (
                "Libraries",
                [
                    NavbarDescribed ("Workers", "Requests, responses and bindings", "/libraries/platform/")
                    NavbarDescribed ("Data & Analytics", "D1, R2, KV, Vectorize and Analytics Engine", "/libraries/data/")
                    NavbarDescribed ("Durable Objects", "Stateful rooms, WebSockets and alarms", "/libraries/platform/durable-objects/")
                    NavbarDescribed ("Background Work", "Queues and Workflows", "/libraries/platform/background-work/")
                    NavbarDescribed ("Agents & Tools", "SDK, MCP, chat and workspace tools", "/libraries/agents/")
                    NavbarDescribed ("AI", "Workers AI, AI Gateway and AI Search", "/libraries/ai/")
                    NavbarDescribed ("Hybrid Search", "Keyword and vector search, indexed incrementally", "/libraries/hybrid-search/")
                    NavbarDescribed ("Compute", "Compare execution environments", "/libraries/compute/")
                    NavbarDescribed ("Containers", "Your own containerized services", "/libraries/containers/")
                    NavbarDescribed ("Sandbox", "Linux commands, files and processes", "/libraries/agents/sandbox/")
                    NavbarDescribed ("Artifacts", "Git-compatible versioned repositories", "/libraries/agents/artifacts/")
                    NavbarDescribed ("Services", "Actors, OAuth and more", "/libraries/services/")
                    NavbarDescribed ("RPC", "Cap'n Web", "/libraries/rpc/")
                    NavbarDescribed ("Feature Flags", "Flagship", "/libraries/feature-flags/")
                    NavbarDescribed ("Pages Plugins", "Access, Turnstile and Static Forms", "/libraries/pages/")
                    NavbarDescribed ("Support", "Shared AI contracts and Workers helpers", "/libraries/support/")
                ]
            )
            NavbarSection ("Line-up", "libraries", "/libraries/")
            NavbarLink ("Control plane", "/libraries/control-plane/")
        ]
    |> Theme.menu
        "guide"
        [
            Menu.section
                "Start Here"
                [
                    Menu.page "guide/index.md"
                    Menu.page "guide/credentials.md"
                    Menu.page "guide/packages.md"
                    Menu.page "guide/first-worker.md"
                    Menu.page "guide/first-deploy.md"
                    Menu.page "guide/local-build.md"
                    Menu.page "guide/verify-bindings.md"
                ]
        ]
    |> Theme.menu
        "libraries"
        [
            Menu.page "libraries/index.md"
            Menu.page "libraries/packages.md"
            Menu.section
                "Runtime"
                [
                    Menu.group
                        "libraries/platform/index.md"
                        [
                            Menu.page "libraries/platform/durable-objects.md"
                            Menu.page "libraries/platform/background-work.md"
                        ]
                    Menu.page "libraries/ai.md"
                    Menu.page "libraries/services.md"
                    Menu.page "libraries/rpc.md"
                    Menu.page "libraries/feature-flags.md"
                    Menu.page "libraries/pages.md"
                    Menu.page "libraries/support.md"
                ]
            Menu.section
                "Data & Analytics"
                [
                    Menu.page "libraries/data/index.md"
                    Menu.page "libraries/data/d1.md"
                    Menu.page "libraries/data/r2.md"
                    Menu.page "libraries/data/kv.md"
                    Menu.page "libraries/data/vectorize.md"
                    Menu.page "libraries/data/analytics-engine.md"
                    Menu.page "libraries/hybrid-search.md"
                ]
            Menu.section
                "Compute"
                [
                    Menu.page "libraries/compute.md"
                    Menu.page "libraries/containers.md"
                    Menu.page "libraries/agents/sandbox.md"
                    Menu.page "libraries/agents/computer.md"
                ]
            Menu.section
                "Agents & Tools"
                [
                    Menu.page "libraries/agents.md"
                    Menu.page "libraries/agents/sdk.md"
                    Menu.page "libraries/agents/chat.md"
                    Menu.page "libraries/agents/mcp.md"
                    Menu.page "libraries/agents/code-mode.md"
                    Menu.page "libraries/agents/shell.md"
                    Menu.page "libraries/agents/git.md"
                    Menu.page "libraries/agents/artifacts.md"
                    Menu.page "libraries/agents/voice.md"
                    Menu.page "libraries/agents/email.md"
                    Menu.link "Verify bindings" "/FSharp.CloudEdge/guide/verify-bindings/"
                ]
            Menu.section
                "Control plane"
                [
                    Menu.page "libraries/control-plane/index.md"
                    Menu.link "Credentials" "/FSharp.CloudEdge/guide/credentials/"
                    Menu.page "libraries/control-plane/account-setup.md"
                    Menu.page "libraries/control-plane/worker-upload.md"
                    Menu.page "libraries/control-plane/asset-uploads.md"
                    Menu.page "libraries/control-plane/clients.md"
                ]
        ]
    |> Theme.navbarEnd
        [
            NavbarLink ("NuGet", nugetUrl)
            NavbarIcon ("GitHub", "https://github.com/fsprojects/FSharp.CloudEdge", Icons.github)
        ]
    |> Theme.editUrl "https://github.com/fsprojects/FSharp.CloudEdge/edit/main/site"
    |> Theme.lightTokens lightColours
    |> Theme.darkTokens darkColours
    |> Theme.lightSyntax lightSyntax
    |> Theme.darkSyntax darkSyntax
    |> Theme.layerAfter "responsive" "landing" (themeCss "landing.css")
    |> Theme.layerAfter "landing" "pages" (themeCss "pages.css")
    |> Theme.headExtra [ darkByDefault ]
    |> Theme.footer (
        Html.p
            [
                Html.text "FSharp.CloudEdge · MIT · "
                Html.a [ prop.href nugetUrl; prop.text "NuGet" ]
                Html.text " · "
                Html.a [ prop.href "https://github.com/fsprojects/FSharp.CloudEdge"; prop.text "GitHub" ]
                Html.text " · Built with Nacara"
            ]
    )

let site =
    Site.create "FSharp.CloudEdge"
    |> Site.baseUrl baseUrl
    |> Site.origin "https://fsprojects.github.io"
    |> Site.output "output"
    |> Site.staticFiles "static"
    |> Markdown.register
    |> TreeSitter.register
    |> Sitemap.register
    |> LinkValidator.register
    |> Theme.register theme
    |> Site.plugin nativeThemes
    |> Site.collection (Theme.docs theme "content")

[<EntryPoint>]
let main argv = Nacara.run site argv
