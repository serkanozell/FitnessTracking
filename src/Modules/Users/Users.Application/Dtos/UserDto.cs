using Users.Domain.Entity;

namespace Users.Application.Dtos
{
    public sealed class UserDto
    {
        public Guid Id { get; init; }
        public string Email { get; init; } = default!;
        public string FirstName { get; init; } = default!;
        public string LastName { get; init; } = default!;
        public string Status { get; init; } = default!;
        public bool IsActive { get; init; }
        public List<string> Roles { get; init; } = new();
        public DateTime? CreatedDate { get; init; }

        public static UserDto FromEntity(User entity) =>
            new()
            {
                Id = entity.Id,
                Email = entity.Email,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                Status = entity.Status.ToString(),
                IsActive = entity.IsActive,
                Roles = entity.UserRoles
                    .Where(ur => !ur.IsDeleted)
                    .Select(ur => ur.Role.Name)
                    .ToList(),
                CreatedDate = entity.CreatedDate
            };
    }
}
