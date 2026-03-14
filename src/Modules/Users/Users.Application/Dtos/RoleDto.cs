using Users.Domain.Entity;

namespace Users.Application.Dtos
{
    public sealed class RoleDto
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = default!;
        public string? Description { get; init; }
        public bool IsActive { get; init; }
        public DateTime? CreatedDate { get; init; }

        public static RoleDto FromEntity(Role entity) =>
            new()
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                IsActive = entity.IsActive,
                CreatedDate = entity.CreatedDate
            };
    }
}
