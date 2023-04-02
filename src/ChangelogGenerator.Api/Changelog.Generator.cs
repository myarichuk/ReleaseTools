using ChangelogGenerator.Api.Repositories;
using LibGit2Sharp;
using Parser.ConventionalCommit;

namespace ChangelogGenerator.Api
{
    /// <summary>
    /// Represents a partial record struct that generates changelog entries based on a Git repository.
    /// </summary>
    public partial record struct Changelog
    {
        /// <summary>
        /// Generates a changelog from the Git repository based on the specified parameters.
        /// </summary>
        public static Changelog Generate(
            SemanticVersion version, 
            string gitRepositoryFolder, 
            string? toSha = null,
            string? fromSha = null) =>
            new(GenerateEntries(gitRepositoryFolder, toSha, fromSha), version);

        /// <summary>
        /// Generates a list of changelog entries from the Git repository based on the specified parameters.
        /// </summary>
        /// <param name="gitRepositoryFolder">The path to the Git repository folder.</param>
        /// <param name="toSha">The commit SHA to stop at (optional).</param>
        /// <param name="fromSha">The commit SHA to start from (optional).</param>
        /// <returns>An enumerable list of <see cref="ChangelogEntry"/> objects.</returns>
        /// <exception cref="MalformedCommitMessageException">Thrown when a commit message is not formatted as a conventional commit.</exception>
        internal static IEnumerable<ChangelogEntry> GenerateEntries(
            string gitRepositoryFolder,
            string? toSha = null,
            string? fromSha = null)
        {
            using var commitDb = new CommitRepository(new Repository(gitRepositoryFolder));
            var commitsToProcess = 
                commitDb.Query(new QueryParams(fromSha, toSha))
                    .ToList();

            foreach (var commit in commitsToProcess)
            {
                if (!ConventionalCommit.TryParse(commit.Message, out var parsedCommitMessage))
                {
                    throw new MalformedCommitMessageException(commit.Message);
                }
                
                yield return new ChangelogEntry(
                    parsedCommitMessage,
                    commit.Sha,
                    Author.From(commit.Author),
                    commit.Encoding);
            }
        }
    }

}
