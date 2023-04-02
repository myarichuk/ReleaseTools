// ReSharper disable MemberCanBePrivate.Global
namespace ChangelogGenerator.Api;

/// <summary>
/// Represents a custom exception class for malformed commit messages.
/// </summary>
public class MalformedCommitMessageException : Exception
{
    /// <summary>
    /// Gets the malformed commit message.
    /// </summary>
    public string CommitMessage { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MalformedCommitMessageException"/> class with the specified malformed commit message.
    /// </summary>
    /// <param name="commitMessage">The malformed commit message.</param>
    public MalformedCommitMessageException(string commitMessage): base($"Failed to parse a conventional commit message. The message is: {commitMessage}")
    {
        CommitMessage = commitMessage;
    }
}