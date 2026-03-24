using BuildingBlocks.Domain.Abstractions;

namespace Users.Domain.Entity
{
    public class UserRole : Entity<Guid>
    {
        public Guid UserId { get; private set; }
        public Guid RoleId { get; private set; }
        public Role Role { get; private set; }

        private UserRole() { }

        internal static UserRole Create(Guid userId, Guid roleId)
        {
            return new UserRole
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                RoleId = roleId,
            };
        }

        internal void Delete()
        {
            IsActive = false;
            IsDeleted = true;
        }

        internal void Restore()
        {
            IsActive = true;
            IsDeleted = false;
        }
    }
}