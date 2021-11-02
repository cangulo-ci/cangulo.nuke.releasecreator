using cangulo.nuke.releasecreator.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace cangulo.nuke.releasecreator.Mappers
{
    public interface IReleaseTypeMapper
    {
        ReleaseType MapReleaseType(IEnumerable<string> conventionalCommitTypesInput, ConventionalCommitTypeVsReleaseType[] conventionalCommitsAllowed);
    }
    public class ReleaseTypeMapper : IReleaseTypeMapper
    {
        public ReleaseType MapReleaseType(IEnumerable<string> conventionalCommitTypesInput, ConventionalCommitTypeVsReleaseType[] conventionalCommitsAllowed)
        {
            var allowedCommitType = conventionalCommitsAllowed.Select(x => x.CommitType);
            var uniqueCommitsTypeProvided = conventionalCommitTypesInput
                                                .Distinct()
                                                .Intersect(allowedCommitType);

            return conventionalCommitsAllowed
                .Where(x =>
                    uniqueCommitsTypeProvided.Any(y => string.Equals(x.CommitType, y, StringComparison.OrdinalIgnoreCase)))
                .Select(x=>x.ReleaseType)
                .Max();
        }
    }
}
