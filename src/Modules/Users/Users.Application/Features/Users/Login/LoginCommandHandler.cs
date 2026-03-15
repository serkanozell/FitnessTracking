using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Security;
using Users.Application.Errors;

namespace Users.Application.Features.Users.Login
{
    internal sealed class LoginCommandHandler(IUserRepository _userRepository,
                                              IRefreshTokenRepository _refreshTokenRepository,
                                              IUsersUnitOfWork _unitOfWork,
                                              IPasswordHasher _passwordHasher,
                                              ITokenService _tokenService) : ICommandHandler<LoginCommand, Result<LoginResponse>>
    {
        public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);

            if (user is null)
                return UserErrors.InvalidCredentials();

            if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
                return UserErrors.InvalidCredentials();

            var roles = user.UserRoles
                .Where(ur => !ur.IsDeleted)
                .Select(ur => ur.Role.Name)
                .ToList();

            var accessToken = _tokenService.GenerateToken(user.Id, user.Email, roles);

            // Revoke existing active refresh tokens
            var activeTokens = await _refreshTokenRepository.GetActiveByUserIdAsync(user.Id, cancellationToken);
            foreach (var activeToken in activeTokens)
            {
                activeToken.Revoke();
                _refreshTokenRepository.Update(activeToken);
            }

            // Create new refresh token
            var refreshToken = Domain.Entity.RefreshToken.Create(user.Id, _tokenService.RefreshTokenExpirationDays);
            await _refreshTokenRepository.AddAsync(refreshToken, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new LoginResponse(accessToken, refreshToken.Token, user.Id, user.Email, roles);
        }
    }
}