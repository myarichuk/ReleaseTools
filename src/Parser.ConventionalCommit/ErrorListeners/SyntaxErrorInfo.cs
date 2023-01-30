using Antlr4.Runtime;

namespace Parser.ConventionalCommit.ErrorListeners
{
    /// <summary>
    /// An object that holds information about syntax error encountered during parsing
    /// </summary>
    public record SyntaxErrorInfo(
        IRecognizer Recognizer,
        IToken OffendingSymbol,
        int Line,
        int CharPositionInLine,
        string ErrorMsg,
        RecognitionException RecognitionException)
    {
        public IRecognizer Recognizer { get; } = Recognizer;
        public IToken OffendingSymbol { get; } = OffendingSymbol;
        public int Line { get; } = Line;
        public int CharPositionInLine { get; } = CharPositionInLine;
        public string ErrorMsg { get; } = ErrorMsg;
        public RecognitionException RecognitionException { get; } = RecognitionException;
    }
}