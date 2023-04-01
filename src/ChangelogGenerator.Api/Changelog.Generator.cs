using ChangelogGenerator.Api.Repositories;
using LibGit2Sharp;
using Parser.ConventionalCommit;

namespace ChangelogGenerator.Api
{
    public partial record struct Changelog
    {
        public static IEnumerable<ChangelogEntry> GenerateEntries(
            string gitRepositoryFolder,
            string? toSha = null,
            string? fromSha = null)
        {
            var commitDb = new CommitRepository(new Repository(gitRepositoryFolder));
            List<Commit> commitsToProcess;
            if (fromSha != null && toSha != null)
            {
                commitsToProcess = commitDb.Query(new QueryParams(fromSha, toSha)).ToList();
            }
            else if (fromSha != null && toSha == null)
            {
                commitsToProcess = commitDb.Query(new QueryParams(fromSha, null)).ToList();
            }
            else if(fromSha == null && toSha != null)
            {
                commitsToProcess = commitDb.Query(new QueryParams(null, toSha)).ToList();
            }
            else
            {
                commitsToProcess = commitDb.Query().ToList();
            }

            foreach (var commit in commitsToProcess)
            {
                if (!ConventionalCommit.TryParse(commit.Message, out var parsedCommitMessage))
                {
                    throw new MalformedCommitMessageException(commit.Message);
                }

                yield return new ChangelogEntry(parsedCommitMessage, commit.Sha, Author.From(commit.Author));
            }
        }
    }
}
