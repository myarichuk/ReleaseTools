using Antlr4.Runtime.Misc;
using Antlr4.Runtime;
using System;

namespace ConventionalCommitsParser
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
            var input = ((ICharStream)recognizer.InputStream)
                .GetText(new Interval(0, offendingSymbol.StopIndex));

            Console.WriteLine($"Error at line {line}, position {charPositionInLine}: {msg}");
            Console.WriteLine($"Offending text: {input.Substring(offendingSymbol.StartIndex, offendingSymbol.StopIndex - offendingSymbol.StartIndex + 1)}");
        }
    }

}
