namespace FitnessTracking.Mvc.Models;

public sealed class PaginationViewModel
{
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
    public int TotalPages { get; init; }
    public bool HasNextPage { get; init; }
    public bool HasPreviousPage { get; init; }
    public Dictionary<string, string> ExtraRouteValues { get; init; } = [];

    public int StartItem => TotalCount == 0 ? 0 : (PageNumber - 1) * PageSize + 1;
    public int EndItem => Math.Min(PageNumber * PageSize, TotalCount);

    public static readonly int[] PageSizeOptions = [5, 10, 25, 50];
}
