#r @".buildTools/FAKE/tools/FakeLib.dll"

open Fake
open Fake.AssemblyInfoFile
open Fake.FileUtils
open Fake.FileSystemHelper
open System.IO


let buildAppOutputDir = ".buildAppOutput"
let buildTestsOutputDir = environVarOrDefault "paths.buildTestsOutput" ".buildTestsOutput"
let artifactsDir = environVarOrDefault "paths.artifacts" ".artifacts"
let nugetPackagesDir = environVarOrDefault "paths.nugetPaсkages" ".deployedPackages"
let nugetPackagesWorkingDir = ".nugetWorkingDir"


let version = environVarOrDefault "VERSION" "1.0.0"
let buildConfiguration = environVarOrDefault "build.configuration" "Release"
let buildPlatform = environVarOrDefault "build.platform" "x64"
let publishPackages = getEnvironmentVarAsBoolOrDefault "build.publishPackages" false
let runTests = getEnvironmentVarAsBoolOrDefault "build.runTests" false

let artifactsOfVersionDir = artifactsDir @@ version 


let authors = ["TRUSTSYS CR Team"]


Target "CleanDirs" (fun _ ->

    CleanDir buildAppOutputDir
    CleanDir buildTestsOutputDir

)

Target "RestorePackages" (fun _ -> 
     "./Trustsys.Crawler.sln"
     |> RestoreMSSolutionPackages (fun p ->
         { p with
             Sources = "https://www.nuget.org/api/v2/" :: "http://192.168.1.67:8070/nuget/" :: p.Sources
             OutputPath = "./packages"
             Retries = 4 })
 )



Target "CrawlerContracts_GenerateAssemblyInfo" (fun _ ->

    CreateCSharpAssemblyInfo "Crawler.Contracts/Properties/AssemblyInfo.cs"
        [Attribute.Title "Trustsys.Crawler.Contracts"
         Attribute.Description ""
         Attribute.Guid "583938A0-49BF-4319-81FC-00B87A700ED6"
         Attribute.Product "Trustsys.Crawler.Contracts"
         Attribute.Version version
         Attribute.FileVersion version]

)

Target "CrawlerContractsImpl_GenerateAssemblyInfo" (fun _ ->

    CreateCSharpAssemblyInfo "Trustsys.Crawler.Contracts.Impl/Properties/AssemblyInfo.cs"
        [Attribute.Title "Trustsys.Crawler.Contracts.Impl"
         Attribute.Description ""
         Attribute.Guid "284D1FEF-3E4D-4759-B3D7-F26BAB54E2CB"
         Attribute.Product "Trustsys.Crawler.Contracts.Impl"
         Attribute.Version version
         Attribute.FileVersion version]

)

Target "CrawlerDal_GenerateAssemblyInfo" (fun _ ->

    CreateCSharpAssemblyInfo "Crawler.Dal/Properties/AssemblyInfo.cs"
        [Attribute.Title "Trustsys.Crawler.Dal"
         Attribute.Description ""
         Attribute.Guid "30B21573-9A66-4861-9758-E41692F98145"
         Attribute.Product "Trustsys.Crawler.Dal"
         Attribute.Version version
         Attribute.FileVersion version]

)

Target "CrawlerWebParserContracts_GenerateAssemblyInfo" (fun _ ->

    CreateCSharpAssemblyInfo "Web.Parser.Contracts/Properties/AssemblyInfo.cs"
        [Attribute.Title "Trustsys.Crawler.Web.Parser.Contracts"
         Attribute.Description ""
         Attribute.Guid "FA6A66D0-4707-49B1-95F0-56D0EC22BED2"
         Attribute.Product "Trustsys.Crawler.Web.Parser.Contracts"
         Attribute.Version version
         Attribute.FileVersion version]

)

// <<------------BUILD


Target "CrawlerContracts_Build" (fun _ ->

    let localBuidlAppOutputDir = buildAppOutputDir @@ "Trustsys.Crawler.Contracts"

    !! ("Crawler.Contracts" @@ "Trustsys.Crawler.Contracts.csproj")

      |> MSBuild localBuidlAppOutputDir "Build" ["Configuration", buildConfiguration; "Platform", buildPlatform]
      |> Log "BuildApp-Output: "
)

Target "CrawlerContractsImpl_Build" (fun _ ->

    let localBuidlAppOutputDir = buildAppOutputDir @@ "Trustsys.Crawler.Contracts.Impl"

    !! ("Trustsys.Crawler.Contracts.Impl" @@ "Trustsys.Crawler.Contracts.Impl.csproj")

      |> MSBuild localBuidlAppOutputDir "Build" ["Configuration", buildConfiguration; "Platform", buildPlatform]

      |> Log "BuildApp-Output: "
)

Target "CrawlerDal_Build" (fun _ ->

    let localBuidlAppOutputDir = buildAppOutputDir @@ "Trustsys.Crawler.Dal"

    !! ("Crawler.Dal" @@ "Trustsys.Crawler.Dal.csproj")

      |> MSBuild localBuidlAppOutputDir "Build" ["Configuration", buildConfiguration; "Platform", buildPlatform]

      |> Log "BuildApp-Output: "
)

Target "CrawlerWebParserContracts_Build" (fun _ ->

    let localBuidlAppOutputDir = buildAppOutputDir @@ "Trustsys.Crawler.Web.Parser.Contracts"

    !! ("Web.Parser.Contracts" @@ "Trustsys.Crawler.Web.Parser.Contracts.csproj")

      |> MSBuild localBuidlAppOutputDir "Build" ["Configuration", buildConfiguration; "Platform", buildPlatform]

      |> Log "BuildApp-Output: "
)

//>>-------------APP

// <<-----------ARTIFACTS

Target "CrawlerContracts_Nuget" (fun _ ->

    let localBuildOutputDir = buildAppOutputDir @@ "Trustsys.Crawler.Contracts"
    let workingDir = nugetPackagesWorkingDir @@ "Trustsys.Crawler.Contracts"
    let net452Dir = workingDir @@ "lib" @@ "net452"

    ensureDirectory (net452Dir)
    ensureDirectory (artifactsOfVersionDir)

    Copy workingDir [ (localBuildOutputDir @@ "Trustsys.Crawler.Contracts.nuspec") ]
    Copy net452Dir [ (localBuildOutputDir @@ "Trustsys.Crawler.Contracts.dll")]

    let nuspecFile = workingDir @@ "Trustsys.Crawler.Contracts.nuspec"

    NuGetPack(fun p ->
        { p with
            Authors = authors
            Project = "Trustsys.Crawler.Contracts"
            Description = "Crawler Contracts library"
            OutputPath = artifactsOfVersionDir 
            WorkingDir = workingDir
            DependenciesByFramework = [{FrameworkVersion = "net452"
                                        Dependencies =
                                        [                                        
                                            "Newtonsoft.Json", GetPackageVersion "./packages/" "Newtonsoft.Json"
                                        ]}]
            FrameworkAssemblies = 
                [ 
                    { 
                        AssemblyName = "Microsoft.CSharp" 
                        FrameworkVersions = ["net452"] 
                    }
                    { 
                        AssemblyName = "System" 
                        FrameworkVersions = ["net452"] 
                    }
                    { 
                        AssemblyName = "System.Core" 
                        FrameworkVersions = ["net452"] 
                    }
                    {
                        AssemblyName = "System.Web.Extensions" 
                        FrameworkVersions = ["net452"]
                    }
                    {
                        AssemblyName = "System.Data.DataSetExtensions"
                        FrameworkVersions = ["net452"]
                    }
                    {
                        AssemblyName = "System.Xml"
                        FrameworkVersions = ["net452"]
                    }
                    {
                        AssemblyName = "System.Xml.Linq"
                        FrameworkVersions = ["net452"]
                    }
                    {
                        AssemblyName = "System.Data" 
                        FrameworkVersions = ["net452"]
                    }
                ]
            Version = version
            Publish = false
        })
            nuspecFile
)

//



Target "CrawlerContractsImpl_Nuget" (fun _ ->

    let localBuildOutputDir = buildAppOutputDir @@ "Trustsys.Crawler.Contracts.Impl"
    let workingDir = nugetPackagesWorkingDir @@ "Trustsys.Crawler.Contracts.Impl"
    let net40Dir = workingDir @@ "lib" @@ "net452"

    ensureDirectory (net40Dir)
    ensureDirectory (artifactsOfVersionDir)

    Copy workingDir [ (localBuildOutputDir @@ "Trustsys.Crawler.Contracts.Impl.nuspec") ]
    Copy net40Dir [ (localBuildOutputDir @@ "Trustsys.Crawler.Contracts.Impl.dll")]

    let nuspecFile = workingDir @@ "Trustsys.Crawler.Contracts.Impl.nuspec"

    NuGetPack(fun p ->
        { p with
            Authors = authors
            Project = "Trustsys.Crawler.Contracts.Impl"
            Description = "Crawler Contracts Implementation library"
            OutputPath = artifactsOfVersionDir 
            WorkingDir = workingDir
            DependenciesByFramework = [{FrameworkVersion = "net40"
                                        Dependencies =
                                        [
                                            "Trustsys.Crawler.Contracts", version
                                            "Trustsys.Crawler.Dal", version
                                            "Trustsys.Crawler.Web.Parser.Contracts", version
                                            "HtmlAgilityPack", GetPackageVersion "./packages/" "HtmlAgilityPack"
                                            "Newtonsoft.Json", GetPackageVersion "./packages/" "Newtonsoft.Json"
                                            "Service.Common", GetPackageVersion "./packages/" "Service.Common"
                                            "Service.Registry", GetPackageVersion "./packages/" "Service.Registry"
                                        ]}]
            FrameworkAssemblies = 
                [
                    {
                        AssemblyName = "System.Configuration" 
                        FrameworkVersions = ["net452"]
                    }
                    {
                        AssemblyName = "System.Core"
                        FrameworkVersions = ["net452"]
                    }
                    {
                        AssemblyName = "System"
                        FrameworkVersions = ["net452"]
                    }
                    {
                        AssemblyName = "System.Runtime.Caching"
                        FrameworkVersions = ["net452"]
                    }
                    {
                        AssemblyName = "System.Runtime.Serialization"
                        FrameworkVersions = ["net452"]
                    }
                    {
                        AssemblyName = "System.ServiceModel" 
                        FrameworkVersions = ["net452"]
                    }
                    {
                        AssemblyName = "System.Web" 
                        FrameworkVersions = ["net452"]
                    }
                    {
                        AssemblyName = "System.Xml" 
                        FrameworkVersions = ["net452"]
                    }
                    {
                        AssemblyName = "System.Xml.Linq" 
                        FrameworkVersions = ["net452"]
                    }
                    {
                        AssemblyName = "System.Data" 
                        FrameworkVersions = ["net452"]
                    }
                    {
                        AssemblyName = "System.Data.DataSetExtensions" 
                        FrameworkVersions = ["net452"]
                    }
                    {
                        AssemblyName = "Microsoft.CSharp" 
                        FrameworkVersions = ["net452"]
                    }
                    {
                        AssemblyName = "System.Net.Http"
                        FrameworkVersions = ["net452"]
                    }
                    
                    
                ]
            Version = version
            Publish = false
        })
            nuspecFile
)

Target "CrawlerDal_Nuget" (fun _ ->

    let localBuildOutputDir = buildAppOutputDir @@ "Trustsys.Crawler.Dal"
    let workingDir = nugetPackagesWorkingDir @@ "Trustsys.Crawler.Dal"
    let net40Dir = workingDir @@ "lib" @@ "net452"

    ensureDirectory (net40Dir)
    ensureDirectory (artifactsOfVersionDir)

    Copy workingDir [ (localBuildOutputDir @@ "Trustsys.Crawler.Dal.nuspec") ]
    Copy net40Dir [ (localBuildOutputDir @@ "Trustsys.Crawler.Dal.dll")]

    let nuspecFile = workingDir @@ "Trustsys.Crawler.Dal.nuspec"

    NuGetPack(fun p ->
        { p with
            Authors = authors
            Project = "Trustsys.Crawler.Dal"
            Description = "Crawler repository"
            OutputPath = artifactsOfVersionDir 
            WorkingDir = workingDir
            DependenciesByFramework = [{FrameworkVersion = "net452"
                                        Dependencies =
                                        [
                                            "Trustsys.Crawler.Contracts", version
                                            "Dapper", GetPackageVersion "./packages/" "Dapper"
                                        ]}]
            FrameworkAssemblies = 
                [
                    {
                        AssemblyName = "System.Configuration" 
                        FrameworkVersions = ["net452"]
                    }
                    {
                        AssemblyName = "System.Core"
                        FrameworkVersions = ["net452"]
                    }
                    {
                        AssemblyName = "System"
                        FrameworkVersions = ["net452"]
                    }
                    {
                        AssemblyName = "System.Xml" 
                        FrameworkVersions = ["net452"]
                    }
                    {
                        AssemblyName = "System.Xml.Linq" 
                        FrameworkVersions = ["net452"]
                    }
                    {
                        AssemblyName = "System.Data" 
                        FrameworkVersions = ["net452"]
                    }
                    {
                        AssemblyName = "System.Data.DataSetExtensions" 
                        FrameworkVersions = ["net452"]
                    }
                    {
                        AssemblyName = "Microsoft.CSharp" 
                        FrameworkVersions = ["net452"]
                    }
                    {
                        AssemblyName = "System.Net.Http"
                        FrameworkVersions = ["net452"]
                    }
                    
                    
                ]
            Version = version
            Publish = false
        })
            nuspecFile
)

Target "CrawlerWebParserContracts_Nuget" (fun _ ->

    let localBuildOutputDir = buildAppOutputDir @@ "Trustsys.Crawler.Web.Parser.Contracts"
    let workingDir = nugetPackagesWorkingDir @@ "Trustsys.Crawler.Web.Parser.Contracts"
    let net452Dir = workingDir @@ "lib" @@ "net452"

    ensureDirectory (net452Dir)
    ensureDirectory (artifactsOfVersionDir)

    Copy workingDir [ (localBuildOutputDir @@ "Trustsys.Crawler.Web.Parser.Contracts.nuspec") ]
    Copy net452Dir [ (localBuildOutputDir @@ "Trustsys.Crawler.Web.Parser.Contracts.dll")]

    let nuspecFile = workingDir @@ "Trustsys.Crawler.Web.Parser.Contracts.nuspec"

    NuGetPack(fun p ->
        { p with
            Authors = authors
            Project = "Trustsys.Crawler.Web.Parser.Contracts"
            Description = "Crawler web parser contracts"
            OutputPath = artifactsOfVersionDir 
            WorkingDir = workingDir
            DependenciesByFramework = [{FrameworkVersion = "net452"
                                        Dependencies =
                                        [  
                                            "Trustsys.Crawler.Contracts", version                                      
                                        ]}]
            FrameworkAssemblies = 
                [ 
                    { 
                        AssemblyName = "Microsoft.CSharp" 
                        FrameworkVersions = ["net452"] 
                    }
                    { 
                        AssemblyName = "System" 
                        FrameworkVersions = ["net452"] 
                    }
                    { 
                        AssemblyName = "System.Core" 
                        FrameworkVersions = ["net452"] 
                    }
                    {
                        AssemblyName = "System.Runtime.Serialization" 
                        FrameworkVersions = ["net452"]
                    }
                    {
                        AssemblyName = "System.ServiceModel" 
                        FrameworkVersions = ["net452"]
                    }

                    {
                        AssemblyName = "System.Data.DataSetExtensions"
                        FrameworkVersions = ["net452"]
                    }
                    {
                        AssemblyName = "System.Xml"
                        FrameworkVersions = ["net452"]
                    }
                    {
                        AssemblyName = "System.Net.Http"
                        FrameworkVersions = ["net452"]
                    }
                    {
                        AssemblyName = "System.Xml.Linq"
                        FrameworkVersions = ["net452"]
                    }
                    {
                        AssemblyName = "System.Data" 
                        FrameworkVersions = ["net452"]
                    }
                ]
            Version = version
            Publish = false
        })
            nuspecFile
)

// >>-----------ARTIFACTS


Target "PublishPackages" (fun _ -> 

    let nupkgs = directoryInfo(artifactsOfVersionDir).EnumerateFiles "*.nupkg"

    ensureDirectory (nugetPackagesDir)

    for nupkg in nupkgs do
        
        let nupkgFile = nupkg.Name

        cp nupkg.FullName (nugetPackagesDir @@ nupkgFile)

)


FinalTarget "Done" DoNothing


"CleanDirs"
    ==> "CrawlerContracts_GenerateAssemblyInfo"
    ==> "CrawlerContractsImpl_GenerateAssemblyInfo"
    ==> "CrawlerDal_GenerateAssemblyInfo"
    ==> "CrawlerWebParserContracts_GenerateAssemblyInfo"

    ==> "RestorePackages"

    ==> "CrawlerContracts_Build"
    ==> "CrawlerContractsImpl_Build"
    ==> "CrawlerDal_Build"
    ==> "CrawlerWebParserContracts_Build"

    ==> "CrawlerContracts_Nuget"
    ==> "CrawlerContractsImpl_Nuget"
    ==> "CrawlerDal_Nuget"
    ==> "CrawlerWebParserContracts_Nuget"

    ==> "PublishPackages"

    ==> "Done"



RunTargetOrDefault "Done"