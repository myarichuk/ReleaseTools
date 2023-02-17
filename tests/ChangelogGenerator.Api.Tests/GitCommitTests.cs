using FluentAssertions;
using Xunit.Abstractions;

namespace ChangelogGenerator.Api.Tests
{
    public class GitCommitTests: BaseGitObjectTests
    {
        public GitCommitTests(ITestOutputHelper log) : base(log)
        {
        }

        [Fact(DisplayName = "Can fetch specific commit from Query() method")]
        public void Can_fetch_specific_commit()
        {
            using var commitDb = new CommitDatabase(GitRepositoryFolder);

            for (int i = 1; i < 6; i++)
            {
                CommitDummyFile($"foobar{i}.txt");
            }

            var commitInfoByMessage = commitDb.Query().FirstOrDefault(c => c.Message == "add file foobar3.txt\n");
            commitInfoByMessage.Should().NotBeNull();

            var commitInfoBySha = commitDb.Query().FirstOrDefault(c => c.Tree.Sha == commitInfoByMessage!.Tree.Sha);
            commitInfoBySha.Should().NotBeNull();
        }

        [Fact(DisplayName = "Can fetch all commits")]
        public void Can_fetch_commits()
        {
            using var commitDb = new CommitDatabase(GitRepositoryFolder);

            for (int i = 1; i < 6; i++)
            {
                CommitDummyFile($"foobar{i}.txt");
            }
            
            var commitsQueryResult = commitDb.Query().ToArray();

            var commitMessages = commitsQueryResult.Select(x => x.Message);
            commitMessages.Should()
                .ContainInOrder(
                    "add file foobar1.txt\n",
                    "add file foobar2.txt\n",
                    "add file foobar3.txt\n",
                    "add file foobar4.txt\n",
                    "add file foobar5.txt\n");
        }

        [Fact(DisplayName = "Can fetch commits bounded by two commits (reverse sorting)")]
        public void Can_fetch_bounded_commits_reverse()
        {
            using var commitDb = new CommitDatabase(GitRepositoryFolder);

            for (int i = 1; i < 6; i++)
            {
                CommitDummyFile($"foobar{i}.txt");
            }

            var commitInfoFrom = commitDb.Query().FirstOrDefault(c => c.Message == "add file foobar4.txt\n");
            commitInfoFrom.Should().NotBeNull(); //sanity check

            var commitInfoTo = commitDb.Query().FirstOrDefault(c => c.Message == "add file foobar2.txt\n");
            commitInfoTo.Should().NotBeNull(); //sanity check

            var boundedCommitsQueryResult = commitDb.Query(
                    commitInfoFrom!,
                    commitInfoTo!,
                    CommitDatabase.Sorting.OldestFirst)
                .ToArray();

            var commitMessages = boundedCommitsQueryResult.Select(x => x.Message);
            commitMessages.Should()
                .ContainInOrder(
                    "add file foobar4.txt\n",
                    "add file foobar3.txt\n",
                    "add file foobar2.txt\n");
        }


        [Fact(DisplayName = "Can fetch commits bounded by two commits")]
        public void Can_fetch_bounded_commits()
        {
            using var commitDb = new CommitDatabase(GitRepositoryFolder);

            for (int i = 1; i < 6; i++)
            {
                CommitDummyFile($"foobar{i}.txt");
            }

            var commitInfoFrom = commitDb.Query().FirstOrDefault(c => c.Message == "add file foobar4.txt\n");
            commitInfoFrom.Should().NotBeNull(); //sanity check

            var commitInfoTo = commitDb.Query().FirstOrDefault(c => c.Message == "add file foobar2.txt\n");
            commitInfoTo.Should().NotBeNull(); //sanity check

            var boundedCommitsQueryResult = commitDb.Query(commitInfoFrom!, commitInfoTo!).ToArray();

            var commitMessages = boundedCommitsQueryResult.Select(x => x.Message);
            commitMessages.Should()
                .ContainInOrder(
                    "add file foobar2.txt\n",
                    "add file foobar3.txt\n",
                    "add file foobar4.txt\n");
        }

        [Fact(DisplayName = "Can fetch commits starting from specific sha")]
        public void Can_fetch_commits_starting_from_sha()
        {
            using var commitDb = new CommitDatabase(GitRepositoryFolder);

            for (int i = 1; i < 6; i++)
            {
                CommitDummyFile($"foobar{i}.txt");
            }

            var commitInfo = commitDb.Query().FirstOrDefault(c => c.Message == "add file foobar3.txt\n");
            commitInfo.Should().NotBeNull(); //sanity check

            var boundedCommitsQueryResults = commitDb.Query(commitInfo!);
            
            var commitMessages = boundedCommitsQueryResults.Select(x => x.Message);
            commitMessages.Should()
                .ContainInOrder(
                    "add file foobar1.txt\n",
                    "add file foobar2.txt\n",
                    "add file foobar3.txt\n");

            commitInfo = commitDb.Query().FirstOrDefault(c => c.Message == "add file foobar2.txt\n");
            commitInfo.Should().NotBeNull(); 

            boundedCommitsQueryResults = commitDb.Query(commitInfo!);
            commitMessages = boundedCommitsQueryResults.Select(x => x.Message);
            commitMessages.Should()
                .ContainInOrder(
                    "add file foobar1.txt\n",
                    "add file foobar2.txt\n");
        }
    }
}