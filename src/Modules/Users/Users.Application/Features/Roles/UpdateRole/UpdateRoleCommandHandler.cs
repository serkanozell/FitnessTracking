using Users.Application.Errors;

namespace Users.Application.Features.Roles.UpdateRole
{
    internal sealed class UpdateRoleCommandHandler(
        IRoleRepository _roleRepository,
        IUsersUnitOfWork _unitOfWork) : ICommandHandler<UpdateRoleCommand, Result>
    {
        public async Task<Result> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            var role = await _roleRepository.GetByIdAsync(request.RoleId, cancellationToken);

            if (role is null)
                return Result.Failure(RoleErrors.NotFound(request.RoleId));

            var existingRole = await _roleRepository.GetByNameAsync(request.Name, cancellationToken);

            if (existingRole is not null && existingRole.Id != request.RoleId)
                return Result.Failure(RoleErrors.DuplicateName(request.Name));

            role.Update(request.Name, request.Description);

            _roleRepository.Update(role);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
