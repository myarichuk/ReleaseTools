using FluentAssertions;
using Parser.ConventionalCommit;
using Xunit.Abstractions;

namespace ParserTests
{
    public class ConventionalCommit
    {
        private readonly ITestOutputHelper _logger;

        public ConventionalCommit(ITestOutputHelper logger) => 
            _logger = logger;

        #region Test Data

        #region Multiline Body (without Footer section) Messages
        private static readonly string CommitMessageWithBody =
            @$"fix(parser):issue description{Environment.NewLine}{Environment.NewLine}this is line #1{Environment.NewLine}and this is line #2{Environment.NewLine}and this is line #3";

        private static readonly string CommitMessageWithBodyWhitespaces1 =
            @$"fix(parser):issue description  {Environment.NewLine}{Environment.NewLine}  this is line #1{Environment.NewLine}and this is line #2{Environment.NewLine}and this is line #3";

        private static readonly string CommitMessageWithBodyWhitespaces2 =
            @$"fix(parser):issue description {Environment.NewLine}{Environment.NewLine}  this is line #1   {Environment.NewLine}  and this is line #2{Environment.NewLine}and this is line #3";

        private static readonly string CommitMessageWithBodyWhitespaces3 =
            @$"fix(parser):issue description  {Environment.NewLine}{Environment.NewLine}  this is line #1   {Environment.NewLine}  and this is line #2  {Environment.NewLine} and this is line #3";

        private static readonly string CommitMessageWithBodyWhitespacesIntermixed =
            @$"fix(parser):issue description   {Environment.NewLine}   {Environment.NewLine}  this is line #1   {Environment.NewLine}  and this is line #2  {Environment.NewLine} and this is line #3";

        private static readonly string CommitMessageWithBodyNewlinesAtTheEnd =
            @$"fix(parser):issue description {Environment.NewLine}{Environment.NewLine}  this is line #1   {Environment.NewLine}  and this is line #2  {Environment.NewLine} and this is line #3 {Environment.NewLine}{Environment.NewLine}";

        private static readonly string CommitMessageWithBodyAndFooter =
            @$"fix(parser):issue description  {Environment.NewLine}{Environment.NewLine}this is line #1{Environment.NewLine}and this is line #2{Environment.NewLine}and this is line #3{Environment.NewLine}{Environment.NewLine}foo: bar{Environment.NewLine}bar: foo";


        public static IEnumerable<object[]> MultilineBodyCommitMessages
        {
            get
            {
                yield return new[] { CommitMessageWithBody };
                yield return new[] { CommitMessageWithBodyWhitespaces1 };
                yield return new[] { CommitMessageWithBodyWhitespaces2 };
                yield return new[] { CommitMessageWithBodyWhitespaces3 };
                yield return new[] { CommitMessageWithBodyNewlinesAtTheEnd };
                yield return new[] { CommitMessageWithBodyWhitespacesIntermixed };
            }
        }
        #endregion

        #endregion

        [Fact(DisplayName = "Can parse minimal message")]
        public void Can_parse_minimal()
        {
            const string msg = "fix: issue description";
            var isParsingSuccessful = Parser.ConventionalCommit.ConventionalCommit.TryParse(msg, out var parsedCommitMessage);

            isParsingSuccessful.Should().BeTrue("the message being parsed is a valid one");
            parsedCommitMessage.Should().NotBeNull("successful parsing should not return null");

            parsedCommitMessage.Type.Should().Be(CommitType.Fix);
            parsedCommitMessage.Description.Should().Be("issue description");
        }

        [Fact(DisplayName = "Should correctly parse type not mentioned in spec")]
        public void Should_correctly_parse_non_spec_type()
        {
            const string msg = "foobar: issue description";
            var isParsingSuccessful = Parser.ConventionalCommit.ConventionalCommit.TryParse(msg, out var parsedCommitMessage);
            isParsingSuccessful.Should().BeTrue();

            parsedCommitMessage!.Type.Should().Be(CommitType.Other);
            parsedCommitMessage.TypeAsString.Should().Be("foobar");
        }

        [Theory(DisplayName = "Can parse multiline body in a commit message")]
        [MemberData(nameof(MultilineBodyCommitMessages))]
        public void Can_parse_body_section(string msg)
        {
            _logger.WriteLine($"Testing parsing of: {msg}");
            var isParsingSuccessful = Parser.ConventionalCommit.ConventionalCommit.TryParse(msg, out var parsedCommitMessage, out var syntaxErrors);

            syntaxErrors.Should().BeNull();
            isParsingSuccessful.Should().BeTrue("the message being parsed is a valid one");
            parsedCommitMessage.Should().NotBeNull("successful parsing should not return null");

            parsedCommitMessage.TypeAsString.Should().Be("fix");

            parsedCommitMessage.Type.Should().Be(CommitType.Fix);
            parsedCommitMessage.Scope.Should().Be("parser");
            parsedCommitMessage.Description.Should().Be("issue description");

            parsedCommitMessage.Body.Should()
                .Contain("this is line #1")
                .And.Contain("and this is line #2")
                .And.Contain("and this is line #3");
        }

        [Theory(DisplayName = "Can detect breaking change")]
        [InlineData("fix(parser)!: issue description")]
        [InlineData(@"fix(parser):issue description

                foo1: bar1
                BREAKING-CHANGE: explanation why is there a breaking change
                ")]
        [InlineData(@"fix(parser):issue description



                foo1: bar1
                BREAKING-CHANGE: explanation why is there a breaking change
                ")]
        [InlineData(@"fix(parser):issue description

                foo1: bar1
                breaking-change: explanation why is there a breaking change
                ")]
        public void Can_detect_breaking_change_flag(string msg)
        {
            var isParsingSuccessful = Parser.ConventionalCommit.ConventionalCommit.TryParse(msg, out var parsedCommitMessage, out var syntaxErrors);

            syntaxErrors.Should().BeNull();
            isParsingSuccessful.Should().BeTrue("the message being parsed is a valid one");
            parsedCommitMessage.Should().NotBeNull("successful parsing should not return null");

            if (parsedCommitMessage!.Footer.Count > 0)
            {
                parsedCommitMessage.Footer.Should().HaveCount(2);
            }

            parsedCommitMessage.IsBreaking.Should().BeTrue();
        }

        [Theory(DisplayName = "Can parse footer section after body in a commit message")]
        [InlineData(@"fix(parser):issue description

                this is line #1
                 and this is line #2

                foo1: bar1
                foo2: bar2
                ")]
        [InlineData(@"fix(parser):issue description


                this is line #1
                 and this is line #2


                foo1: bar1
                foo2: bar2

                ")]
        [InlineData(@"fix(parser):issue description




                this is line #1
                    and this is line #2




                foo1: bar1
                foo2: bar2               
                ")]
        public void Can_parse_body_and_footer_section(string msg)
        {
            var isParsingSuccessful = Parser.ConventionalCommit.ConventionalCommit.TryParse(msg, out var parsedCommitMessage, out var syntaxErrors);

            syntaxErrors.Should().BeNull();
            isParsingSuccessful.Should().BeTrue("the message being parsed is a valid one");
            parsedCommitMessage.Should().NotBeNull("successful parsing should not return null");

            parsedCommitMessage.Type.Should().Be(CommitType.Fix);
            parsedCommitMessage.Scope.Should().Be("parser");
            parsedCommitMessage.Description.Should().Be("issue description");

            parsedCommitMessage.Body.Should()
                .Contain($"this is line #1")
                .And.Contain("and this is line #2");

            parsedCommitMessage.Footer.Should().NotBeEmpty();
        }

        [Theory(DisplayName = "Can parse message with scope")]
        [InlineData("fix(parser):issue description")]
        [InlineData("  fix  (parser):  issue description   ")]
        [InlineData("fix(parser ) :issue description")]
        [InlineData("fix (  parser) : issue description")]
        [InlineData("  fix (  parser) :       issue description  ")]
        public void Can_parse_full_without_body_and_footer(string msg)
        {
            var isParsingSuccessful = Parser.ConventionalCommit.ConventionalCommit.TryParse(msg, out var parsedCommitMessage);

            isParsingSuccessful.Should().BeTrue("the message being parsed is a valid one");
            parsedCommitMessage.Should().NotBeNull("successful parsing should not return null");

            parsedCommitMessage.Type.Should().Be(CommitType.Fix);
            parsedCommitMessage.Scope.Should().Be("parser");
            parsedCommitMessage.Description.Should().Be("issue description");
        }
    }
}
