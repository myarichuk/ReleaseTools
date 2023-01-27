namespace ChangelogGenerator.Api;

public class RepositoryNotReadyException : Exception
{
    public RepositoryNotReadyException(string? message) : base(message)
    {
    }

    public RepositoryNotReadyException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}