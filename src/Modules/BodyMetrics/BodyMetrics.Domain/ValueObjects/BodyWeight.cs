using BuildingBlocks.Domain.Abstractions;

namespace BodyMetrics.Domain.ValueObjects
{
    public sealed class BodyWeight : ValueObject
    {
        public decimal Value { get; }

        private BodyWeight() { }

        public BodyWeight(decimal value)
        {
            if (value <= 0)
                throw new ArgumentException("Weight must be greater than zero.", nameof(value));

            if (value > 500)
                throw new ArgumentException("Weight cannot exceed 500 kg.", nameof(value));

            Value = value;
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
