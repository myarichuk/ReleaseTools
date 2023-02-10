using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Parser.ConventionalCommit.ErrorListeners;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable OutParameterValueIsAlwaysDiscarded.Global

namespace Parser.ConventionalCommit
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public partial class ConventionalCommit
    {
        private static readonly ThreadLocal<SyntaxErrorListener> SyntaxErrorListener =
            new(() => new ());

        private static readonly ThreadLocal<ConventionalCommitParsingListener> ParsingWalker =
            new(() => new ());

#if DEBUG
        private static readonly ThreadLocal<ConsoleOutputErrorListener> ConsoleOutputErrorListener = 
            new(() => new());
#endif

        /// <summary>
        /// Try parsing provided conventional commit message
        /// </summary>
        /// <param name="commitMessage">the commit message</param>
        /// <param name="parsedCommitMessage">structure representing the parsed commit message</param>
        /// <returns>true if parsing successful, false otherwise</returns>
        public static bool TryParse(string commitMessage, out ConventionalCommit? parsedCommitMessage) =>
            TryParse(commitMessage, out parsedCommitMessage, out _);

        /// <summary>
        /// Try parsing provided conventional commit message
        /// </summary>
        /// <param name="commitMessage">the commit message</param>
        /// <param name="parsedCommitMessage">structure representing the parsed commit message</param>
        /// <param name="syntaxErrors">list of syntax errors if the parsing has failed</param>
        /// <returns>true if parsing successful, false otherwise</returns>
        public static bool TryParse(string commitMessage, out ConventionalCommit? parsedCommitMessage, out IReadOnlyList<SyntaxErrorInfo>? syntaxErrors)
        {
            parsedCommitMessage = null;
            var lexer = new ConventionalCommitLexer(new AntlrInputStream(commitMessage));//CachedLexer.Value;
            var parser = new ConventionalCommitParser(new CommonTokenStream(lexer));//CachedParser.Value;

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
                ////note: looking at tokens like this is sometimes useful for debugging
                // var tokens = lexer.GetAllTokens().Select(x => (x.Type, x.Text)).ToArray();
                // lexer.Reset();


                parser.RemoveErrorListeners();
                parser.AddErrorListener(syntaxErrorListener);
#if DEBUG
                parser.AddErrorListener(consoleOutputErrorListener);
#endif

                var ast = parser.commitMessage();
                
                
                ParseTreeWalker.Default.Walk(listener, ast);

                parsedCommitMessage = listener.ParsedMessage;
                
                if (syntaxErrorListener.Errors.Count > 0)
                {
                    syntaxErrors = syntaxErrorListener.Errors.ToArray();
                    parsedCommitMessage = null;
                }

                return syntaxErrorListener.Errors.Count == 0;
            }
            catch (Exception e)
            {
                throw new ParsingFailedException($"Failed to parse provided input <<{commitMessage}>>", e);
            }
            finally
            {
                listener.Reset();
            }
        }

    }
}
