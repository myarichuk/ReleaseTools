namespace ChangelogGenerator.Api.Repositories;

internal record struct QueryParams(
    string? IncludeFromSha,
    string? ExcludeToFromSha,
    ResultSorting Sorting = ResultSorting.NewestFirst)
{
}