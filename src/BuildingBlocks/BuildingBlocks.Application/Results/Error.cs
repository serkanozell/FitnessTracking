namespace BuildingBlocks.Application.Results
{
    public sealed record Error(string Code, string Message)
    {
        // Predefined Errors
        public static readonly Error None = new(string.Empty, string.Empty);
        public static readonly Error NullValue = new("Error.NullValue", "A null value was provided.");

        // Factory Methods
        public static Error NotFound(string entity, object id) =>
            new($"{entity}.NotFound", $"{entity} with ID '{id}' was not found.");

        public static Error Validation(string message) =>
            new("Error.Validation", message);

        public static Error Conflict(string message) =>
            new("Error.Conflict", message);
    }
}