using ChangelogGenerator.Api.Repositories;
using FluentAssertions;
using LibGit2Sharp;
using Xunit.Abstractions;

namespace ChangelogGenerator.Api.Tests
{
    public class CommitRepository: BaseTestWithGitRepository
    {
        public CommitRepository(ITestOutputHelper log) : base(log)
        {
        }

        [Fact(DisplayName = "Can fetch specific commit from Query() method")]
        public void Can_fetch_specific_commit()
        {
            var commitDb = new Repositories.CommitRepository(new Repository(GitRepositoryFolder));

            for (int i = 1; i < 6; i++)
            {
                CommitDummyFile($"foobar{i}.txt");
            }

            var commitInfoByMessage = commitDb.Query().FirstOrDefault(c => c.Message == "chore: add file foobar3.txt\n");
            commitInfoByMessage.Should().NotBeNull();

            var commitInfoBySha = commitDb.Query().FirstOrDefault(c => c.Tree.Sha == commitInfoByMessage!.Tree.Sha);
            commitInfoBySha.Should().NotBeNull();
        }

        [Fact(DisplayName = "Can fetch all commits")]
        public void Can_fetch_commits()
        {
            var commitDb = new Repositories.CommitRepository(new Repository(GitRepositoryFolder));

            for (int i = 1; i < 6; i++)
            {
                CommitDummyFile($"foobar{i}.txt");
            }
            
            var commitsQueryResult = commitDb.Query().ToArray();

            var commitMessages = commitsQueryResult.Select(x => x.Message);
            commitMessages.Should()
                .ContainInOrder(
                    "chore: add file foobar1.txt\n",
                    "chore: add file foobar2.txt\n",
                    "chore: add file foobar3.txt\n",
                    "chore: add file foobar4.txt\n",
                    "chore: add file foobar5.txt\n");
        }

        [Fact(DisplayName = "Can fetch commits bounded by two commits (reverse sorting)")]
        public void Can_fetch_bounded_commits_reverse()
        {
            var commitDb = new Repositories.CommitRepository(new Repository(GitRepositoryFolder));

            for (int i = 1; i < 6; i++)
            {
                CommitDummyFile($"foobar{i}.txt");
            }

            var commitInfoFrom = commitDb.Query().FirstOrDefault(c => c.Message == "chore: add file foobar4.txt\n");
            commitInfoFrom.Should().NotBeNull(); //sanity check

            var commitInfoTo = commitDb.Query().FirstOrDefault(c => c.Message == "chore: add file foobar2.txt\n");
            commitInfoTo.Should().NotBeNull(); //sanity check

            var boundedCommitsQueryResult = commitDb.Query(
                    new QueryParams(commitInfoFrom?.Sha,
                        commitInfoTo?.Sha,
                        ResultSorting.OldestFirst))
                .ToArray();

            var commitMessages = boundedCommitsQueryResult.Select(x => x.Message);
            commitMessages.Should()
                .ContainInOrder(
                    "chore: add file foobar4.txt\n",
                    "chore: add file foobar3.txt\n",
                    "chore: add file foobar2.txt\n");
        }

        [Fact(DisplayName = "Can fetch commits bounded by two commits (reverse sorting) -> using SHAs")]
        public void Can_fetch_bounded_commits_reverse_by_sha()
        {
            var commitDb = new Repositories.CommitRepository(new Repository(GitRepositoryFolder));

            for (int i = 1; i < 6; i++)
            {
                CommitDummyFile($"foobar{i}.txt");
            }

            var commitInfoFrom = commitDb.Query().FirstOrDefault(c => c.Message == "chore: add file foobar4.txt\n");
            commitInfoFrom.Should().NotBeNull(); //sanity check

            var commitInfoTo = commitDb.Query().FirstOrDefault(c => c.Message == "chore: add file foobar2.txt\n");
            commitInfoTo.Should().NotBeNull(); //sanity check

            var boundedCommitsQueryResult = commitDb.Query(
                    new QueryParams(
                        commitInfoFrom!.Sha,
                    commitInfoTo!.Sha,
                    ResultSorting.OldestFirst))
                .ToArray();

            var commitMessages = boundedCommitsQueryResult.Select(x => x.Message);
            commitMessages.Should()
                .ContainInOrder(
                    "chore: add file foobar4.txt\n",
                    "chore: add file foobar3.txt\n",
                    "chore: add file foobar2.txt\n");
        }

        [Fact(DisplayName = "Can fetch commits bounded by two commits")]
        public void Can_fetch_bounded_commits()
        {
            var commitDb = new Repositories.CommitRepository(new Repository(GitRepositoryFolder));

            for (int i = 1; i < 6; i++)
            {
                CommitDummyFile($"foobar{i}.txt");
            }

            var commitInfoFrom = commitDb.Query().FirstOrDefault(c => c.Message == "chore: add file foobar4.txt\n");
            commitInfoFrom.Should().NotBeNull(); //sanity check

            var commitInfoTo = commitDb.Query().FirstOrDefault(c => c.Message == "chore: add file foobar2.txt\n");
            commitInfoTo.Should().NotBeNull(); //sanity check

            var boundedCommitsQueryResult = commitDb.Query(
                    new QueryParams(
                        commitInfoFrom?.Sha,
                    commitInfoTo?.Sha))
                .ToArray();

            var commitMessages = boundedCommitsQueryResult.Select(x => x.Message);
            commitMessages.Should()
                .ContainInOrder(
                    "chore: add file foobar2.txt\n",
                    "chore: add file foobar3.txt\n",
                    "chore: add file foobar4.txt\n");
        }

        [Fact(DisplayName = "Can fetch commits starting from specific sha")]
        public void Can_fetch_commits_starting_from_sha()
        {
            var commitDb = new Repositories.CommitRepository(new Repository(GitRepositoryFolder));

            for (int i = 1; i < 6; i++)
            {
                CommitDummyFile($"foobar{i}.txt");
            }

            var commitInfo = commitDb.Query().FirstOrDefault(c => c.Message == "chore: add file foobar3.txt\n");
            commitInfo.Should().NotBeNull(); //sanity check

            var boundedCommitsQueryResults = commitDb.Query(new QueryParams(commitInfo?.Sha, null));
            
            var commitMessages = boundedCommitsQueryResults.Select(x => x.Message);
            commitMessages.Should()
                .ContainInOrder(
                    "chore: add file foobar1.txt\n",
                    "chore: add file foobar2.txt\n",
                    "chore: add file foobar3.txt\n");

            commitInfo = commitDb.Query().FirstOrDefault(c => c.Message == "chore: add file foobar2.txt\n");
            commitInfo.Should().NotBeNull(); 

            boundedCommitsQueryResults = commitDb.Query(new QueryParams(commitInfo?.Sha, null));
            commitMessages = boundedCommitsQueryResults.Select(x => x.Message);
            commitMessages.Should()
                .ContainInOrder(
                    "chore: add file foobar1.txt\n",
                    "chore: add file foobar2.txt\n");
        }
    }
}