using BuildingBlocks.Domain.Security;
using Users.Application.Errors;
using Users.Domain.Entity;

namespace Users.Application.Features.Users.Register
{
    internal sealed class RegisterCommandHandler(
        IUserRepository _userRepository,
        IUsersUnitOfWork _unitOfWork,
        IPasswordHasher _passwordHasher) : ICommandHandler<RegisterCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            if (await _userRepository.ExistsByEmailAsync(request.Email, cancellationToken))
                return UserErrors.DuplicateEmail(request.Email);

            var passwordHash = _passwordHasher.Hash(request.Password);

            var user = User.Create(request.Email, passwordHash, request.FirstName, request.LastName);

            await _userRepository.AddAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(user.Id);
        }
    }
}
