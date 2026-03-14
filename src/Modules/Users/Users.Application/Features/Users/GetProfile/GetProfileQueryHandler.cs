using Users.Application.Dtos;
using Users.Application.Errors;

namespace Users.Application.Features.Users.GetProfile
{
    internal sealed class GetProfileQueryHandler(
        IUserRepository _userRepository) : IQueryHandler<GetProfileQuery, Result<UserDto>>
    {
        public async Task<Result<UserDto>> Handle(GetProfileQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

            if (user is null)
                return UserErrors.NotFound(request.UserId);

            return UserDto.FromEntity(user);
        }
    }
}
