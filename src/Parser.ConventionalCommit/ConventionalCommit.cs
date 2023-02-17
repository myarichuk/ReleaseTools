using System;
using System.Collections.Generic;
using System.Text;
// ReSharper disable MemberCanBePrivate.Global

namespace Parser.ConventionalCommit
{
    /// <summary>
    /// An object that represents a conventional commit messages
    /// </summary>
    public partial record struct ConventionalCommit
    {
        /// <summary>
        /// Type of the commit, varies from convention to convention
        /// </summary>
        public CommitType Type { get; internal set; }

        /// <summary>
        /// Type of the commit, either equals the <seealso cref="Type"/> value or if 'Other' then equals the other type
        /// </summary>
        public string TypeAsString { get; internal set; }

        /// <summary>
        /// Is commit is breaking? (denoted by '!' character before type part)
        /// </summary>
        public bool IsBreaking { get; internal set; }

        /// <summary>
        /// This is an optional field that can be used to specify the area of the code that the commit relates to. For example, fix(parser) would indicate that the issue fix was made to the parser.
        /// </summary>
        public string? Scope { get; internal set; }

        /// <summary>
        /// This is a brief, one-line summary of the changes made in the commit.
        /// </summary>
        public string Description { get; internal set; }

        /// <summary>
        /// This is an optional section where you can provide more detailed information about the changes made in the commit.
        /// </summary>
        public string? Body { get; internal set; }

        private readonly Dictionary<string, string> _footer = new();

        /// <summary>
        /// This is an optional section where you can include information such as related issues, breaking changes, or other notes.
        /// </summary>
        public IReadOnlyDictionary<string, string> Footer => _footer;

        internal void AddFooterItem(string key, string value) => _footer.Add(key, value);

        /// <summary>
        /// Initializes an empty new instance of the <see cref="ConventionalCommit"/> class
        /// </summary>
        public ConventionalCommit()
        {
            Description = string.Empty;
            Type = CommitType.Other;
            TypeAsString = string.Empty;
            IsBreaking = false;
            Scope = default;
            Body = default;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConventionalCommit"/> class
        /// </summary>
        /// <param name="type"></param>
        /// <param name="description"></param>
        /// <param name="isBreaking"></param>
        /// <param name="scope"></param>
        /// <param name="body"></param>
        /// <param name="footer"></param>
        public ConventionalCommit(
            string type,
            string description, 
            bool isBreaking = false, 
            string? scope = null,
            string? body = null,
            IDictionary<string, string>? footer = null)
        {
            Type = Enum.TryParse<CommitType>(type, true, out var parsedEnum) ? parsedEnum : CommitType.Other;
            TypeAsString = type;
            Scope = scope;
            Description = description;
            IsBreaking = isBreaking;
            Body = body;
            _footer = footer != null ? new Dictionary<string, string>(footer): new Dictionary<string, string>();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append(TypeAsString);
            if (IsBreaking)
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
