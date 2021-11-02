namespace cangulo.nuke.releasecreator.Models
{
    public class ConventionalCommit
    {
        // FOLLOWING https://www.conventionalcommits.org/en/v1.0.0/

        public string CommitType { get; set; }
        public string Body { get; set; }

        public override string ToString()
            => $"{CommitType}: {Body}";
    }
}
