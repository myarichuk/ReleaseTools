﻿using System.Runtime.CompilerServices;
using LibGit2Sharp;
using static ChangelogGenerator.Api.CommitDatabase;

namespace ChangelogGenerator.Api;

public class CommitDatabase: GitObjectDatabase<Commit>
{
    public enum Sorting
    {
        NewestFirst,
        OldestFirst
    }

    public CommitDatabase(string repositoryPath): base(repositoryPath)
    {
    }

    public CommitDatabase(string repositoryPath, string username, string email): base(repositoryPath, username, email)
    {
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override IEnumerable<Commit> Query() => Repository.Commits;

    public IEnumerable<Commit> Query(Sorting commitSorting = Sorting.NewestFirst) => 
        Repository.Commits.QueryBy(new CommitFilter {SortBy = (commitSorting == Sorting.NewestFirst ? CommitSortStrategies.Reverse : CommitSortStrategies.Time) | CommitSortStrategies.Topological});

    public IEnumerable<Commit> Query(Commit includeFromThisCommit, Sorting commitSorting = Sorting.NewestFirst) =>
        Repository.Commits.QueryBy(new CommitFilter
        {
            IncludeReachableFrom = includeFromThisCommit.Sha,
            SortBy = (commitSorting == Sorting.NewestFirst ? CommitSortStrategies.Reverse : CommitSortStrategies.Time) | CommitSortStrategies.Topological
        });

    public IEnumerable<Commit> Query(Commit includeFromThisCommit, Commit excludeFromThisCommit, Sorting commitSorting = Sorting.NewestFirst) => 
        Repository.Commits.QueryBy(new CommitFilter
        {
            IncludeReachableFrom = includeFromThisCommit,
            ExcludeReachableFrom = excludeFromThisCommit.Parents,
            SortBy = (commitSorting == Sorting.NewestFirst ? CommitSortStrategies.Reverse : CommitSortStrategies.Time) | CommitSortStrategies.Topological
        });
}