// include Fake libs
#r "./packages/FAKE/tools/FakeLib.dll"

open Fake

// Directories
let buildDir  = "./build/"
let deployDir = "./deploy/"


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

let tellSecrets files =
    let githubId, githubSecret = readSecrets "github"
    printfn "Github client ID: %s" githubId 

// Targets
Target "Clean" (fun _ ->
    CleanDirs [buildDir; deployDir]
)

Target "Tattle" (fun _ ->
    !! "/**/*.*"
        |> tellSecrets
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
  ==> "Tattle"
  ==> "Build"
  ==> "Deploy"

// start build
RunTargetOrDefault "Build"
