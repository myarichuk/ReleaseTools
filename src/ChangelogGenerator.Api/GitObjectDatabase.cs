using System.Runtime.CompilerServices;
using LibGit2Sharp;

namespace ChangelogGenerator.Api;

public abstract class GitObjectDatabase<TObject> : IDisposable
{
    private bool _isDisposed;
    protected readonly Repository Repository;

    protected GitObjectDatabase(string repositoryPath)
    {
        ThrowIfInvalidRepo(repositoryPath);

        Repository = new Repository(repositoryPath);
        AssertRepositoryInGoodState();
    }

    protected GitObjectDatabase(string repositoryPath, string username, string email)
    {
        ThrowIfInvalidRepo(repositoryPath);

        Repository = new Repository(
            repositoryPath,
            new RepositoryOptions
            {
                Identity = new Identity(username, email)
            });
        AssertRepositoryInGoodState();
    }

    public abstract IEnumerable<TObject> Query();

    private void AssertRepositoryInGoodState()
    {
        if (Repository.Info.CurrentOperation != CurrentOperation.None)
        {
            throw new RepositoryNotReadyException($"Repository is in the middle of a pending interactive operation. Cannot continue. (operation type = {Repository.Info.CurrentOperation})");
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void ThrowIfInvalidRepo(string repositoryPath)
    {
        if (!Repository.IsValid(repositoryPath))
        {
            throw new ArgumentException($"The path '{repositoryPath}' is not a valid git repository");
        }
    }

    private void DisposeThis()
    {
        if (!_isDisposed)
        {
            _isDisposed = true;
            Repository.Dispose();
        }
    }

    public void Dispose()
    {
        DisposeThis();
        GC.SuppressFinalize(this);
    }

    ~GitObjectDatabase() => Dispose();
}