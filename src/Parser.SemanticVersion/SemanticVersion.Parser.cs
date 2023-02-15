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
            var lexer = new SemanticVersionLexer(new AntlrInputStream(semver));
            var parser = new SemanticVersionParser(new CommonTokenStream(lexer));
            var syntaxErrorListener = SyntaxErrorListener.Value;
            var listener = ParsingWalker.Value;
            
            syntaxErrors = null;
#if DEBUG
            var consoleOutputErrorListener = ConsoleOutputErrorListener.Value;
#endif
            syntaxErrorListener.Reset();
            try
            {
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
                listener.Reset();
            }

        }
    }
}
