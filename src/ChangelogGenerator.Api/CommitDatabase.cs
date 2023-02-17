using System.Runtime.CompilerServices;
using LibGit2Sharp;

namespace ChangelogGenerator.Api;

public class CommitDatabase: GitObjectDatabase<Commit>
{
    public enum Sorting
    {
        NewestFirst,
        OldestFirst
    }

    public CommitDatabase(Repository gitRepository) : base(gitRepository)
    {
    }

    public CommitDatabase(string repositoryPath): base(repositoryPath)
    {
    }

    public CommitDatabase(string repositoryPath, string username, string email): base(repositoryPath, username, email)
    {
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override IEnumerable<Commit> Query() => GitRepository.Commits;

    public IEnumerable<Commit> Query(Sorting commitSorting = Sorting.NewestFirst) => 
        GitRepository.Commits.QueryBy(new CommitFilter {SortBy = (commitSorting == Sorting.NewestFirst ? CommitSortStrategies.Reverse : CommitSortStrategies.Time) | CommitSortStrategies.Topological});

    public IEnumerable<Commit> Query(Commit includeFromThisCommit, Sorting commitSorting = Sorting.NewestFirst) =>
        GitRepository.Commits.QueryBy(new CommitFilter
        {
            IncludeReachableFrom = includeFromThisCommit.Sha,
            SortBy = (commitSorting == Sorting.NewestFirst ? CommitSortStrategies.Reverse : CommitSortStrategies.Time) | CommitSortStrategies.Topological
        });

    public IEnumerable<Commit> Query(Commit includeFromThisCommit, Commit excludeFromThisCommit, Sorting commitSorting = Sorting.NewestFirst) => 
        GitRepository.Commits.QueryBy(new CommitFilter
        {
            IncludeReachableFrom = includeFromThisCommit,
            ExcludeReachableFrom = excludeFromThisCommit.Parents,
            SortBy = (commitSorting == Sorting.NewestFirst ? CommitSortStrategies.Reverse : CommitSortStrategies.Time) | CommitSortStrategies.Topological
        });
}