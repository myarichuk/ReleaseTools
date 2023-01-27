using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace ConventionalCommitsParser
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public partial class ConventionalCommit
    {
        private static readonly ThreadLocal<ConventionalCommitLexer> CachedLexer = new(() => new ConventionalCommitLexer(null));
        private static readonly ThreadLocal<ConventionalCommitParser> CachedParser = new(() => new ConventionalCommitParser(null));

        public static bool TryParse(string commitMessage, out ConventionalCommit? parsedCommitMessage)
        {
            parsedCommitMessage = null;
            var lexer = CachedLexer.Value;
            var parser = CachedParser.Value;

            try
            {

                lexer!.SetInputStream(new AntlrInputStream(commitMessage));
                parser!.SetInputStream(new CommonTokenStream(lexer));

                var ast = parser.commitMessage();

                var listener = new ConventionalCommitParsingListener();
                ParseTreeWalker.Default.Walk(listener, ast);

                parsedCommitMessage = listener.ParsedMessage;

                return parser.NumberOfSyntaxErrors == 0;
            }
            finally
            {
                lexer.Reset();
                parser.Reset();
            }
        }

    }
}
