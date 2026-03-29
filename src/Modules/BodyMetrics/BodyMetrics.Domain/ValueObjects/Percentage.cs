using BuildingBlocks.Domain.Abstractions;

namespace BodyMetrics.Domain.ValueObjects
{
    public sealed class Percentage : ValueObject
    {
        public decimal Value { get; }

        private Percentage() { }

        public Percentage(decimal value)
        {
            if (value < 0)
                throw new ArgumentException("Percentage cannot be negative.", nameof(value));

            if (value > 100)
                throw new ArgumentException("Percentage cannot exceed 100.", nameof(value));

            Value = value;
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
