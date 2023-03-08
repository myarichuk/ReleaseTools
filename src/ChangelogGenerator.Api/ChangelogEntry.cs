﻿using System.Security.Cryptography;
using LibGit2Sharp;
using Parser.ConventionalCommit;

namespace ChangelogGenerator.Api
{
    /// <summary>
    /// An object representing an entry in the conventional changelog
    /// </summary>
    /// <param name="Commit"></param>
    /// <param name="Sha"></param>
    /// <param name="Author"></param>
    public record struct ChangelogEntry(ConventionalCommit Commit, string Sha, Author Author)
    {
    }

    /// <summary>
    /// An object representing the author of the commit
    /// </summary>
    /// <param name="Name"></param>
    /// <param name="Email"></param>
    /// <param name="When"></param>
    public record struct Author(string Name, string Email, DateTimeOffset When)
    {
        internal static Author From(Signature commitSignature) =>
            new(commitSignature.Name, commitSignature.Email, commitSignature.When);
    }
}
