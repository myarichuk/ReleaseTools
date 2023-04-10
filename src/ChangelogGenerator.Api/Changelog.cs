using Parser.ConventionalCommit;

namespace ChangelogGenerator.Api
{
    /// <summary>
    /// Represents a readonly partial record struct that contains changelog entries and a semantic version of the release.
    /// </summary>
    /// <param name="Entries">An enumerable collection of <see cref="ChangelogEntry"/> objects, representing changelog entries.</param>
    /// <param name="Version">The <see cref="SemanticVersion"/> of the changelog (release version).</param>
    public readonly partial record struct Changelog(
        IEnumerable<ChangelogEntry> Entries,
        SemanticVersion Version)
    {
        /// <summary>
        /// Gets commit entries grouped by type
        /// </summary>
        public IReadOnlyDictionary<CommitType, IEnumerable<ChangelogEntry>> EntriesByType =>
            Entries.GroupBy(x => x.Commit.Type)
                .ToDictionary(
                    x => x.Key, 
                    x => x.AsEnumerable());
    }
}
