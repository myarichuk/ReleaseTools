namespace ChangelogGenerator.Api;

/// <summary>
/// Represents a readonly record struct for semantic versioning.
/// </summary>
public readonly record struct SemanticVersion(
    int Major,
    int Minor,
    int Patch,
    string Prerelease = "",
    string Build = "")
{
    /// <summary>
    /// Returns a string that represents the current semantic version object.
    /// </summary>
    /// <returns>A string in the format of Major.Minor.Patch[-Prerelease][+Build].</returns>
    public override string ToString() =>
        $"{Major}.{Minor}.{Patch}{(string.IsNullOrEmpty(Prerelease) ? "" : "-" + Prerelease)}{(string.IsNullOrEmpty(Build) ? "" : "+" + Build)}";
}