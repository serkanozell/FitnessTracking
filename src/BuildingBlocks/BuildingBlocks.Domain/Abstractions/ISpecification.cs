using System.Linq.Expressions;

namespace BuildingBlocks.Domain.Abstractions
{
    public interface ISpecification<T>
    {
        Expression<Func<T, bool>>? Criteria { get; }
        IReadOnlyList<Expression<Func<T, object>>> Includes { get; }
        IReadOnlyList<string> IncludeStrings { get; }
        Expression<Func<T, object>>? OrderBy { get; }
        Expression<Func<T, object>>? OrderByDescending { get; }
        bool AsNoTracking { get; }
        bool AsSplitQuery { get; }
    }
}
