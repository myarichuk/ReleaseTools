using LibGit2Sharp;
using System.Runtime.InteropServices;
using Xunit.Abstractions;

namespace ChangelogGenerator.Api.Tests
{
    public abstract class BaseGitObjectTests: IDisposable
    {
        protected readonly ITestOutputHelper Log;
        protected readonly string GitRepositoryFolder;
        protected readonly Repository GitRepository;

        protected static readonly Signature DefaultSignature = 
            new("John Dow", "john.dow@foobar.com", DateTimeOffset.UtcNow);

        protected BaseGitObjectTests(ITestOutputHelper log)
        {
            Log = log;

            DeleteOldTestRepos();

            GitRepositoryFolder = $"test_repo_{Guid.NewGuid()}";
            Repository.Init(GitRepositoryFolder);
            GitRepository = new Repository(GitRepositoryFolder);
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
                    Log.WriteLine(ex.ToString());
                }
            }
        }

        public void Dispose() => GitRepository.Dispose();

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

        protected string CommitDummyFile(string filename)
        {
            File.WriteAllText(Path.Combine(GitRepositoryFolder, filename), "some file contents");

            Commands.Stage(GitRepository, "*");
            var commitInfo = GitRepository.Commit($"add file {filename}", DefaultSignature, DefaultSignature);
            return commitInfo.Sha;
        }
    }
}
