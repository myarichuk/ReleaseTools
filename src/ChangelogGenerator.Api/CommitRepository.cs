using System.Runtime.CompilerServices;
using LibGit2Sharp;

namespace ChangelogGenerator.Api;

public class CommitRepository: GitObjectRepository<Commit, QueryParams>
{
    #region Query Parameters

    #endregion

    public CommitRepository(Repository repository) : base(repository)
    {
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override IQueryable<Commit> Query() => 
        Repository.Commits.QueryBy(new CommitFilter
        {
            SortBy = CommitSortStrategies.Reverse |
                     CommitSortStrategies.Topological
        }).AsQueryable();

    public override IQueryable<Commit> Query(in QueryParams @params)
    {
        var includeFromCommit = LookupBySha(@params.IncludeFromSha);
        var excludeFromCommit = LookupBySha(@params.ExcludeToFromSha);

        return Repository.Commits.QueryBy(new CommitFilter
        {
            IncludeReachableFrom = includeFromCommit,
            ExcludeReachableFrom = excludeFromCommit?.Parents,
            SortBy = (@params.Sorting == ResultSorting.NewestFirst
                         ? CommitSortStrategies.Reverse
                         : CommitSortStrategies.Time) |
                     CommitSortStrategies.Topological
        }).AsQueryable();

        Commit? LookupBySha(string? sha) => 
            !string.IsNullOrWhiteSpace(sha) ? 
                Repository.Lookup<Commit>(sha) : null;
    }
}