namespace ChangelogGenerator.Api;

public readonly record struct SemanticVersion(
    int Major,
    int Minor,
    int Patch,
    string Prerelease = "",
    string Build = "")
{
    public override string ToString() =>
        $"{Major}.{Minor}.{Patch}{(string.IsNullOrEmpty(Prerelease) ? "" : "-" + Prerelease)}{(string.IsNullOrEmpty(Build) ? "" : "+" + Build)}";
}