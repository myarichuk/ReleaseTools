// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

using Parser.SemanticVersion;

namespace ChangelogGenerator.Api;

public ref struct Changelog
{
    public DateTime Created { get; } = DateTime.UtcNow;

    public SemanticVersion Version { get; }

    public ReadOnlySpan<ChangelogEntry> Entries { get; }

    public Changelog(
        ReadOnlySpan<ChangelogEntry> entries, 
        in SemanticVersion version, 
        DateTime? created = null)
    {
        Entries = entries;
        Version = version;
        Created = created ?? DateTime.UtcNow;
    }
}
