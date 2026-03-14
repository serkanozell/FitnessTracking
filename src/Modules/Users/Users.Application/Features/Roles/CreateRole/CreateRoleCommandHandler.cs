using Users.Application.Errors;
using Users.Domain.Entity;

namespace Users.Application.Features.Roles.CreateRole
{
    internal sealed class CreateRoleCommandHandler(
        IRoleRepository _roleRepository,
        IUsersUnitOfWork _unitOfWork) : ICommandHandler<CreateRoleCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            var existingRole = await _roleRepository.GetByNameAsync(request.Name, cancellationToken);

            if (existingRole is not null)
                return RoleErrors.DuplicateName(request.Name);

            var role = Role.Create(request.Name, request.Description);

            await _roleRepository.AddAsync(role, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(role.Id);
        }
    }
}
