using System.Linq.Expressions;

namespace BuildingBlocks.Domain.Abstractions
{
    public abstract class Specification<T> : ISpecification<T>
    {
        private readonly List<Expression<Func<T, object>>> _includes = new();
        private readonly List<string> _includeStrings = new();

        protected Specification()
        {
        }

        protected Specification(Expression<Func<T, bool>> criteria)
        {
            Criteria = criteria;
        }

        public Expression<Func<T, bool>>? Criteria { get; private set; }
        public IReadOnlyList<Expression<Func<T, object>>> Includes => _includes;
        public IReadOnlyList<string> IncludeStrings => _includeStrings;
        public Expression<Func<T, object>>? OrderBy { get; private set; }
        public Expression<Func<T, object>>? OrderByDescending { get; private set; }
        public bool AsNoTracking { get; private set; }
        public bool AsSplitQuery { get; private set; }

        protected void SetCriteria(Expression<Func<T, bool>> criteria) => Criteria = criteria;

        protected void AddInclude(Expression<Func<T, object>> includeExpression) => _includes.Add(includeExpression);

        protected void AddInclude(string includeString) => _includeStrings.Add(includeString);

        protected void ApplyOrderBy(Expression<Func<T, object>> orderByExpression) => OrderBy = orderByExpression;

        protected void ApplyOrderByDescending(Expression<Func<T, object>> orderByDescendingExpression) => OrderByDescending = orderByDescendingExpression;

        protected void ApplyNoTracking() => AsNoTracking = true;

        protected void ApplySplitQuery() => AsSplitQuery = true;
    }
}
