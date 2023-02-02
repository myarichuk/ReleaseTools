using System;

namespace Parser.SemanticVersion
{
    /// <summary>
    /// A class of an exception that will be thrown when an unhandled error occurs.
    /// </summary>
    public class ParsingFailedException : Exception
    {
        public ParsingFailedException(string message, Exception exception): base(message, exception)
        {
        }
    }
}