using System.Collections.Generic;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;

namespace Parser.SemanticVersion.ErrorListeners
{
    internal class SyntaxErrorListener: BaseErrorListener
    {
        private readonly List<SyntaxErrorInfo> _errors = new();

        public IReadOnlyList<SyntaxErrorInfo> Errors => _errors;

        public void Reset() => _errors.Clear();

        public override void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg,
            RecognitionException e)
        {
            var input = ((CommonTokenStream)recognizer.InputStream)
                .GetText(new Interval(0, offendingSymbol.StopIndex));

            _errors.Add(new SyntaxErrorInfo(recognizer, offendingSymbol, line, charPositionInLine, msg, e));
        }
    }
}
