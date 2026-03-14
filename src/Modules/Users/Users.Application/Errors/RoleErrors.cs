namespace Users.Application.Errors
{
    public static class RoleErrors
    {
        public static Error NotFound(Guid id) => new("Role.NotFound", $"Role with ID '{id}' was not found.");

        public static Error DuplicateName(string name) => new("Role.DuplicateName", $"Role with name '{name}' already exists.");
    }
}
