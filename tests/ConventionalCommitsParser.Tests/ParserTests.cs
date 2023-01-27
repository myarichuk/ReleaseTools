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

        [Theory(DisplayName = "Can parse message with scope")]
        [InlineData("fix(parser):issue description")]
        [InlineData("  fix  (parser):  issue description   ")]
        [InlineData("fix(parser ) :issue description")]
        [InlineData("fix (  parser) : issue description")]
        [InlineData("  fix (  parser) :       issue description  ")]
        public void Can_parse_full_without_body_and_footer(string msg)
        {
            var isParsingSuccessful = ConventionalCommit.TryParse(msg, out var parsedCommitMessage);

            isParsingSuccessful.Should().BeTrue("the message being parsed is a valid one");
            parsedCommitMessage.Should().NotBeNull("successful parsing should not return null");

            parsedCommitMessage?.Type.Should().Be(CommitType.Fix);
            parsedCommitMessage?.Scope.Should().Be("parser");
            parsedCommitMessage?.Description.Should().Be("issue description");
        }

        [Theory(DisplayName = "Should fail when message is malformed")]
        [InlineData("fix(parser!: issue description")]
        [InlineData("fix parser): issue description")]
        [InlineData("fix!(parser)! issue description")]
        [InlineData("fix parser   : issue description")]
        public void Can_fail_incomplete_input(string msg)
        {
            var isParsingSuccessful = ConventionalCommit.TryParse(msg, out var parsedCommitMessage);

            isParsingSuccessful.Should().BeFalse("Calling ConventionalCommit::TryParse() on a malformed message should return false");
            parsedCommitMessage.Should().BeNull("Calling ConventionalCommit::TryParse() on a malformed message return null message");
        }
    }
}
