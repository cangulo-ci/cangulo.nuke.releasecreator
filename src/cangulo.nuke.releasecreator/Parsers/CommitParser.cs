using cangulo.nuke.releasecreator.Models;
using System;
using System.Linq;

namespace cangulo.nuke.releasecreator.Parsers
{
    public interface ICommitParser
    {
        ConventionalCommit ParseConventionalCommit(string commitMsg, string[] conventionalCommitTypes);
    }

    public class CommitParser : ICommitParser
    {
        public ConventionalCommit ParseConventionalCommit(string commitMsg, string[] conventionalCommitTypes)
        {
            var parts = commitMsg.Split(":", StringSplitOptions.TrimEntries);

            if (parts.Length < 2 || parts.Any(string.IsNullOrEmpty))
                throw new ArgumentException(BuildErrorMsg(commitMsg));

            var inputComType = parts[0].Trim().ToLowerInvariant();
            var acceptedComTypes = conventionalCommitTypes.Select(x => x.Trim().ToLowerInvariant());

            if (acceptedComTypes.Any(x => x == inputComType))
            {
                return new ConventionalCommit
                {
                    CommitType = inputComType,
                    Body = parts[1].Trim()
                };
            }
            else
                throw new InvalidOperationException(BuildErrorMsg(commitMsg));
        }

        private static string BuildErrorMsg(string commitMsg)
            => $"The next commit msg does not provide a valid conventional commit type: {commitMsg}";
    }
}