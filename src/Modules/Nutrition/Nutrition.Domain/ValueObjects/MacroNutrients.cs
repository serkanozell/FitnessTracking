using BuildingBlocks.Domain.Abstractions;

namespace Nutrition.Domain.ValueObjects
{
    public sealed class MacroNutrients : ValueObject
    {
        public decimal Calories { get; }
        public decimal Protein { get; }
        public decimal Carbohydrates { get; }
        public decimal Fat { get; }

        private MacroNutrients() { }

        public MacroNutrients(decimal calories, decimal protein, decimal carbohydrates, decimal fat)
        {
            if (calories < 0)
                throw new ArgumentException("Calories cannot be negative.", nameof(calories));

            if (protein < 0)
                throw new ArgumentException("Protein cannot be negative.", nameof(protein));

            if (carbohydrates < 0)
                throw new ArgumentException("Carbohydrates cannot be negative.", nameof(carbohydrates));

            if (fat < 0)
                throw new ArgumentException("Fat cannot be negative.", nameof(fat));

            Calories = calories;
            Protein = protein;
            Carbohydrates = carbohydrates;
            Fat = fat;
        }

        public static MacroNutrients Calculate(MacroNutrients perServing, decimal quantity, decimal defaultServingSize)
        {
            if (defaultServingSize <= 0)
                throw new ArgumentException("Default serving size must be greater than zero.", nameof(defaultServingSize));

            var ratio = quantity / defaultServingSize;

            return new MacroNutrients(
                Math.Round(perServing.Calories * ratio, 2),
                Math.Round(perServing.Protein * ratio, 2),
                Math.Round(perServing.Carbohydrates * ratio, 2),
                Math.Round(perServing.Fat * ratio, 2));
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Calories;
            yield return Protein;
            yield return Carbohydrates;
            yield return Fat;
        }
    }
}
