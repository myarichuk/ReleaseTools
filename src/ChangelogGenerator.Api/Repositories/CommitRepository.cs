using System.Runtime.CompilerServices;
using LibGit2Sharp;

// ReSharper disable IdentifierTypo

namespace ChangelogGenerator.Api.Repositories;

/// <summary>
/// Represents an internal sealed class for managing commit objects in a Git repository, derived from the ObjectRepository class.
/// </summary>
internal sealed class CommitRepository : ObjectRepository<Commit, QueryParams>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CommitRepository"/> class with the specified Git repository.
    /// </summary>
    /// <param name="repository">The Git repository.</param>
    public CommitRepository(Repository repository) : base(repository)
    {
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override IQueryable<Commit> Query() =>
        Repository.Commits
            .QueryBy(ParameterlessFilter)
            .AsQueryable();

    public override IQueryable<Commit> Query(in QueryParams @params)
    {
        if (@params.ExcludeToFromSha == null &&
            @params.IncludeFromSha == null)
        {
            return Query();
        }

        var excludeFromCommit = LookupBySha(@params.ExcludeToFromSha);

        return Repository.Commits.QueryBy(CreateBetweenShaFilter(@params, excludeFromCommit?.Parents!)).AsQueryable();

        Commit? LookupBySha(string? sha) =>
            !string.IsNullOrWhiteSpace(sha) ?
                Repository.Lookup<Commit>(sha) : null;
    }
}
