using FluentAssertions;
using Parser.ConventionalCommit;
using Xunit.Abstractions;

namespace ChangelogGenerator.Api.Tests
{
    public class ChangelogGenerateEntries: BaseTestWithGitRepository
    {
        public ChangelogGenerateEntries(ITestOutputHelper log) : base(log)
        {
        }

        [Fact]
        public void Can_generate_changelog_for_simple_case()
        {
            //var commitDb = new CommitRepository(new Repository(GitRepositoryFolder));

            for (int i = 1; i < 6; i++)
            {
                CommitDummyFile($"foobar{i}.txt");
            }

            var changelogEntries = Changelog.GenerateEntries(GitRepositoryFolder).ToArray();

            changelogEntries.Select(x => x.Commit.Description)
                .Should()
                .ContainInOrder(
                    "add file foobar1.txt",
                    "add file foobar2.txt",
                    "add file foobar3.txt",
                    "add file foobar4.txt",
                    "add file foobar5.txt");

            changelogEntries.Should().AllSatisfy(x => 
                x.Commit.Type.Should().Be(CommitType.Chore));
        }
    }
}
