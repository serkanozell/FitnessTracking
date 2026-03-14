using Users.Application.Errors;

namespace Users.Application.Features.Users.RemoveRole
{
    internal sealed class RemoveRoleCommandHandler(
        IUserRepository _userRepository,
        IUsersUnitOfWork _unitOfWork) : ICommandHandler<RemoveRoleCommand, Result>
    {
        public async Task<Result> Handle(RemoveRoleCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

            if (user is null)
                return Result.Failure(UserErrors.NotFound(request.UserId));

            user.RemoveRole(request.RoleId);

            _userRepository.Update(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
