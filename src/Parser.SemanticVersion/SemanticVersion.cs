namespace Parser.SemanticVersion
{
    public partial record struct SemanticVersion
    {
        /// <summary>
        /// semantic tag of the build section (e.g rc4, build5, alpha.2, beta)
        /// </summary>
        /// <remarks>See <see cref="SemanticTag"/> enum for details of which tags it supports</remarks>
        public SemanticVersionTag BuildVersion { get; set; }

        /// <summary>
        /// semantic tag of the pre-release section (e.g rc4, build5, alpha.2, beta)
        /// </summary>
        /// <remarks>See <see cref="SemanticTag"/> enum for details of which tags it supports</remarks>
        public SemanticVersionTag PreReleaseVersion { get; set; }


        /// <summary>
        /// major version
        /// </summary>
        public int Major { get; set; }

        /// <summary>
        /// minor version
        /// </summary>
        public int Minor { get; set; }

        /// <summary>
        /// patch version
        /// </summary>
        public int Patch { get; set; }

        /// <summary>
        /// pre-release section
        /// </summary>
        public string PreRelease { get; set; }

        /// <summary>
        /// build section
        /// </summary>
        public string Build { get; set; }
    }
}
