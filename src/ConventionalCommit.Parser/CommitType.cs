namespace ConventionalCommit.Parser
{
    /// <summary>
    /// Details valid commit types, as per conventional commit spec
    /// </summary>
    public enum CommitType
    {
        /// <summary>
        /// A type that is not one of the commit types mentioned in the spec
        /// </summary>
        Other,

        /// <summary>
        /// A new feature
        /// </summary>
        Feat,

        /// <summary>
        /// A fix of an issue
        /// </summary>
        Fix,

        /// <summary>
        /// Documentation changes
        /// </summary>
        Docs,

        /// <summary>
        /// Changes that do not affect the meaning of the code (white-space, formatting, missing semi-colons, etc)
        /// </summary>
        Style,

        /// <summary>
        /// A code change that neither fixes a issue nor adds a feature
        /// </summary>
        Refactor,

        /// <summary>
        /// A code change that improves performance
        /// </summary>
        Perf,

        /// <summary>
        /// Adding missing tests or correcting existing tests
        /// </summary>
        Test,

        /// <summary>
        /// Other changes that don't modify src or test files
        /// </summary>
        Chore,

        /// <summary>
        /// Changes that affect the build system or external dependencies
        /// </summary>
        Build,

        /// <summary>
        /// Changes to CI configuration files and scripts
        /// </summary>
        Ci,

        /// <summary>
        /// A change that breaks backward compatibility
        /// </summary>
        Breaking,

        /// <summary>
        /// Changes related to security
        /// </summary>
        Security,

        /// <summary>
        /// Revert a previous commit
        /// </summary>
        Revert,

        /// <summary>
        /// Changes to the project configuration
        /// </summary>
        Config,

        /// <summary>
        /// Upgrades package dependencies
        /// </summary>
        Upgrade,

        /// <summary>
        /// Downgrades package dependencies
        /// </summary>
        Downgrade,

        /// <summary>
        /// Pin package dependencies
        /// </summary>
        Pin
    }
}