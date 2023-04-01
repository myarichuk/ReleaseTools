using LibGit2Sharp;
// ReSharper disable IdentifierTypo
// ReSharper disable UnusedMemberInSuper.Global

namespace ChangelogGenerator.Api;

public abstract class GitObjectRepository<TQueryResultItem, TQueryParameters>
{
    protected readonly Repository Repository;

    // ReSharper disable once StaticMemberInGenericType
    protected static readonly CommitFilter ParameterlessFilter = new()
    {
        SortBy = CommitSortStrategies.Reverse |
                 CommitSortStrategies.Topological
    };

    protected GitObjectRepository(Repository repository)
    {
        Repository = repository;
        AssertRepositoryInGoodState();
    }

    //template method pattern :)
    public abstract IQueryable<TQueryResultItem> Query();
    public abstract IQueryable<TQueryResultItem> Query(in TQueryParameters @params);

    private void AssertRepositoryInGoodState()
    {
        if (Repository.Info.CurrentOperation != CurrentOperation.None)
        {
            throw new RepositoryNotReadyException($"Repository is in the middle of a pending interactive operation. Cannot continue. (operation type = {Repository.Info.CurrentOperation})");
        }
    }

    protected static CommitFilter CreateBetweenShaFilter(QueryParams @params, IEnumerable<Commit>? excludeParents) =>
        new()
        {
            IncludeReachableFrom = @params.IncludeFromSha,
            ExcludeReachableFrom = excludeParents,
            SortBy = (@params.Sorting == ResultSorting.NewestFirst
                         ? CommitSortStrategies.Reverse
                         : CommitSortStrategies.Time) |
                     CommitSortStrategies.Topological
        };
}