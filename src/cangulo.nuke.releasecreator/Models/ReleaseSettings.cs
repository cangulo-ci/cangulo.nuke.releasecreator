using System;

namespace cangulo.nuke.releasecreator.Models
{
    public class ReleaseSettings
    {
        public string VersionTrackerFilePath { get; set; }
        public ConventionalCommitTypeVsReleaseType[] ConventionalCommitsAllowed { get; set; }
        public ChangelogSettings ChangelogSettings { get; set; }
        public UpdateVersionInCSProjSettings UpdateVersionInCSProjSettings { get; set; }
        public GitPushFilesSettings GitPushReleaseFilesSettings { get; set; }
        public string[] ReleaseAssets { get; set; } = Array.Empty<string>();
    }

    public class ConventionalCommitTypeVsReleaseType
    {
        public string CommitType { get; set; }
        public ReleaseType ReleaseType { get; set; }
    }
    public class UpdateVersionInCSProjSettings
    {
        public string ProjectPath { get; set; }
        public string PreReleaseVersionSuffix { get; set; }
    }

    public class CurrentVersionFileModel
    {
        public string CurrentVersion { get; set; }
    }

    public class GitPushFilesSettings
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string[] FoldersPath { get; set; } = Array.Empty<string>();
        public string[] FilesPath { get; set; } = Array.Empty<string>();
    }
}
