using BuildingBlocks.Domain.Security;
using Users.Application.Errors;

namespace Users.Application.Features.Users.ChangePassword
{
    internal sealed class ChangePasswordCommandHandler(
        IUserRepository _userRepository,
        IUsersUnitOfWork _unitOfWork,
        IPasswordHasher _passwordHasher) : ICommandHandler<ChangePasswordCommand, Result>
    {
        public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

            if (user is null)
                return Result.Failure(UserErrors.NotFound(request.UserId));

            if (!_passwordHasher.Verify(request.CurrentPassword, user.PasswordHash))
                return Result.Failure(UserErrors.InvalidCredentials());

            var newHash = _passwordHasher.Hash(request.NewPassword);

            user.ChangePassword(newHash);

            _userRepository.Update(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
