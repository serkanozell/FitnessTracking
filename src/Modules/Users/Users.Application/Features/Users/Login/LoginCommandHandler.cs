using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Security;
using Users.Application.Errors;

namespace Users.Application.Features.Users.Login
{
    internal sealed class LoginCommandHandler(
        IUserRepository _userRepository,
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

            var token = _tokenService.GenerateToken(user.Id, user.Email, roles);

            return new LoginResponse(token, user.Id, user.Email, roles);
        }
    }
}
