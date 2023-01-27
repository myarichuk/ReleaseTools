using LibGit2Sharp;
using Xunit.Abstractions;

namespace ChangelogGenerator.Api.Tests
{
    public class GitCommitTests: IDisposable
    {
        private readonly ITestOutputHelper _log;
        private readonly string _repositoryFolder;
        private readonly Repository _repository;

        public GitCommitTests(ITestOutputHelper log)
        {
            _log = log;

            var testRepoFolders = Directory.EnumerateDirectories("./", "test_repo_*", SearchOption.TopDirectoryOnly);
            foreach (var testRepoFolder in testRepoFolders)
            {
                try
                {
                    Directory.Delete(testRepoFolder, true);
                }
                catch(Exception ex)
                {
                    _log.WriteLine(ex.ToString());
                }
            }

            _repositoryFolder = $"test_repo_{Guid.NewGuid()}";
            Repository.Init(_repositoryFolder);
            _repository = new Repository(_repositoryFolder);
        }

        [Fact]
        public void Can_fetch_commits()
        {
            using var commitDb = new GitCommitDatabase(_repositoryFolder);
        }

        public void Dispose()
        {
            _repository.Dispose();
        }
    }
}