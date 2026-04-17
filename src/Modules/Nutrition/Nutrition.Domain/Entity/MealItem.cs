using BuildingBlocks.Domain.Abstractions;
using Nutrition.Domain.Enums;
using Nutrition.Domain.ValueObjects;

namespace Nutrition.Domain.Entity
{
    public class MealItem : Entity<Guid>
    {
        public Guid MealId { get; private set; }
        public Guid FoodId { get; private set; }
        public string FoodName { get; private set; }
        public decimal Quantity { get; private set; }
        public ServingUnit ServingUnit { get; private set; }
        public MacroNutrients Macros { get; private set; }

        private MealItem() { }

        internal MealItem(Guid id, Guid mealId, Guid foodId, string foodName, decimal quantity, ServingUnit servingUnit, MacroNutrients macros)
        {
            Id = id;
            MealId = mealId;
            FoodId = foodId;
            FoodName = foodName;
            Quantity = quantity;
            ServingUnit = servingUnit;
            Macros = macros;
        }

        internal void UpdateQuantity(decimal quantity, MacroNutrients macros)
        {
            Quantity = quantity;
            Macros = macros;
        }
    }
}
