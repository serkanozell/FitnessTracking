namespace Users.Application.Errors
{
    public static class UserErrors
    {
        public static Error NotFound(Guid id) => new("User.NotFound", $"User with ID '{id}' was not found.");

        public static Error NotFoundByEmail(string email) => new("User.NotFoundByEmail", $"User with email '{email}' was not found.");

        public static Error DuplicateEmail(string email) => new("User.DuplicateEmail", $"User with email '{email}' already exists.");

        public static Error InvalidCredentials() => new("User.InvalidCredentials", "Invalid email or password.");

        public static Error AlreadyActive(Guid id) => new("User.AlreadyActive", $"User with ID '{id}' is already active.");

        public static Error AlreadyDeleted(Guid id) => new("User.AlreadyDeleted", $"User with ID '{id}' is already deleted.");
    }
}
