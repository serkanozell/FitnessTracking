using BuildingBlocks.Application.Abstractions;
using Users.Application.Errors;

namespace Users.Application.Features.Users.RefreshToken
{
    internal sealed class RefreshTokenCommandHandler(
        IRefreshTokenRepository _refreshTokenRepository,
        IUserRepository _userRepository,
        IUsersUnitOfWork _unitOfWork,
        ITokenService _tokenService) : ICommandHandler<RefreshTokenCommand, Result<RefreshTokenResponse>>
    {
        public async Task<Result<RefreshTokenResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var existingToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken, cancellationToken);

            if (existingToken is null || !existingToken.IsActive)
                return UserErrors.InvalidRefreshToken();

            var user = await _userRepository.GetByIdAsync(existingToken.UserId, cancellationToken);

            if (user is null || !user.IsActive)
                return UserErrors.InvalidRefreshToken();

            // Revoke the used refresh token
            existingToken.Revoke();
            _refreshTokenRepository.Update(existingToken);

            // Generate new tokens
            var roles = user.UserRoles
                .Where(ur => !ur.IsDeleted)
                .Select(ur => ur.Role.Name)
                .ToList();

            var accessToken = _tokenService.GenerateToken(user.Id, user.Email, roles);

            var newRefreshToken = Domain.Entity.RefreshToken.Create(user.Id, _tokenService.RefreshTokenExpirationDays);
            await _refreshTokenRepository.AddAsync(newRefreshToken, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new RefreshTokenResponse(accessToken, newRefreshToken.Token, user.Id, user.Email, roles);
        }
    }
}
