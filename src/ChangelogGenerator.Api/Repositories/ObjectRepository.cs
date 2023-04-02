using LibGit2Sharp;

// ReSharper disable IdentifierTypo
// ReSharper disable UnusedMemberInSuper.Global

namespace ChangelogGenerator.Api.Repositories;

/// <summary>
/// Represents an internal abstract class for managing Git objects in a repository.
/// </summary>
/// <typeparam name="TQueryResultItem">The type of query result item.</typeparam>
/// <typeparam name="TQueryParameters">The type of query parameters.</typeparam>
internal abstract class ObjectRepository<TQueryResultItem, TQueryParameters>
{
    protected readonly Repository Repository;

    // ReSharper disable once StaticMemberInGenericType
    protected static readonly CommitFilter ParameterlessFilter = new()
    {
        SortBy = CommitSortStrategies.Reverse |
                 CommitSortStrategies.Topological
    };

    protected ObjectRepository(Repository repository)
    {
        Repository = repository;
        AssertRepositoryInGoodState();
    }

    /// <summary>
    /// Queries the Git objects without parameters.
    /// </summary>
    /// <returns>An IQueryable of <see cref="TQueryResultItem"/> objects.</returns>
    public abstract IQueryable<TQueryResultItem> Query();

    /// <summary>
    /// Queries the Git objects using the specified query parameters.
    /// </summary>
    /// <param name="params">The query parameters of type <see cref="TQueryParameters"/>.</param>
    /// <returns>An IQueryable of <see cref="TQueryResultItem"/> objects.</returns>
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