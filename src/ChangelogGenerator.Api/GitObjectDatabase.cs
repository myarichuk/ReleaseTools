using LibGit2Sharp;

namespace ChangelogGenerator.Api;

public abstract class GitObjectDatabase<TObject> : IDisposable
{
    private bool _isDisposed;
    protected Repository _repository;

    protected GitObjectDatabase(string repositoryPath)
    {
        ThrowIfInvalidRepo(repositoryPath);

        _repository = new Repository(repositoryPath);
        AssertRepositoryInGoodState();
    }

    protected GitObjectDatabase(string repositoryPath, string username, string email)
    {
        ThrowIfInvalidRepo(repositoryPath);

        _repository = new Repository(
            repositoryPath,
            new RepositoryOptions
            {
                Identity = new Identity(username, email)
            });
        AssertRepositoryInGoodState();
    }

    public abstract IEnumerable<TObject> Query();

    protected void AssertRepositoryInGoodState()
    {
        if (_repository.Info.CurrentOperation != CurrentOperation.None)
        {
            throw new RepositoryNotReadyException($"Repository is in the middle of a pending interactive operation. Cannot continue. (operation type = {_repository.Info.CurrentOperation})");
        }
    }

    protected static void ThrowIfInvalidRepo(string repositoryPath)
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
            _repository.Dispose();
        }
    }

    public void Dispose()
    {
        DisposeThis();
        GC.SuppressFinalize(this);
    }

    ~GitObjectDatabase() => Dispose();
}