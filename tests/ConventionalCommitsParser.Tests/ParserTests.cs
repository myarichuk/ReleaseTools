using FluentAssertions;

namespace ConventionalCommitsParser.Tests
{
    public class ParserTests
    {
        [Fact(DisplayName = "Can parse minimal message")]
        public void Can_parse_minimal()
        {
            const string msg = "fix: issue description";
            var isParsingSuccessful = ConventionalCommit.TryParse(msg, out var parsedCommitMessage);

            isParsingSuccessful.Should().BeTrue("the message being parsed is a valid one");
            parsedCommitMessage.Should().NotBeNull("successful parsing should not return null");

            parsedCommitMessage.Type.Should().Be(CommitType.Fix);
            parsedCommitMessage.Description.Should().Be("issue description");
        }
    }
}
