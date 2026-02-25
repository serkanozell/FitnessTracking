namespace BuildingBlocks.Application.Pagination
{
    public sealed class PagedResult<T>
    {
        public IReadOnlyList<T> Items { get; init; } = [];
        public int PageNumber { get; init; }
        public int PageSize { get; init; }
        public int TotalCount { get; init; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasNextPage => PageNumber < TotalPages;
        public bool HasPreviousPage => PageNumber > 1;

        public static PagedResult<T> Create(IReadOnlyList<T> items, int pageNumber, int pageSize, int totalCount) =>
            new()
            {
                Items = items,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };

        public PagedResult<TTarget> Map<TTarget>(Func<T, TTarget> selector) =>
            PagedResult<TTarget>.Create(
                Items.Select(selector).ToList(),
                PageNumber,
                PageSize,
                TotalCount);
    }
}
