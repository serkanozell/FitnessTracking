namespace FitnessTracking.Mvc.Models;

// ── Food ──

public sealed class FoodDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = default!;
    public string Category { get; init; } = default!;
    public decimal DefaultServingSize { get; init; }
    public string ServingUnit { get; init; } = default!;
    public decimal Calories { get; init; }
    public decimal Protein { get; init; }
    public decimal Carbohydrates { get; init; }
    public decimal Fat { get; init; }
    public decimal? Fiber { get; init; }
    public Guid? UserId { get; init; }
    public bool IsActive { get; init; }
    public bool IsDeleted { get; init; }
    public DateTime? CreatedDate { get; init; }
}

public sealed class FoodEditModel
{
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = "Protein";
    public decimal DefaultServingSize { get; set; } = 100;
    public string ServingUnit { get; set; } = "Gram";
    public decimal Calories { get; set; }
    public decimal Protein { get; set; }
    public decimal Carbohydrates { get; set; }
    public decimal Fat { get; set; }
    public decimal? Fiber { get; set; }
}

// ── MealPlan ──

public sealed class MealPlanDto
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public string Name { get; init; } = default!;
    public DateTime Date { get; init; }
    public string? Note { get; init; }
    public decimal TotalCalories { get; init; }
    public decimal TotalProtein { get; init; }
    public decimal TotalCarbohydrates { get; init; }
    public decimal TotalFat { get; init; }
    public bool IsActive { get; init; }
    public bool IsDeleted { get; init; }
    public DateTime? CreatedDate { get; init; }
    public IReadOnlyList<MealDto> Meals { get; init; } = [];
}

public sealed class MealDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = default!;
    public int Order { get; init; }
    public decimal MealCalories { get; init; }
    public decimal MealProtein { get; init; }
    public decimal MealCarbohydrates { get; init; }
    public decimal MealFat { get; init; }
    public IReadOnlyList<MealItemDto> Items { get; init; } = [];
}

public sealed class MealItemDto
{
    public Guid Id { get; init; }
    public Guid FoodId { get; init; }
    public string FoodName { get; init; } = default!;
    public decimal Quantity { get; init; }
    public string ServingUnit { get; init; } = default!;
    public decimal Calories { get; init; }
    public decimal Protein { get; init; }
    public decimal Carbohydrates { get; init; }
    public decimal Fat { get; init; }
}

public sealed class MealPlanEditModel
{
    public string Name { get; set; } = string.Empty;
    public DateTime Date { get; set; } = DateTime.Today;
    public string? Note { get; set; }
}

// ── API Responses ──

public sealed record CreateFoodResponse(Guid Id);
public sealed record CreateMealPlanResponse(Guid Id);
public sealed record AddMealResponse(Guid MealId);
public sealed record AddMealItemResponse(Guid MealItemId);
