using BuildingBlocks.Application.Pagination;
using BuildingBlocks.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.Infrastructure.Specifications
{
    public static class SpecificationQueryableExtensions
    {
        public static async Task<IReadOnlyList<T>> ToListAsync<T>(
            this IQueryable<T> source,
            ISpecification<T> specification,
            CancellationToken cancellationToken = default)
            where T : class
        {
            return await SpecificationEvaluator.GetQuery(source, specification).ToListAsync(cancellationToken);
        }

        public static async Task<(IReadOnlyList<T> Items, int TotalCount)> ToPagedListAsync<T>(
            this IQueryable<T> source,
            ISpecification<T> specification,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default)
            where T : class
        {
            pageNumber = PaginationDefaults.NormalizePageNumber(pageNumber);
            pageSize = PaginationDefaults.NormalizePageSize(pageSize);

            var query = SpecificationEvaluator.GetQuery(source, specification);

            var totalCount = await query.CountAsync(cancellationToken);

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (items, totalCount);
        }
    }
}
