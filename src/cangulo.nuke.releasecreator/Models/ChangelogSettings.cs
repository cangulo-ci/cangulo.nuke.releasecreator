namespace cangulo.nuke.releasecreator.Models
{
    public class ChangelogSettings : cangulo.changelog.Models.ChangelogSettings
    {
        public string ChangelogPath { get; set; } = "./CHANGELOG.md";
    }
}
