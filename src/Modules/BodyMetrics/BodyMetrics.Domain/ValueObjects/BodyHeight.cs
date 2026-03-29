using BuildingBlocks.Domain.Abstractions;

namespace BodyMetrics.Domain.ValueObjects
{
    public sealed class BodyHeight : ValueObject
    {
        public decimal Value { get; }

        private BodyHeight() { }

        public BodyHeight(decimal value)
        {
            if (value <= 0)
                throw new ArgumentException("Height must be greater than zero.", nameof(value));

            if (value > 300)
                throw new ArgumentException("Height cannot exceed 300 cm.", nameof(value));

            Value = value;
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
