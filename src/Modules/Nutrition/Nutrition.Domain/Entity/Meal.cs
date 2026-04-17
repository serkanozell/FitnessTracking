using BuildingBlocks.Domain.Abstractions;
using BuildingBlocks.Domain.Exceptions;
using Nutrition.Domain.Enums;
using Nutrition.Domain.ValueObjects;

namespace Nutrition.Domain.Entity
{
    public class Meal : Entity<Guid>
    {
        public Guid MealPlanId { get; private set; }
        public string Name { get; private set; }
        public int Order { get; private set; }

        private readonly List<MealItem> _mealItems = new();
        public IReadOnlyList<MealItem> MealItems => _mealItems.AsReadOnly();

        private Meal() { }

        internal Meal(Guid id, Guid mealPlanId, string name, int order)
        {
            Id = id;
            MealPlanId = mealPlanId;
            Name = name;
            Order = order;
        }

        internal void Update(string name, int order)
        {
            Name = name;
            Order = order;
        }

        internal MealItem AddItem(Guid foodId, string foodName, decimal quantity, ServingUnit servingUnit, MacroNutrients macros)
        {
            var item = new MealItem(Guid.NewGuid(), Id, foodId, foodName, quantity, servingUnit, macros);
            _mealItems.Add(item);
            return item;
        }

        internal void RemoveItem(Guid mealItemId)
        {
            var item = _mealItems.SingleOrDefault(x => x.Id == mealItemId);
            if (item is null)
            {
                return;
            }

            _mealItems.Remove(item);
        }

        internal MealItem UpdateItemQuantity(Guid mealItemId, decimal quantity, MacroNutrients macros)
        {
            var item = _mealItems.SingleOrDefault(x => x.Id == mealItemId)
                       ?? throw new DomainNotFoundException("MealItem", mealItemId, "Meal", Id);

            item.UpdateQuantity(quantity, macros);
            return item;
        }
    }
}
