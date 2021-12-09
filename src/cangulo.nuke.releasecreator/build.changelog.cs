using cangulo.changelog.builders;
using cangulo.nuke.releasecreator.Models;
using cangulo.nuke.releasecreator.Repository;
using Microsoft.Extensions.DependencyInjection;
using Nuke.Common;
using System.Linq;

internal partial class Build : NukeBuild
{
    private Target UpdateChangelog => _ => _
        .DependsOn(SetNextReleaseNumber, ListCommits)
        .OnlyWhenDynamic(() => ChangelogSettingsAreDefined())
        .Before(PushFilesUpdatedWithNewVersion)
        .Executes(() =>
        {
            ParseSettings();

            if (ReleaseSettings.ChangelogSettings is null)
                Logger.Info("No changelog settings defined");
            else
            {
                var serviceProvider = SetupServices();

                var resultBagRepository = serviceProvider.GetRequiredService<IResultBagRepository>();

                var nextVersion = GetNextVersion(resultBagRepository);
                var commitMsgs = resultBagRepository.GetResult<string[]>(nameof(ListCommits));
                ControlFlow.Assert(commitMsgs.Any(), $"no commit messages found");

                var changelogBuilder = serviceProvider.GetRequiredService<IChangelogBuilder>();
                var changelogPath = ReleaseSettings.ChangelogSettings.ChangelogPath;
                changelogBuilder.Build(nextVersion, commitMsgs, changelogPath);

                Logger.Success("Updated changelog");

                EnqueueFileToBePushed(resultBagRepository, changelogPath);
            }
        });

    private bool ChangelogSettingsAreDefined()
    {
        ParseSettings();
        return ReleaseSettings.ChangelogSettings is not null;
    }
}