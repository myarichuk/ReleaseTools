using FluentAssertions;
using Xunit.Abstractions;

namespace ChangelogGenerator.Api.Tests
{
    public class TagRepository: BaseTestWithGitRepository
    {
        private readonly Repositories.TagRepository _tagRepository;
        private readonly List<string> _commitShaSet = new();

        private const int MaxCommits = 10;

        public TagRepository(ITestOutputHelper log) : base(log)
        {
            _tagRepository = new Repositories.TagRepository(GitRepository);

            for (int i = 0; i < MaxCommits; i++)
            {
                var commitSha = CommitDummyFile($"foo{i}.txt");
                _commitShaSet.Add(commitSha);

                GitRepository.Tags.Add(i.ToString(), commitSha, DefaultSignature, $"commit tag #{i}");
            }
        }

        [Fact(DisplayName = "Can fetch tags from a repo")]
        public void Can_fetch_tags()
        {
            var tags = _tagRepository.Query().ToArray();

            tags.Select(x => x.Target.Sha)
                .Should().ContainInOrder(_commitShaSet);
        }

        [Fact(DisplayName = "Can fetch tags between certain SHAs from a repo")]
        public void Can_fetch_tags_between()
        {
            var tags = _tagRepository.Query(_commitShaSet[1], _commitShaSet[MaxCommits - 2]);

            tags.Select(x => x.Target.Sha)
                .Should().ContainInOrder(_commitShaSet.Skip(1).Take(MaxCommits - 3));
        }

        [Fact(DisplayName = "No tags between SHAs should return empty results")]
        public void No_tags_between_SHAs_should_return_empty_results()
        {
            var extraSha1 = CommitDummyFile("foo_a1.txt");
            CommitDummyFile("foo_a2.txt");
            CommitDummyFile("foo_a3.txt");
            var extraSha2 = CommitDummyFile("foo_a4.txt");

            var tags = _tagRepository.Query(extraSha1, extraSha2);

            tags.Should().BeEmpty();
        }
    }
}
