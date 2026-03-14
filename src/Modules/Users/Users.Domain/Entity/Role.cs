using BuildingBlocks.Domain.Abstractions;
using Users.Domain.Events;

namespace Users.Domain.Entity
{
    public class Role : AggregateRoot<Guid>
    {
        public string Name { get; private set; }
        public string? Description { get; private set; }

        private Role() { }

        public static Role Create(string name, string? description)
        {
            var role = new Role
            {
                Id = Guid.NewGuid(),
                Name = name,
                Description = description,
            };

            role.AddDomainEvent(new RoleCreatedEvent(role.Id, role.Name));

            return role;
        }

        public void Update(string name, string? description)
        {
            Name = name;
            Description = description;

            AddDomainEvent(new RoleUpdatedEvent(Id));
        }

        public void Activate()
        {
            IsActive = true;
            IsDeleted = false;

            AddDomainEvent(new RoleActivatedEvent(Id));
        }

        public void Delete()
        {
            IsActive = false;
            IsDeleted = true;

            AddDomainEvent(new RoleDeletedEvent(Id));
        }
    }
}
