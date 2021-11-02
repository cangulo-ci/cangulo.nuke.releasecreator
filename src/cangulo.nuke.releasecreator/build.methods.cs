using cangulo.nuke.releasecreator.Constants;
using cangulo.nuke.releasecreator.Models;
using cangulo.nuke.releasecreator.Parsers;
using cangulo.nuke.releasecreator.Repository;
using cangulo.nuke.releasecreator.Services;
using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Octokit;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

internal partial class Build : NukeBuild
{
    private GitHubClient GetGHClient(GitHubActions gitHubAction)
    {
        var repoOwner = gitHubAction.GitHubRepositoryOwner;

        // TODO: Migrate the injection of the client to an interface
        var ghClient = new GitHubClient(new ProductHeaderValue($"{repoOwner}"))
        {
            Credentials = new Credentials(GitHubToken)
        };

        return ghClient;
    }

    private static IEnumerable<ConventionalCommit> GetConventionalCommits(ICommitParser commitParser, List<string> commits, string[] commitTypesAllowed)
    => commits
        .Select(
            comMsg => commitParser.ParseConventionalCommit(comMsg, commitTypesAllowed));

    private ReleaseNumber ReadVersionFromFile(IReleaseNumberParser releaseNumberParser)
    {
        var jsonFileContent = File.ReadAllText(ReleaseSettings.VersionTrackerFilePath);
        var currentVersionFileModel = JsonSerializer.Deserialize<CurrentVersionFileModel>(jsonFileContent, SerializerContants.DESERIALIZER_OPTIONS);
        return releaseNumberParser.Parse(currentVersionFileModel.CurrentVersion);
    }

    private static string GetNextVersion(IResultBagRepository resultBagRepository)
    {
        var nextVersion = resultBagRepository.GetResult(nameof(SetNextReleaseNumber));
        return nextVersion;
    }
}
