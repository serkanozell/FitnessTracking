using BuildingBlocks.Domain.Abstractions;
using BuildingBlocks.Domain.Exceptions;
using Users.Domain.Enums;
using Users.Domain.Events;

namespace Users.Domain.Entity
{
    public class User : AggregateRoot<Guid>
    {
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public UserStatus Status { get; private set; }

        private readonly List<UserRole> _userRoles = new();
        public IReadOnlyList<UserRole> UserRoles => _userRoles.AsReadOnly();

        private User() { }

        public static User Create(string email, string passwordHash, string firstName, string lastName)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = email,
                PasswordHash = passwordHash,
                FirstName = firstName,
                LastName = lastName,
                Status = UserStatus.Active,
            };

            user.AddDomainEvent(new UserRegisteredEvent(user.Id, user.Email));

            return user;
        }

        public void UpdateProfile(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;

            AddDomainEvent(new UserProfileUpdatedEvent(Id));
        }

        public void ChangePassword(string newPasswordHash)
        {
            PasswordHash = newPasswordHash;

            AddDomainEvent(new UserPasswordChangedEvent(Id));
        }

        public UserRole AssignRole(Guid roleId)
        {
            var existing = _userRoles.SingleOrDefault(ur => ur.RoleId == roleId);

            if (existing is not null)
            {
                if (!existing.IsDeleted)
                    throw new BusinessRuleViolationException($"User already has role '{roleId}'.");

                // Reactivate the soft-deleted role assignment
                existing.Restore();
                AddDomainEvent(new UserRoleAssignedEvent(Id, roleId));
                return existing;
            }

            var userRole = UserRole.Create(Id, roleId);
            _userRoles.Add(userRole);

            AddDomainEvent(new UserRoleAssignedEvent(Id, roleId));
            return userRole;
        }

        public void RemoveRole(Guid roleId)
        {
            var userRole = _userRoles.SingleOrDefault(ur => ur.RoleId == roleId && !ur.IsDeleted)
                ?? throw new DomainNotFoundException("UserRole", roleId, "User", Id);

            _userRoles.Remove(userRole);

            AddDomainEvent(new UserRoleRemovedEvent(Id, roleId));
        }

        public void Activate()
        {
            IsActive = true;
            IsDeleted = false;

            AddDomainEvent(new UserActivatedEvent(Id));
        }

        public void Delete(string deletedBy)
        {
            IsActive = false;
            IsDeleted = true;

            // Clear user roles
            foreach (var userRole in _userRoles)
            {
                userRole.Delete();
            }

            AddDomainEvent(new UserDeletedEvent(Id, deletedBy));
        }
    }
}