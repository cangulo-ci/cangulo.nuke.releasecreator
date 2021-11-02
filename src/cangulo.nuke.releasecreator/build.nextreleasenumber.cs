using cangulo.nuke.releasecreator.Helpers;
using cangulo.nuke.releasecreator.Mappers;
using cangulo.nuke.releasecreator.Models;
using cangulo.nuke.releasecreator.Parsers;
using cangulo.nuke.releasecreator.Repository;
using cangulo.nuke.releasecreator.Services;
using Microsoft.Extensions.DependencyInjection;
using Nuke.Common;
using System;
using System.Collections.Generic;
using System.Linq;

internal partial class Build : NukeBuild
{
    private Target CalculateNextReleaseNumber => _ => _
        .DependsOn(ListCommits)
        .Executes(() =>
        {
            ParseSettings();
            var serviceProvider = SetupServices();

            var commitParser = serviceProvider.GetRequiredService<ICommitParser>();
            var nextReleaseNumberHelper = serviceProvider.GetRequiredService<INextReleaseNumberHelper>();
            var releaseNumberParser = serviceProvider.GetRequiredService<IReleaseNumberParser>();
            var releaseTypeMapper = serviceProvider.GetRequiredService<IReleaseTypeMapper>();
            var resultBagRepository = serviceProvider.GetRequiredService<IResultBagRepository>();

            var commitMsgs = resultBagRepository.GetResultOrDefault(nameof(ListCommits), Array.Empty<string>());
            ControlFlow.Assert(commitMsgs.Any(), "no commits founds");

            var releaseType = GetReleaseTypeFromCommits(commitParser, releaseTypeMapper, commitMsgs);

            if (releaseType is ReleaseType.Undefined)
                Logger.Success("no release required");
            else
            {
                var currentReleaseNumber = ReadVersionFromFile(releaseNumberParser);
                var nextReleaseNumber = nextReleaseNumberHelper.Calculate(releaseType, currentReleaseNumber);

                Logger.Info($"Next release Number:{nextReleaseNumber} - Release Type: {releaseType}");

                resultBagRepository.AddOrReplaceResult(nameof(SetNextReleaseNumber), nextReleaseNumber.ToString());

                Logger.Success($"saved next release number ({nextReleaseNumber}) in {ResultBagFilePath}");
            }
        });

    private Target SetNextReleaseNumber => _ => _
        .DependsOn(ListCommits)
        .Executes(() =>
        {
            ParseSettings();
            var serviceProvider = SetupServices();

            var commitParser = serviceProvider.GetRequiredService<ICommitParser>();
            var nextReleaseNumberHelper = serviceProvider.GetRequiredService<INextReleaseNumberHelper>();
            var releaseNumberParser = serviceProvider.GetRequiredService<IReleaseNumberParser>();
            var releaseTypeMapper = serviceProvider.GetRequiredService<IReleaseTypeMapper>();
            var resultBagRepository = serviceProvider.GetRequiredService<IResultBagRepository>();

            var commitMsgs = resultBagRepository.GetResultOrDefault(nameof(ListCommits), Array.Empty<string>());
            ControlFlow.Assert(commitMsgs.Any(), "no commits founds");

            var releaseType = GetReleaseTypeFromCommits(commitParser, releaseTypeMapper, commitMsgs);

            ControlFlow.Assert(releaseType is not ReleaseType.Undefined, "no release is required. Commits provided don't have a release type.");

            var currentReleaseNumber = ReadVersionFromFile(releaseNumberParser);
            var nextReleaseNumber = nextReleaseNumberHelper.Calculate(releaseType, currentReleaseNumber);

            Logger.Info($"next release Number:{nextReleaseNumber} - Release Type: {releaseType}");

            resultBagRepository.AddOrReplaceResult(nameof(SetNextReleaseNumber), nextReleaseNumber.ToString());

            Logger.Success($"saved next release number ({nextReleaseNumber}) in {ResultBagFilePath}");
        });

    private ReleaseType GetReleaseTypeFromCommits(ICommitParser commitParser, IReleaseTypeMapper releaseTypeMapper, IEnumerable<string> commitMsgs)
    {
        var commits = commitMsgs.ToList();

        Logger.Info($"{commits.Count} commits found:");
        commits
            .ForEach(x => Logger.Info($"\t{x}"));

        ControlFlow.NotEmpty(
            ReleaseSettings.ConventionalCommitsAllowed,
            "Please provide the conventional commits settings");

        var allowedCommitTypes = ReleaseSettings
                                    .ConventionalCommitsAllowed
                                    .Select(x => x.CommitType)
                                    .ToArray();
        var conventionalCommits = GetConventionalCommits(commitParser, commits, allowedCommitTypes);

        var commitTypesInput = conventionalCommits.Select(x => x.CommitType).Distinct();
        var releaseType = releaseTypeMapper.MapReleaseType(commitTypesInput, ReleaseSettings.ConventionalCommitsAllowed);
        return releaseType;
    }


}