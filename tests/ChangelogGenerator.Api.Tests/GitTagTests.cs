using FluentAssertions;
using Xunit.Abstractions;

namespace ChangelogGenerator.Api.Tests
{
    public class GitTagTests: BaseGitObjectTests
    {
        public GitTagTests(ITestOutputHelper log) : base(log)
        {
        }

        [Fact(DisplayName = "Can fetch tags from a repo")]
        public void Can_fetch_tags()
        {
            using var tagDatabase = new TagDatabase(GitRepository);

            var sha1 = CommitDummyFile("foo1.txt");
            var sha2 = CommitDummyFile("foo2.txt");
            var sha3 = CommitDummyFile("foo3.txt");
            var sha4 = CommitDummyFile("foo4.txt");

            GitRepository.Tags.Add("1", sha1, DefaultSignature, "first tag");
            GitRepository.Tags.Add("3", sha3, DefaultSignature, "third tag");
            GitRepository.Tags.Add("4", sha4, DefaultSignature, "fourth tag");

            var tags = tagDatabase.Query().ToArray();

            tags.Select(x => x.FriendlyName).Should().ContainInOrder("1", "3", "4");
        }

        [Fact(DisplayName = "Can fetch tags between certain SHAs from a repo")]
        public void Can_fetch_tags_between()
        {
            using var tagDatabase = new TagDatabase(GitRepository);

            var shas = new List<string>();

            for (int i = 0; i < 10; i++)
            {
                shas.Add(CommitDummyFile($"foo{i}.txt"));
            }

            GitRepository.Tags.Add("1", shas[1], DefaultSignature, "");
            GitRepository.Tags.Add("3", shas[3], DefaultSignature, "");
            GitRepository.Tags.Add("6", shas[6], DefaultSignature, "");
            GitRepository.Tags.Add("8", shas[8], DefaultSignature, "");

            var tags = tagDatabase.Query().ToArray();

            tags.Select(x => x.FriendlyName).Should().ContainInOrder("1", "3", "4");
        }
    }
}
