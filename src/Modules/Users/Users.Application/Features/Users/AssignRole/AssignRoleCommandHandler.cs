using Users.Application.Errors;

namespace Users.Application.Features.Users.AssignRole
{
    internal sealed class AssignRoleCommandHandler(
        IUserRepository _userRepository,
        IRoleRepository _roleRepository,
        IUsersUnitOfWork _unitOfWork) : ICommandHandler<AssignRoleCommand, Result>
    {
        public async Task<Result> Handle(AssignRoleCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

            if (user is null)
                return Result.Failure(UserErrors.NotFound(request.UserId));

            if (!await _roleRepository.ExistsAsync(request.RoleId, cancellationToken))
                return Result.Failure(RoleErrors.NotFound(request.RoleId));

            user.AssignRole(request.RoleId);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}