#r "../packages/Suave/lib/net40/Suave.dll"

open Suave                 // always open suave
open Suave.Successful      // for OK-result
open Suave.Web             // for config

type OAuthSecret = {
    ClientId: string
    ClientSecret: string
}

let githubOauth = {
    ClientId = "GITHUB_CLIENT_ID"
    ClientSecret = "GITHUB_CLIENT_SECRET"
}

startWebServer defaultConfig (sprintf "Github client ID is %s" githubOauth.ClientId |> OK) // Check that it gets compiled into the .exe properly