using Parser.ConventionalCommit;

namespace ChangelogGenerator.Api
{
    public record struct ChangelogEntry(string Sha, string Author, in ConventionalCommit Message)
    {
    }
}
