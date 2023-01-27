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

        [Fact(DisplayName = "Should fail parsing with type not mentioned in the spec")]
        public void Should_fail_parse_non_spec_type()
        {
            const string msg = "foobar: issue description";
            var isParsingSuccessful = ConventionalCommit.TryParse(msg, out var parsedCommitMessage);
            isParsingSuccessful.Should().BeFalse("Calling ConventionalCommit::TryParse() on a message with non-spec type should return false");

        }

        [Fact(DisplayName = "Can parse multiline body in a commit message")]
        public void Can_parse_body_section()
        {
            const string msg = 
                @"fix(parser):issue description


                this is line #1
                 and this is line #2
                ";

            var isParsingSuccessful = ConventionalCommit.TryParse(msg, out var parsedCommitMessage, out var syntaxErrors);

            syntaxErrors.Should().BeNull();
            isParsingSuccessful.Should().BeTrue("the message being parsed is a valid one");
            parsedCommitMessage.Should().NotBeNull("successful parsing should not return null");

            parsedCommitMessage?.Type.Should().Be(CommitType.Fix);
            parsedCommitMessage?.Scope.Should().Be("parser");
            parsedCommitMessage?.Description.Should().Be("issue description");

            parsedCommitMessage.Body.Should().Be($"this is line #1{Environment.NewLine}and this is line #2");
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
