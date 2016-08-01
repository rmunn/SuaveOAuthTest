// include Fake libs
#r "./packages/FAKE/tools/FakeLib.dll"

open Fake

// Directories
let buildDir  = "./build/"
let deployDir = "./deploy/"

// Encodings
let utf8 = System.Text.UTF8Encoding(false)

// Filesets
let appReferences  =
    !! "/**/*.csproj"
    ++ "/**/*.fsproj"

// version info
let version = "0.1"  // or retrieve from CI server

let secretsDir = "./secrets"

let readSecrets servicename =
    let clientIdFname = sprintf "%s-client-id.txt" servicename
    let clientSecretFname = sprintf "%s-client-secret.txt" servicename
    let clientId = ReadLine (secretsDir </> clientIdFname)
    let clientSecret = ReadLine (secretsDir </> clientSecretFname)
    (clientId, clientSecret)

let replaceSecrets files =
    let githubId, githubSecret = readSecrets "github"
    files |> RegexReplaceInFilesWithEncoding "GITHUB_CLIENT_ID" githubId utf8
    files |> RegexReplaceInFilesWithEncoding "GITHUB_CLIENT_SECRET" githubSecret utf8

let unReplaceSecrets files =
    let githubId, githubSecret = readSecrets "github"
    files |> RegexReplaceInFilesWithEncoding githubId "GITHUB_CLIENT_ID" utf8
    files |> RegexReplaceInFilesWithEncoding githubSecret "GITHUB_CLIENT_SECRET" utf8

// Targets
Target "Clean" (fun _ ->
    CleanDirs [buildDir; deployDir]
)

Target "ReplaceSecrets" (fun _ ->
    // TODO: Change glob below once I move secrets into their own .fsx file
    !! "/**/TryOAuth.fsx"
        |> replaceSecrets
)

Target "UnReplaceSecrets" (fun _ ->
    // TODO: Change glob below once I move secrets into their own .fsx file
    !! "/**/TryOAuth.fsx"
        |> unReplaceSecrets
)

Target "Build" (fun _ ->
    // compile all projects below src/app/
    MSBuildDebug buildDir "Build" appReferences
    |> Log "AppBuild-Output: "
)

Target "Deploy" (fun _ ->
    !! (buildDir + "/**/*.*")
    -- "*.zip"
    |> Zip buildDir (deployDir + "TryOAuth." + version + ".zip")
)

// Build order
"Clean"
  ==> "ReplaceSecrets"
  ==> "Build"
  ==> "UnReplaceSecrets"
  ==> "Deploy"

// start build
RunTargetOrDefault "UnReplaceSecrets"
