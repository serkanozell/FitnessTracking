using BuildingBlocks.Domain.Abstractions;
using Nutrition.Domain.Enums;
using Nutrition.Domain.Events;
using Nutrition.Domain.ValueObjects;

namespace Nutrition.Domain.Entity
{
    public class Food : AggregateRoot<Guid>
    {
        public Guid? UserId { get; private set; }
        public string Name { get; private set; }
        public FoodCategory Category { get; private set; }
        public decimal DefaultServingSize { get; private set; }
        public ServingUnit ServingUnit { get; private set; }
        public MacroNutrients Macros { get; private set; }
        public decimal? Fiber { get; private set; }        

        private Food() { }

        public static Food Create(
            string name,
            FoodCategory category,
            decimal defaultServingSize,
            ServingUnit servingUnit,
            decimal calories,
            decimal protein,
            decimal carbohydrates,
            decimal fat,
            decimal? fiber,
            Guid? userId)
        {
            var food = new Food
            {
                Id = Guid.NewGuid(),
                Name = name,
                Category = category,
                DefaultServingSize = defaultServingSize,
                ServingUnit = servingUnit,
                Macros = new MacroNutrients(calories, protein, carbohydrates, fat),
                Fiber = fiber,
                UserId = userId
            };

            food.AddDomainEvent(new FoodCreatedEvent(food.Id));

            return food;
        }

        public void Update(
            string name,
            FoodCategory category,
            decimal defaultServingSize,
            ServingUnit servingUnit,
            decimal calories,
            decimal protein,
            decimal carbohydrates,
            decimal fat,
            decimal? fiber)
        {
            Name = name;
            Category = category;
            DefaultServingSize = defaultServingSize;
            ServingUnit = servingUnit;
            Macros = new MacroNutrients(calories, protein, carbohydrates, fat);
            Fiber = fiber;

            AddDomainEvent(new FoodUpdatedEvent(Id));
        }

        public void Activate()
        {
            IsActive = true;
            IsDeleted = false;

            AddDomainEvent(new FoodActivatedEvent(Id));
        }

        public void Delete()
        {
            IsActive = false;
            IsDeleted = true;

            AddDomainEvent(new FoodDeletedEvent(Id));
        }
    }
}
