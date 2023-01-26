using System.Threading;
using Antlr4.Runtime;

namespace ConventionalCommitsParser
{
    public partial class ConventionalCommit
    {
        private static readonly ThreadLocal<ConventionalCommitLexer> CachedLexer = new(() => new ConventionalCommitLexer(null));
        private static readonly ThreadLocal<ConventionalCommitParser> CachedParser = new(() => new ConventionalCommitParser(null));

        public static bool TryParse(string commitMessage, out ConventionalCommit? parsedCommitMessage)
        {
            parsedCommitMessage = null;
            var lexer  = CachedLexer.Value;
            var parser = CachedParser.Value;

            lexer!.SetInputStream(new AntlrInputStream(commitMessage));
            parser!.SetInputStream(new CommonTokenStream(lexer));

            var ast = parser.commitMessage();

            //TODO: add visitor implementation to transform AST to ConventionalCommit class

            return false;
        }

    }
}
