using BuildingBlocks.Domain.Abstractions;

namespace WorkoutPrograms.Domain.ValueObjects
{
    public sealed class RepRange : ValueObject
    {
        public int Minimum { get; }
        public int Maximum { get; }

        private RepRange() { }

        public RepRange(int minimum, int maximum)
        {
            if (minimum <= 0)
                throw new ArgumentException("Minimum reps must be greater than zero.", nameof(minimum));

            if (maximum <= 0)
                throw new ArgumentException("Maximum reps must be greater than zero.", nameof(maximum));

            if (maximum < minimum)
                throw new ArgumentException("Maximum reps cannot be less than minimum reps.");

            Minimum = minimum;
            Maximum = maximum;
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Minimum;
            yield return Maximum;
        }
    }
}