using cangulo.changelog.builders;
using cangulo.nuke.releasecreator.Models;
using cangulo.nuke.releasecreator.Repository;
using Microsoft.Extensions.DependencyInjection;
using Nuke.Common;
using Octokit;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

internal partial class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.CreateRelease);

    private Target CreateRelease => _ => _
        .DependsOn(
            ListCommits,
            SetNextReleaseNumber,
            UpdateVersionInCSProj,
            UpdateVersionTrackerFile,
            UpdateChangelog,
            PushFilesUpdatedWithNewVersion)
        .Executes(async () =>
        {
            ParseSettings();
            var serviceProvider = SetupServices();

            var releaseBodyBuilder = serviceProvider.GetRequiredService<IReleaseNotesBuilder>();
            var resultBagRepository = serviceProvider.GetRequiredService<IResultBagRepository>();

            var nextVersion = GetNextVersion(resultBagRepository);

            ControlFlow.NotNull(GitHubActions, "This Target can't be executed locally");

            var repoOwner = GitHubActions.GitHubRepositoryOwner;
            var repoName = GitHubActions.GitHubRepository.Replace($"{repoOwner}/", string.Empty);
            var ghClient = GetGHClient(GitHubActions);

            var commitMsgs = resultBagRepository.GetResult<string[]>(nameof(ListCommits));
            ControlFlow.Assert(commitMsgs.Any(), $"no commit messages found");

            Logger.Info($"Creating Release {nextVersion}");

            var tagName = $"v{nextVersion}";
            var newReleaseData = new NewRelease(tagName)
            {
                Name = tagName,
                Body = releaseBodyBuilder.Build(commitMsgs)
            };

            var releaseOperatorClient = ghClient.Repository.Release;
            var releaseCreated = await releaseOperatorClient.Create(repoOwner, repoName, newReleaseData);

            Logger.Success($"Release {nextVersion} created!");

            var assets = ReleaseSettings.ReleaseAssets;
            if (assets.Any())
                await PushAssets(releaseOperatorClient, releaseCreated, assets);
        });

    private static async Task PushAssets(IReleasesClient releaseOperatorClient, Release releaseCreated, string[] assets)
    {
        foreach (var releaseAsset in assets)
        {
            var fileName = Path.GetFileName(releaseAsset);

            var assetData = new ReleaseAssetUpload
            {
                FileName = fileName,
                RawData = File.OpenRead(RootDirectory / releaseAsset),
                ContentType = "application/zip"
            };
            await releaseOperatorClient.UploadAsset(releaseCreated, assetData);
            Logger.Info($"Asset {fileName} uploaded");
        }
    }
}