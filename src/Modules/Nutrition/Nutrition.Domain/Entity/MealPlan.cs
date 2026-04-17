using BuildingBlocks.Domain.Abstractions;
using BuildingBlocks.Domain.Exceptions;
using Nutrition.Domain.Enums;
using Nutrition.Domain.Events;
using Nutrition.Domain.ValueObjects;

namespace Nutrition.Domain.Entity
{
    public class MealPlan : AggregateRoot<Guid>
    {
        public Guid UserId { get; private set; }
        public string Name { get; private set; }
        public DateTime Date { get; private set; }
        public string? Note { get; private set; }

        private readonly List<Meal> _meals = new();
        public IReadOnlyList<Meal> Meals => _meals.AsReadOnly();

        private MealPlan() { }

        public static MealPlan Create(Guid userId, string name, DateTime date, string? note)
        {
            var mealPlan = new MealPlan
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Name = name,
                Date = date,
                Note = note
            };

            mealPlan.AddDomainEvent(new MealPlanCreatedEvent(mealPlan.Id, userId));

            return mealPlan;
        }

        public void Update(string name, DateTime date, string? note)
        {
            Name = name;
            Date = date;
            Note = note;

            AddDomainEvent(new MealPlanUpdatedEvent(Id));
        }

        public void Activate()
        {
            IsActive = true;
            IsDeleted = false;

            AddDomainEvent(new MealPlanActivatedEvent(Id));
        }

        public void Delete(string deletedBy)
        {
            IsActive = false;
            IsDeleted = true;

            AddDomainEvent(new MealPlanDeletedEvent(Id, deletedBy));
        }

        public Meal AddMeal(string name, int order)
        {
            if (Meals.Any(x => x.Name == name))
            {
                throw new BusinessRuleViolationException(
                    $"Meal with name ({name}) already exists in meal plan {Id}.");
            }

            var meal = new Meal(Guid.NewGuid(), Id, name, order);
            _meals.Add(meal);

            AddDomainEvent(new MealPlanMealChangedEvent(Id));

            return meal;
        }

        public void UpdateMeal(Guid mealId, string name, int order)
        {
            var meal = _meals.SingleOrDefault(x => x.Id == mealId)
                       ?? throw new DomainNotFoundException("Meal", mealId, "MealPlan", Id);

            meal.Update(name, order);

            AddDomainEvent(new MealPlanMealChangedEvent(Id));
        }

        public void RemoveMeal(Guid mealId)
        {
            var meal = _meals.SingleOrDefault(x => x.Id == mealId);
            if (meal is null)
            {
                return;
            }

            _meals.Remove(meal);

            AddDomainEvent(new MealPlanMealChangedEvent(Id));
        }

        public MealItem AddItemToMeal(Guid mealId, Guid foodId, string foodName, decimal quantity, ServingUnit servingUnit, MacroNutrients macros)
        {
            var meal = _meals.SingleOrDefault(x => x.Id == mealId)
                       ?? throw new DomainNotFoundException("Meal", mealId, "MealPlan", Id);

            var item = meal.AddItem(foodId, foodName, quantity, servingUnit, macros);

            AddDomainEvent(new MealItemChangedEvent(Id, mealId));

            return item;
        }

        public void RemoveItemFromMeal(Guid mealId, Guid mealItemId)
        {
            var meal = _meals.SingleOrDefault(x => x.Id == mealId)
                       ?? throw new DomainNotFoundException("Meal", mealId, "MealPlan", Id);

            meal.RemoveItem(mealItemId);

            AddDomainEvent(new MealItemChangedEvent(Id, mealId));
        }

        public MealItem UpdateMealItemQuantity(Guid mealId, Guid mealItemId, decimal quantity, MacroNutrients macros)
        {
            var meal = _meals.SingleOrDefault(x => x.Id == mealId)
                       ?? throw new DomainNotFoundException("Meal", mealId, "MealPlan", Id);

            var item = meal.UpdateItemQuantity(mealItemId, quantity, macros);

            AddDomainEvent(new MealItemChangedEvent(Id, mealId));

            return item;
        }
    }
}
