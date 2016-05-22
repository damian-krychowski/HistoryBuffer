var target = Argument("target", "Default");

Task("Restore-NuGet-Packages")
    .Does(() =>
{
    NuGetRestore("../Source/HistoryBuffer.sln");
});

Task("Clean")
    .Does(() =>
{
    CleanDirectories("../Source/**/bin");
    CleanDirectories("../Source/**/obj");
});

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
{
    MSBuild("../Source/HistoryBuffer.sln", settings => settings.SetConfiguration("Release"));
});

Task("Run-Unit-Tests")
    .IsDependentOn("Build")
    .Does(() =>
{
    NUnit("../Source/HistoryBuffer.Tests/bin/Release/HistoryBuffer.Tests.dll", new NUnitSettings {
        ToolPath = "../Source/packages/NUnit.ConsoleRunner.3.2.1/tools/nunit3-console.exe"
    });
});

Task("Create-NuGet-Package")
    .IsDependentOn("Run-Unit-Tests")
    .Does(() =>
{
    var nuGetPackSettings = new NuGetPackSettings {
        Id                      = "HistoryBuffer",
        Version                 = EnvironmentVariable("APPVEYOR_BUILD_VERSION"),
        Title                   = "HistoryBuffer",
        Authors                 = new[] {"Damian Krychowski"},
        Owners                  = new[] {"Damian Krychowski"},
        Description             = "HistoryBuffer with undo and repeat functionalities.",
        ProjectUrl              = new Uri("https://github.com/damian-krychowski/HistoryBuffer"),
        LicenseUrl              = new Uri("https://github.com/damian-krychowski/HistoryBuffer/blob/master/LICENSE"),
        Copyright               = "Damian Krychowski 2016",    
        RequireLicenseAcceptance= false,
        Symbols                 = false,
        NoPackageAnalysis       = true,
        Files                   = new[] { new NuSpecContent {Source = "bin/Release/HistoryBuffer.dll", Target = "lib/net452"} },
        BasePath                = "../Source/HistoryBuffer",
        OutputDirectory         = ".."
    };
    
    NuGetPack(nuGetPackSettings);
});

Task("Push-NuGet-Package")
    .IsDependentOn("Create-NuGet-Package")
    .Does(() =>
{
    var package = "../HistoryBuffer." + EnvironmentVariable("APPVEYOR_BUILD_VERSION") +".nupkg";
                
    NuGetPush(package, new NuGetPushSettings {
        Source = "https://nuget.org/",
        ApiKey = EnvironmentVariable("NUGET_API_KEY")
    });
});


Task("Default")
	.IsDependentOn("Push-NuGet-Package")
    .Does(() =>
{
    Information("HistoryBuffer building finished.");
});

RunTarget(target);