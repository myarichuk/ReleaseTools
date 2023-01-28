using FluentAssertions;
using LibGit2Sharp;
using System.Runtime.InteropServices;
using Xunit.Abstractions;

namespace ChangelogGenerator.Api.Tests
{
    public class GitCommitTests: IDisposable
    {
        private readonly ITestOutputHelper _log;
        private readonly string _repositoryFolder;
        private readonly Repository _repository;

        private static readonly Signature DefaultSignature = 
            new("John Dow", "john.dow@foobar.com", DateTimeOffset.UtcNow);

        public GitCommitTests(ITestOutputHelper log)
        {
            _log = log;

            DeleteOldTestRepos();

            _repositoryFolder = $"test_repo_{Guid.NewGuid()}";
            Repository.Init(_repositoryFolder);
            _repository = new Repository(_repositoryFolder);
        }

        [Fact]
        public void Can_fetch_commits()
        {
            using var commitDb = new GitCommitDatabase(_repositoryFolder);

            File.WriteAllText(Path.Combine(_repositoryFolder, "foobar1.txt"), "some file contents");
            
            Commands.Stage(_repository, "*");
            _repository.Commit("commit A", DefaultSignature, DefaultSignature);

            File.WriteAllText(Path.Combine(_repositoryFolder, "foobar2.txt"), "some file contents");
            
            Commands.Stage(_repository, "*");
            _repository.Commit("commit B", DefaultSignature, DefaultSignature);

            File.WriteAllText(Path.Combine(_repositoryFolder, "foobar3.txt"), "some file contents");
            
            Commands.Stage(_repository, "*");
            _repository.Commit("commit C", DefaultSignature, DefaultSignature);
            
            var commitsQueryResult = commitDb.Query().ToArray();

            commitsQueryResult.Should().HaveCount(3);
            var commitMessages = commitsQueryResult.Select(x => x.Message);
            commitMessages.Should().ContainInOrder("commit A\n", "commit B\n", "commit C\n");
        }

        private void DeleteOldTestRepos()
        {
            var testRepoFolders = Directory.EnumerateDirectories("./", "test_repo_*", SearchOption.TopDirectoryOnly);
            foreach (var testRepoFolder in testRepoFolders)
            {
                try
                {
                    DeleteDirectory(testRepoFolder);
                }
                catch (Exception ex)
                {
                    _log.WriteLine(ex.ToString());
                }
            }
        }

        public void Dispose() => _repository.Dispose();

        //credit: https://github.com/rgl/UseLibgit2sharp/blob/master/Program.cs
        // recursively force the deletion of the given directory.
        // NB on windows, because the git repository files are read-only,
        //    Directory.Delete will fail with UnauthorizedAccessException,
        //    so, before deleting a file, we have to remove its read-only
        //    attribute.
        private static void DeleteDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                return;
            }

            // on non-Windows use the regular Directory.Delete because they
            // do not care about the file permissions. to delete a file,
            // only the parent directory permissions matter.
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Directory.Delete(path, true);
                return;
            }

            foreach (var directoryPath in Directory.GetDirectories(path))
            {
                DeleteDirectory(directoryPath);
            }

            foreach (var filePath in Directory.GetFiles(path))
            {
                var fileAttributes = File.GetAttributes(filePath);

                if ((fileAttributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    File.SetAttributes(filePath, fileAttributes ^ FileAttributes.ReadOnly);
                }

                File.Delete(filePath);
            }

            Directory.Delete(path);
        }
    }
}