using System;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;

namespace ConventionalCommitsParser.ErrorListeners
{
    internal class ConsoleOutputErrorListener : BaseErrorListener
    {
        public override void SyntaxError(
            IRecognizer recognizer,
            IToken offendingSymbol,
            int line,
            int charPositionInLine,
            string msg,
            RecognitionException e)
        {
            Console.WriteLine($"Error at line {line}, position {charPositionInLine}: {msg}");
            Console.WriteLine($"Offending text: {offendingSymbol.Text}");
        }
    }

}
