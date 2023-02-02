using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Parser.SemanticVersion.ErrorListeners;

namespace Parser.SemanticVersion
{
    public partial class SemanticVersion
    {
        private static readonly ThreadLocal<SemanticVersionLexer> CachedLexer = 
            new(() => new(null));

        private static readonly ThreadLocal<SemanticVersionParser> CachedParser = 
            new(() => new(null));

        private static readonly ThreadLocal<SyntaxErrorListener> SyntaxErrorListener =
            new(() => new());

        private static readonly ThreadLocal<SemanticVersionParserListener> ParsingWalker =
            new(() => new());

#if DEBUG
        private static readonly ThreadLocal<ConsoleOutputErrorListener> ConsoleOutputErrorListener =
            new(() => new());
#endif

        public static bool TryParse(string semver, out SemanticVersion parsedSemver, out IReadOnlyList<SyntaxErrorInfo> syntaxErrors)
        {
            parsedSemver = null;
            var lexer = CachedLexer.Value;
            var parser = CachedParser.Value;
            var syntaxErrorListener = SyntaxErrorListener.Value;
            var listener = ParsingWalker.Value;
            listener.Reset();
            
            syntaxErrors = null;
#if DEBUG
            var consoleOutputErrorListener = ConsoleOutputErrorListener.Value;
#endif
            syntaxErrorListener.Reset();
            try
            {
                lexer!.SetInputStream(new AntlrInputStream(semver));
                parser!.SetInputStream(new CommonTokenStream(lexer));

                parser.RemoveErrorListeners();
                parser.AddErrorListener(syntaxErrorListener);
#if DEBUG
                parser.AddErrorListener(consoleOutputErrorListener);
#endif
                var ast = parser.semver();
                ParseTreeWalker.Default.Walk(listener, ast);

                parsedSemver = listener.ParsedSemver;
                
                if (syntaxErrorListener.Errors.Count > 0)
                {
                    syntaxErrors = syntaxErrorListener.Errors.ToArray();
                    parsedSemver = null;
                }

                return syntaxErrorListener.Errors.Count == 0;
            }
            catch (Exception e)
            {
                throw new ParsingFailedException($"Failed to parse provided input <<{semver}>>", e);
            }
            finally
            {
                lexer.Reset();
                parser.Reset();
            }

        }
    }
}
