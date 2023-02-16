using System.Runtime.CompilerServices;
using LibGit2Sharp;

namespace ChangelogGenerator.Api;

public abstract class GitObjectDatabase<TObject> : IDisposable
{
    private bool _isDisposed;
    protected readonly Repository GitRepository;

    protected GitObjectDatabase(string repositoryPath)
    {
        ThrowIfInvalidRepo(repositoryPath);

        GitRepository = new Repository(repositoryPath);
        AssertRepositoryInGoodState();
    }

    protected GitObjectDatabase(string repositoryPath, string username, string email)
    {
        ThrowIfInvalidRepo(repositoryPath);

        GitRepository = new Repository(
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
        if (GitRepository.Info.CurrentOperation != CurrentOperation.None)
        {
            throw new RepositoryNotReadyException($"Repository is in the middle of a pending interactive operation. Cannot continue. (operation type = {GitRepository.Info.CurrentOperation})");
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
            GitRepository.Dispose();
        }
    }

    public void Dispose()
    {
        DisposeThis();
        GC.SuppressFinalize(this);
    }

    ~GitObjectDatabase() => Dispose();
}