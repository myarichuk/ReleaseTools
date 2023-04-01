namespace ChangelogGenerator.Api
{
    public partial record struct Changelog(IEnumerable<ChangelogEntry> Entries, SemanticVersion Version);
}
