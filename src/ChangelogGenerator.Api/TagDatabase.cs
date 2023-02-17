using LibGit2Sharp;

namespace ChangelogGenerator.Api
{
    public class TagDatabase: GitObjectDatabase<Tag>
    {
        public TagDatabase(Repository gitRepository) : base(gitRepository)
        {
        }

        public TagDatabase(string repositoryPath) : base(repositoryPath)
        {
        }

        public TagDatabase(string repositoryPath, string username, string email) : base(repositoryPath, username, email)
        {
        }

        public override IQueryable<Tag> Query() => 
            GitRepository.Tags.AsQueryable();

        /// <summary>
        /// Query for all tags located between the two SHAs, excluding the concrete SHAs
        /// </summary>
        /// <param name="fromSha">the SHA of commit from which to include tags</param>
        /// <param name="toSha">the SHA of commit up to which include tags</param>
        /// <returns>Queryable object that fulfills "from/to" </returns>
        public IQueryable<Tag> Query(string fromSha, string toSha)
        {
            var excludeCommit = GitRepository.Lookup<Commit>(fromSha);
            var excludeParents = excludeCommit.Parents;

            var relevantCommitSha = new HashSet<string>(
                GitRepository.Commits.QueryBy(new CommitFilter
                {
                    IncludeReachableFrom = toSha,
                    ExcludeReachableFrom = excludeParents,
                    SortBy = CommitSortStrategies.Reverse | CommitSortStrategies.Topological
                }).Select(c => c.Sha));

            return relevantCommitSha.Count == 0
                ? Enumerable.Empty<Tag>().AsQueryable()
                : GitRepository.Tags
                    .Where(tag =>
                        tag.PeeledTarget is Commit &&
                        relevantCommitSha.Contains(tag.Target.Sha))
                    .AsQueryable();
        }
    }
}
