using System;
using System.Collections.Generic;
using System.Text;

namespace ConventionalCommitsParser
{
    /*
        List of common types used across different variants of the conventional commit convention:
        ==========================================================================================
        * feat: A new feature
        * fix: A issue fix
        * docs: Documentation changes
        * style: Changes that do not affect the meaning of the code (white-space, formatting, missing semi-colons, etc)
        * refactor: A code change that neither fixes a issue nor adds a feature
        * perf: A code change that improves performance
        * test: Adding missing tests or correcting existing tests
        * chore: Other changes that don't modify src or test files
        * build: Changes that affect the build system or external dependencies
        * ci: Changes to CI configuration files and scripts
        * breaking: A change that breaks backward compatibility
        * security: Changes related to security
        * revert: Revert a previous commit
        * config: Changes to the project configuration
        * upgrade: Upgrades package dependencies
        * downgrade: Downgrades package dependencies
        * pin: Pin package dependencies
     */

    /// <summary>
    /// An object that represents a conventional commit messages
    /// </summary>
    public partial class ConventionalCommit
    {
        /// <summary>
        /// Type of the commit, varies from convention to convention
        /// </summary>
        public string Type { get; }

        /// <summary>
        /// Is commit is breaking? (denoted by '!' character before type part)
        /// </summary>
        public bool Breaking { get; }

        /// <summary>
        /// This is an optional field that can be used to specify the area of the code that the commit relates to. For example, fix(parser) would indicate that the issue fix was made to the parser.
        /// </summary>
        public string? Scope { get; }

        /// <summary>
        /// This is a brief, one-line summary of the changes made in the commit.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// This is an optional section where you can provide more detailed information about the changes made in the commit.
        /// </summary>
        public string? Body { get; }

        /// <summary>
        /// This is an optional section where you can include information such as related issues, breaking changes, or other notes.
        /// </summary>
        public IReadOnlyDictionary<string ,string>? Footer { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConventionalCommit"/> class
        /// </summary>
        /// <param name="type"></param>
        /// <param name="description"></param>
        /// <param name="breaking"></param>
        /// <param name="scope"></param>
        /// <param name="body"></param>
        /// <param name="footer"></param>
        public ConventionalCommit(
            string type,
            string description, 
            bool breaking = false, 
            string? scope = null,
            string? body = null,
            IReadOnlyDictionary<string, string>? footer = null)
        {
            Type = type;
            Scope = scope;
            Description = description;
            Breaking = breaking;
            Body = body;
            Footer = footer;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append(Type);
            if (Breaking)
                sb.Append('!');

            if (!string.IsNullOrWhiteSpace(Scope))
                sb.AppendFormat("({0}): ", Scope);

            sb.AppendLine(Description);
            sb.AppendLine();
            sb.AppendLine();
            if (!string.IsNullOrWhiteSpace(Body))
                sb.AppendLine(Body);
            sb.AppendLine();
            sb.AppendLine();
            if (Footer != null)
            {
                foreach(var kvp in Footer)
                    AppendFooterTuple(sb, kvp);
            }

            static void AppendFooterTuple(StringBuilder stringBuilder, in KeyValuePair<string, string> footerTuple) =>
                stringBuilder.AppendFormat("{0} : {1}", footerTuple.Key, footerTuple.Value).AppendLine();

            return sb.ToString();
        }
    }
}
