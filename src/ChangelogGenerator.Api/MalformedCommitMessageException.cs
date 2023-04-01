// ReSharper disable MemberCanBePrivate.Global
namespace ChangelogGenerator.Api;

public class MalformedCommitMessageException : Exception
{
    public string CommitMessage { get; }

    public MalformedCommitMessageException(string commitMessage): base($"Failed to parse a conventional commit message. The message is: {commitMessage}")
    {
        CommitMessage = commitMessage;
    }
}