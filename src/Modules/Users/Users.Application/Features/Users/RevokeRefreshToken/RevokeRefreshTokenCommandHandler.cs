using Users.Application.Errors;

namespace Users.Application.Features.Users.RevokeRefreshToken
{
    internal sealed class RevokeRefreshTokenCommandHandler(
        IRefreshTokenRepository _refreshTokenRepository,
        IUsersUnitOfWork _unitOfWork) : ICommandHandler<RevokeRefreshTokenCommand, Result<bool>>
    {
        public async Task<Result<bool>> Handle(RevokeRefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var refreshToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken, cancellationToken);

            if (refreshToken is null || !refreshToken.IsActive)
                return UserErrors.InvalidRefreshToken();

            refreshToken.Revoke();
            _refreshTokenRepository.Update(refreshToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
