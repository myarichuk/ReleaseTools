using System.Runtime.CompilerServices;
using LibGit2Sharp;
// ReSharper disable IdentifierTypo

namespace ChangelogGenerator.Api;

public sealed class CommitRepository: GitObjectRepository<Commit, QueryParams>
{
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
        var excludeFromCommit = LookupBySha(@params.ExcludeToFromSha);

        return Repository.Commits.QueryBy(CreateBetweenShaFilter(@params, excludeFromCommit?.Parents!)).AsQueryable();

        Commit? LookupBySha(string? sha) => 
            !string.IsNullOrWhiteSpace(sha) ? 
                Repository.Lookup<Commit>(sha) : null;
    }
}