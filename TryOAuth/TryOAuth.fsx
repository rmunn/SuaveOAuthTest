#r "../packages/Suave/lib/net40/Suave.dll"

open Suave                 // always open suave
open Suave.Successful      // for OK-result
open Suave.Web             // for config
open Suave.Html

type OAuthSecret = {
    ClientId: string
    ClientSecret: string
}

let githubOauth = {
    ClientId = "GITHUB_CLIENT_ID"
    ClientSecret = "GITHUB_CLIENT_SECRET"
}

let basePage =
    html [
        head [
            metaAttr ["charset", "utf-8"]
            title "A demo of OAuth"
        ]
        body [
            tag "h1" [] (text "How's that?")
            p (text "Lorem ipsum and all that nonsense")
        ]
    ]

startWebServer defaultConfig (basePage |> renderHtmlDocument |> OK)