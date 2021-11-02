using cangulo.nuke.releasecreator.Models;
using System;

namespace cangulo.nuke.releasecreator.Helpers
{
    public interface INextReleaseNumberHelper
    {
        ReleaseNumber Calculate(ReleaseType releaseType, ReleaseNumber currentVersion);
    }
    public class NextReleaseNumberHelper : INextReleaseNumberHelper
    {
        public ReleaseNumber Calculate(ReleaseType releaseType, ReleaseNumber currentVersion)
        {
            switch (releaseType)
            {
                case ReleaseType.Major:
                    return new ReleaseNumber { Major = currentVersion.Major + 1, Minor = 0, Patch = 0 };
                case ReleaseType.Minor:
                    return new ReleaseNumber { Major = currentVersion.Major, Minor = currentVersion.Minor + 1, Patch = 0 };
                case ReleaseType.Patch:
                    return new ReleaseNumber { Major = currentVersion.Major, Minor = currentVersion.Minor, Patch = currentVersion.Patch + 1 };
                default:
                    throw new InvalidOperationException("invalid release type provided");
            }
        }
    }
}
