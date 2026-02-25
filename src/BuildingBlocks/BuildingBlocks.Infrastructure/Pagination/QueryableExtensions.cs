using BuildingBlocks.Application.Pagination;
using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.Infrastructure.Pagination
{
    public static class QueryableExtensions
    {
        public static async Task<(IReadOnlyList<T> Items, int TotalCount)> ToPagedListAsync<T>(
            this IQueryable<T> source,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            pageNumber = PaginationDefaults.NormalizePageNumber(pageNumber);
            pageSize = PaginationDefaults.NormalizePageSize(pageSize);

            var totalCount = await source.CountAsync(cancellationToken);

            var items = await source
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }
    }
}