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
    }
}
