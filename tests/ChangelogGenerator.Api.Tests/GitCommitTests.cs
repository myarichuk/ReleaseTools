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

        [Fact(DisplayName = "Can fetch specific commit from Query() method")]
        public void Can_fetch_specific_commit()
        {
            using var commitDb = new CommitDatabase(_repositoryFolder);

            for (int i = 1; i < 6; i++)
            {
                CommitDummyFile($"foobar{i}.txt");
            }

            var commitInfoByMessage = commitDb.Query().FirstOrDefault(c => c.Message == "add file foobar3.txt\n");
            commitInfoByMessage.Should().NotBeNull();

            var commitInfoBySha = commitDb.Query().FirstOrDefault(c => c.Tree.Sha == commitInfoByMessage!.Tree.Sha);
            commitInfoBySha.Should().NotBeNull();
        }

        [Fact(DisplayName = "Can fetch all commits")]
        public void Can_fetch_commits()
        {
            using var commitDb = new CommitDatabase(_repositoryFolder);

            for (int i = 1; i < 6; i++)
            {
                CommitDummyFile($"foobar{i}.txt");
            }
            
            var commitsQueryResult = commitDb.Query().ToArray();

            var commitMessages = commitsQueryResult.Select(x => x.Message);
            commitMessages.Should()
                .ContainInOrder(
                    "add file foobar1.txt\n",
                    "add file foobar2.txt\n",
                    "add file foobar3.txt\n",
                    "add file foobar4.txt\n",
                    "add file foobar5.txt\n");
        }

        [Fact(DisplayName = "Can fetch commits starting from specific sha")]
        public void Can_fetch_commits_starting_from_sha()
        {
            using var commitDb = new CommitDatabase(_repositoryFolder);

            for (int i = 1; i < 6; i++)
            {
                CommitDummyFile($"foobar{i}.txt");
            }

            var commitInfo = commitDb.Query().FirstOrDefault(c => c.Message == "add file foobar3.txt\n");
            commitInfo.Should().NotBeNull(); //sanity check

            var boundedCommitsQueryResults = commitDb.Query(commitInfo!);
            
            var commitMessages = boundedCommitsQueryResults.Select(x => x.Message);
            commitMessages.Should()
                .ContainInOrder(
                    "add file foobar1.txt\n",
                    "add file foobar2.txt\n",
                    "add file foobar3.txt\n");

            commitInfo = commitDb.Query().FirstOrDefault(c => c.Message == "add file foobar2.txt\n");
            commitInfo.Should().NotBeNull(); 

            boundedCommitsQueryResults = commitDb.Query(commitInfo!);
            commitMessages = boundedCommitsQueryResults.Select(x => x.Message);
            commitMessages.Should()
                .ContainInOrder(
                    "add file foobar1.txt\n",
                    "add file foobar2.txt\n");
        }

        private void CommitDummyFile(string filename)
        {
            File.WriteAllText(Path.Combine(_repositoryFolder, filename), "some file contents");

            Commands.Stage(_repository, "*");
            _repository.Commit($"add file {filename}", DefaultSignature, DefaultSignature);
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