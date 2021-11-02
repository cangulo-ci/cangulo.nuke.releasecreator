using cangulo.nuke.releasecreator.Repository;
using Microsoft.Extensions.DependencyInjection;
using Nuke.Common;
using System.Linq;

internal partial class Build : NukeBuild
{
    private Target ListCommits => _ => _
        .Unlisted()
        .Requires(() => PrNumberProvided())
        .Executes(async () =>
        {
            ParseSettings();
            var serviceProvider = SetupServices();

            var resultBagRepository = serviceProvider.GetRequiredService<IResultBagRepository>();

            ControlFlow.NotNull(GitHubActions, "This Target can't be executed locally");

            var repoOwner = GitHubActions.GitHubRepositoryOwner;
            var repoName = GitHubActions.GitHubRepository.Replace($"{repoOwner}/", string.Empty);
            var ghClient = GetGHClient(GitHubActions);

            ControlFlow.Assert(
                int.TryParse(PullRequestNumber, out int prNumber),
                $"Pull Request Number is invalid. Value Provide from the env vars is {PullRequestNumber}");

            var commitsFullDetails = await ghClient.Repository.PullRequest.Commits(repoOwner, repoName, prNumber);
            var commitMsgs = commitsFullDetails.Select(x => x.Commit.Message);
            //var commitMsgs = new string[] { "feat:wip-123bla bla", "feat:WIP-123bla bla", "Fix:WiP-133bla bla" };

            ControlFlow.Assert(commitMsgs.Any(), $"no commit messages found");

            Logger.Info("Commits:");
            commitMsgs
                .ToList()
                .ForEach(x => Logger.Info($"\t{x}"));

            resultBagRepository.AddOrReplaceResult(nameof(ListCommits), commitMsgs.ToArray());
        });

    private bool PrNumberProvided()
        => int.TryParse(PullRequestNumber, out int prNumber);
}