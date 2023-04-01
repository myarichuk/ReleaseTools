using LibGit2Sharp;
// ReSharper disable RegionWithSingleElement

namespace ChangelogGenerator.Api;

public sealed class TagRepository: GitObjectRepository<Tag, QueryParams>
{

    public TagRepository(Repository repository) : base(repository)
    {
    }

    public override IQueryable<Tag> Query() => 
        Repository.Tags.AsQueryable();

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
    /// Query for all tags located between the two SHAs, excluding the concrete SHAs
    /// </summary>
    /// <param name="fromSha">the SHA of commit from which to include tags</param>
    /// <param name="toSha">the SHA of commit up to which include tags</param>
    /// <returns>Queryable object that fulfills "from/to" </returns>
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