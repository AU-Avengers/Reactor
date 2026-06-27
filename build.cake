var target = Argument("target", "Build");

var workflow = BuildSystem.GitHubActions.Environment.Workflow;
var buildId = workflow.RunNumber;
var tag = workflow.RefType == GitHubActionsRefType.Tag ? workflow.RefName : null;

Task("Build")
    .Does(() =>
{
    Information("Running dependency restoration...");
    DotNetRestore(new DotNetRestoreSettings {
        ArgumentCustomization = args => args.Append("--force-evaluate")
    });

    var settings = new DotNetBuildSettings
    {
        Configuration = "Release",
        NoRestore = true,
        MSBuildSettings = new DotNetMSBuildSettings()
    };

    if (tag != null)
    {
        settings.MSBuildSettings.Version = tag;
    }
    else if (buildId != 0)
    {
        settings.MSBuildSettings.VersionSuffix = "ci." + buildId;
    }

    DotNetBuild(".", settings);
});

RunTarget(target);
