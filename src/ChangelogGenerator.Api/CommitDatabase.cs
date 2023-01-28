using LibGit2Sharp;
using static ChangelogGenerator.Api.CommitDatabase;

namespace ChangelogGenerator.Api;

public class CommitDatabase: IDisposable
{
    private bool _isDisposed;
    private readonly Repository _repository;

    public enum Sorting
    {
        NewestFirst,
        OldestFirst
    }

    public CommitDatabase(string repositoryPath)
    {
        ThrowIfInvalidRepo(repositoryPath);

        _repository = new Repository(repositoryPath);
        AssertRepositoryInGoodState();
    }

    public CommitDatabase(string repositoryPath, string username, string email)
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

    public IEnumerable<Commit> Query(Sorting commitSorting = Sorting.NewestFirst) => 
        _repository.Commits.QueryBy(new CommitFilter {SortBy = (commitSorting == Sorting.NewestFirst ? CommitSortStrategies.Reverse : CommitSortStrategies.Time) | CommitSortStrategies.Topological});

    public IEnumerable<Commit> Query(Commit oldestCommitToInclude, Sorting commitSorting = Sorting.NewestFirst)
    {
        return _repository.Commits.QueryBy(new CommitFilter
        {
            IncludeReachableFrom = oldestCommitToInclude.Sha,
            SortBy = (commitSorting == Sorting.NewestFirst ? CommitSortStrategies.Reverse : CommitSortStrategies.Time) | CommitSortStrategies.Topological
        });
    }

    public IEnumerable<Commit> Query(string startShaOrTag, string endShaOrTag, Sorting commitSorting = Sorting.NewestFirst) => 
        _repository.Commits.QueryBy(new CommitFilter
        {
            IncludeReachableFrom = startShaOrTag,
            ExcludeReachableFrom = endShaOrTag,
            SortBy = (commitSorting == Sorting.NewestFirst ? CommitSortStrategies.Reverse : CommitSortStrategies.Time) | CommitSortStrategies.Topological
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

    ~CommitDatabase() => Dispose();
}