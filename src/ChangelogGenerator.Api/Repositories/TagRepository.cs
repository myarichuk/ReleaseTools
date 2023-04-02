using LibGit2Sharp;

// ReSharper disable RegionWithSingleElement

namespace ChangelogGenerator.Api.Repositories;

/// <summary>
/// Represents an internal sealed class for managing tag objects in a Git repository, derived from the ObjectRepository class.
/// </summary>
internal sealed class TagRepository : ObjectRepository<Tag, QueryParams>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TagRepository"/> class with the specified Git repository.
    /// </summary>
    /// <param name="repository">The Git repository.</param>
    public TagRepository(Repository repository) : base(repository)
    {
    }

    /// <summary>
    /// Queries the Git tags without parameters.
    /// </summary>
    /// <returns>An IQueryable of <see cref="Tag"/> objects.</returns>
    public override IQueryable<Tag> Query() =>
        Repository.Tags.AsQueryable();

    /// <summary>
    /// Queries the Git tags using the specified query parameters.
    /// </summary>
    /// <param name="params">The query parameters of type <see cref="QueryParams"/>.</param>
    /// <returns>An IQueryable of <see cref="Tag"/> objects.</returns>
    public override IQueryable<Tag> Query(in QueryParams @params)
    {
        var excludeCommit = Repository.Lookup<Commit>(@params.ExcludeToFromSha);
        var excludeParents = excludeCommit.Parents;

        var relevantCommitSha = new HashSet<string>(
            Repository.Commits.QueryBy(
                CreateBetweenShaFilter(@params, excludeParents))
                .Select(c => c.Sha));

        return relevantCommitSha.Count == 0
            ? Enumerable.Empty<Tag>().AsQueryable()
            : Repository.Tags
                .Where(tag =>
                    tag.PeeledTarget is Commit &&
                    relevantCommitSha.Contains(tag.Target.Sha))
                .AsQueryable();
    }

    /// <summary>
    /// Queries for all tags located between the two SHAs, excluding the concrete SHAs.
    /// </summary>
    /// <param name="fromSha">The SHA of the commit from which to include tags.</param>
    /// <param name="toSha">The SHA of the commit up to which include tags.</param>
    /// <returns>An IQueryable of <see cref="Tag"/> objects that fulfill the "from/to" condition.</returns>
    public IQueryable<Tag> Query(string fromSha, string toSha)
    {
        var excludeCommit = Repository.Lookup<Commit>(fromSha);
        var excludeParents = excludeCommit.Parents;

        var relevantCommitSha = new HashSet<string>(
            Repository.Commits.QueryBy(new CommitFilter
            {
                IncludeReachableFrom = toSha,
                ExcludeReachableFrom = excludeParents,
                SortBy = CommitSortStrategies.Reverse | CommitSortStrategies.Topological
            }).Select(c => c.Sha));

        return relevantCommitSha.Count == 0
            ? Enumerable.Empty<Tag>().AsQueryable()
            : Repository.Tags
                .Where(tag =>
                    tag.PeeledTarget is Commit &&
                    relevantCommitSha.Contains(tag.Target.Sha))
                .AsQueryable();
    }
}