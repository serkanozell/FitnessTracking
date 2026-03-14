using Users.Application.Errors;

namespace Users.Application.Features.Users.ActivateUser
{
    internal sealed class ActivateUserCommandHandler(
        IUserRepository _userRepository,
        IUsersUnitOfWork _unitOfWork) : ICommandHandler<ActivateUserCommand, Result>
    {
        public async Task<Result> Handle(ActivateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

            if (user is null)
                return Result.Failure(UserErrors.NotFound(request.UserId));

            if (user.IsActive && !user.IsDeleted)
                return Result.Failure(UserErrors.AlreadyActive(request.UserId));

            user.Activate();

            _userRepository.Update(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
