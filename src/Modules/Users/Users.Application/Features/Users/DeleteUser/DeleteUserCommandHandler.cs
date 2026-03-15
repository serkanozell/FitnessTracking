using BuildingBlocks.Application.Abstractions;
using Users.Application.Errors;

namespace Users.Application.Features.Users.DeleteUser
{
    internal sealed class DeleteUserCommandHandler(
        IUserRepository _userRepository,
        IRefreshTokenRepository _refreshTokenRepository,
        IUsersUnitOfWork _unitOfWork,
        ICurrentUser _currentUser) : ICommandHandler<DeleteUserCommand, Result>
    {
        public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

            if (user is null)
                return Result.Failure(UserErrors.NotFound(request.UserId));

            if (user.IsDeleted)
                return Result.Failure(UserErrors.AlreadyDeleted(request.UserId));

            user.Delete(_currentUser.UserId ?? "system");

            // Revoke all active refresh tokens
            var activeTokens = await _refreshTokenRepository.GetActiveByUserIdAsync(user.Id, cancellationToken);
            foreach (var token in activeTokens)
            {
                token.Revoke();
                _refreshTokenRepository.Update(token);
            }

            _userRepository.Update(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}