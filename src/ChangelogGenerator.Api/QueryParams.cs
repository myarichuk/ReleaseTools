namespace ChangelogGenerator.Api;

public record struct QueryParams(
    string? IncludeFromSha, 
    string? ExcludeToFromSha, 
    ResultSorting Sorting = ResultSorting.NewestFirst)
{
}