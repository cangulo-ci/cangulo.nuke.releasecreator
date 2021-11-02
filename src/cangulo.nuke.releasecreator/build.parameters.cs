using cangulo.nuke.releasecreator.Models;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.Tooling;

internal partial class Build : NukeBuild
{

    [Parameter("Path to Release Settings path. Default value: releaseSettings.json")]
    public AbsolutePath ReleaseSettingsPath { get; } = RootDirectory / "releaseSettings.json";

    private ReleaseSettings ReleaseSettings;

    // [Parameter("The result bag is a temporary file created in the pipeline to share parameters between targets. Default value: release.resultbag.json")]
    public AbsolutePath ResultBagFilePath { get; } = RootDirectory / "release.resultbag.json";

    [GitRepository]
    private readonly GitRepository GitRepository;

    [PathExecutable("git")]
    private readonly Tool Git;

    [Parameter("GH token to authenticate when querying GH API"), Secret]
    private readonly string GitHubToken;

    [Parameter("Pull Request Number to use when quering for its commits"), Secret]
    private readonly string PullRequestNumber;

    [CI]
    private readonly GitHubActions GitHubActions;
}