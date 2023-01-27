using LibGit2Sharp;

namespace ChangelogGenerator.Api;

public class GitCommitDatabase: IDisposable
{
    private bool _isDisposed;
    private readonly Repository _repository;

    public GitCommitDatabase(string repositoryPath)
    {
        ThrowIfInvalidRepo(repositoryPath);

        _repository = new Repository(repositoryPath);
        AssertRepositoryInGoodState();
    }

    public GitCommitDatabase(string repositoryPath, string username, string email)
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

    public IEnumerable<Commit> Query() => 
        _repository.Commits.QueryBy(new CommitFilter());

    public IEnumerable<Commit> Query(string startShaOrTag) => 
        _repository.Commits.QueryBy(new CommitFilter
        {
            IncludeReachableFrom = startShaOrTag
        });

    public IEnumerable<Commit> Query(string startShaOrTag, string endShaOrTag) => 
        _repository.Commits.QueryBy(new CommitFilter
        {
            IncludeReachableFrom = startShaOrTag,
            ExcludeReachableFrom = endShaOrTag
        });

    private void AssertRepositoryInGoodState()
    {
        if (_repository.Info.CurrentOperation != CurrentOperation.None)
        {
            throw new RepositoryNotReadyException($"Repository is in the middle of a pending interactive operation. Cannot continue. (operation type = {_repository.Info.CurrentOperation})");
        }
    }

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
            _repository.Dispose();
        }
    }

    public void Dispose()
    {
        DisposeThis();
        GC.SuppressFinalize(this);
    }

    ~GitCommitDatabase() => Dispose();
}