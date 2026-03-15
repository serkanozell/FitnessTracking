using BuildingBlocks.Domain.Security;
using Users.Application.Errors;
using Users.Domain.Constants;
using Users.Domain.Entity;

namespace Users.Application.Features.Users.CreateUser
{
    internal sealed class CreateUserCommandHandler(
        IUserRepository _userRepository,
        IRoleRepository _roleRepository,
        IUsersUnitOfWork _unitOfWork,
        IPasswordHasher _passwordHasher) : ICommandHandler<CreateUserCommand, Result<Guid>>
    {
        public async Task<Result<Guid>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            if (await _userRepository.ExistsByEmailAsync(request.Email, cancellationToken))
                return UserErrors.DuplicateEmail(request.Email);

            // Validate all role IDs exist
            foreach (var roleId in request.RoleIds)
            {
                if (!await _roleRepository.ExistsAsync(roleId, cancellationToken))
                    return UserErrors.RoleNotFound(roleId);
            }

            var passwordHash = _passwordHasher.Hash(request.Password);

            var user = User.Create(request.Email, passwordHash, request.FirstName, request.LastName);

            // Assign specified roles; if none provided, assign Member by default
            var roleIds = request.RoleIds.Count > 0
                ? request.RoleIds
                : [RoleConstants.MemberRoleId];

            foreach (var roleId in roleIds)
            {
                user.AssignRole(roleId);
            }

            await _userRepository.AddAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(user.Id);
        }
    }
}