﻿// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace Parser.SemanticVersion
{
    public record struct SemanticVersionTag(SemanticTag Tag, int? Version, string? Identifier)
    {
        public SemanticTag Tag { get; internal set; } = Tag;
        public int? Version { get; internal set; } = Version;
        public string? Identifier { get; internal set; } = Identifier;
    }
}
