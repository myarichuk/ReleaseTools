using FluentAssertions;
using Parser.ConventionalCommit;
using Xunit.Abstractions;

namespace ChangelogGenerator.Api.Tests;

public class ChangelogGenerateEntries : BaseTestWithGitRepository
{
    public ChangelogGenerateEntries(ITestOutputHelper log) : base(log)
    {
    }

    [Fact]
    public void Can_generate_changelog_for_simple_case()
    {
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

    [Fact]
    public void Can_generate_changelog_for_case_with_breaking_change()
    {
        for (int i = 1; i < 6; i++)
        {
            CommitDummyFile($"foobar{i}.txt", $"feat: add file foobar{i}.txt");
        }

        CommitDummyFile("foobar6.txt", "feat!: add file foobar6.txt");

        var changelogEntries = Changelog.GenerateEntries(GitRepositoryFolder).ToArray();
        changelogEntries.Select(x => x.Commit.Description)
            .Should()
            .ContainInOrder(
                "add file foobar1.txt",
                "add file foobar2.txt",
                "add file foobar3.txt",
                "add file foobar4.txt",
                "add file foobar5.txt",
                "add file foobar6.txt");

        changelogEntries.Should().AllSatisfy(x =>
            x.Commit.Type.Should().Be(CommitType.Feat));
        changelogEntries.Should()
            .ContainSingle(x => x.Commit.Description == "add file foobar6.txt" && x.Commit.IsBreaking);
    }

    [Fact]
    public void Can_generate_changelog_for_case_with_multiple_breaking_changes()
    {
        for (int i = 1; i < 6; i++)
        {
            CommitDummyFile($"foobar{i}.txt", $"feat: add file foobar{i}.txt");
        }

        CommitDummyFile("foobar6.txt", "feat!: add file foobar6.txt");
        CommitDummyFile("foobar7.txt", "feat!: add file foobar7.txt");
        var changelogEntries = Changelog.GenerateEntries(GitRepositoryFolder).ToArray();
        changelogEntries.Select(x => x.Commit.Description)
            .Should()
            .ContainInOrder(
                "add file foobar1.txt",
                "add file foobar2.txt",
                "add file foobar3.txt",
                "add file foobar4.txt",
                "add file foobar5.txt",
                "add file foobar6.txt",
                "add file foobar7.txt");

        changelogEntries.Should().AllSatisfy(x =>
            x.Commit.Type.Should().Be(CommitType.Feat));

        changelogEntries.Should()
            .ContainSingle(x => x.Commit.Description == "add file foobar6.txt" && x.Commit.IsBreaking);
        changelogEntries.Should()
            .ContainSingle(x => x.Commit.Description == "add file foobar7.txt" && x.Commit.IsBreaking);
    }

    [Fact]
    public void Can_generate_changelog_for_case_with_multiple_breaking_changes_and_multiple_commit_types()
    {
        for (int i = 1; i < 6; i++)
        {
            CommitDummyFile($"foobar{i}.txt", $"feat: add file foobar{i}.txt");
        }

        CommitDummyFile("foobar6.txt", "feat!: add file foobar6.txt");
        CommitDummyFile("foobar7.txt", "feat!: add file foobar7.txt");
        CommitDummyFile("foobar8.txt", "fix!: add file foobar8.txt");
        CommitDummyFile("foobar9.txt", "feat!: add file foobar9.txt");

        var changelogEntries = Changelog.GenerateEntries(GitRepositoryFolder).ToArray();

        changelogEntries.Select(x => x.Commit.Description)
            .Should()
            .ContainInOrder(
                "add file foobar1.txt",
                "add file foobar2.txt",
                "add file foobar3.txt",
                "add file foobar4.txt",
                "add file foobar5.txt",
                "add file foobar6.txt",
                "add file foobar7.txt",
                "add file foobar8.txt",
                "add file foobar9.txt");

        changelogEntries.Should().AllSatisfy(x =>
            x.Commit.Type.Should().BeOneOf(CommitType.Feat, CommitType.Fix));

        changelogEntries.Should()
            .ContainSingle(x => 
                x.Commit.Description == "add file foobar6.txt" && x.Commit.IsBreaking);
        changelogEntries.Should()
            .ContainSingle(x => 
                x.Commit.Description == "add file foobar7.txt" && x.Commit.IsBreaking);
        changelogEntries.Should()
            .ContainSingle(x =>
                x.Commit.Description == "add file foobar8.txt" &&
                x.Commit.IsBreaking &&
                x.Commit.Type == CommitType.Fix);
        changelogEntries.Should()
            .ContainSingle(x => 
                x.Commit.Description == "add file foobar9.txt" && x.Commit.IsBreaking);
    }

    //unit test that checks the same generator with "from" and "to" sha filters
    [Fact]
    public void
        Can_generate_changelog_for_case_with_multiple_breaking_changes_and_multiple_commit_types_with_sha_filters()
    {
        for (int i = 1; i < 6; i++)
        {
            CommitDummyFile($"foobar{i}.txt", $"feat: add file foobar{i}.txt");
        }

        var fromSha = CommitDummyFile("foobar6.txt", "feat!: add file foobar6.txt");
        CommitDummyFile("foobar7.txt", "feat!: add file foobar7.txt");
        var toSha = CommitDummyFile("foobar8.txt", "fix!: add file foobar8.txt");
        CommitDummyFile("foobar9.txt", "feat!: add file foobar9.txt");

        var changelogEntries = 
            Changelog.GenerateEntries(GitRepositoryFolder, fromSha, toSha)
                     .ToArray();

        changelogEntries.Select(x => x.Commit.Description)
            .Should()
            .ContainInOrder(
                "add file foobar6.txt",
                "add file foobar7.txt",
                "add file foobar8.txt");

        changelogEntries.Should().AllSatisfy(x =>
            x.Commit.Type.Should().BeOneOf(CommitType.Feat, CommitType.Fix));
        changelogEntries.Should()
            .ContainSingle(x =>
                x.Commit.Description == "add file foobar6.txt" && x.Commit.IsBreaking);
        changelogEntries.Should()
            .ContainSingle(x =>
                x.Commit.Description == "add file foobar7.txt" && x.Commit.IsBreaking);
        changelogEntries.Should()
            .ContainSingle(x =>
                x.Commit.Description == "add file foobar8.txt" &&
                x.Commit.IsBreaking &&
                x.Commit.Type == CommitType.Fix);
    }
}