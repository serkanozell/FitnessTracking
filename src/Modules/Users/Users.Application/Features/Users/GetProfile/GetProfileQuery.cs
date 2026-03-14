using Users.Application.Dtos;

namespace Users.Application.Features.Users.GetProfile
{
    public sealed record GetProfileQuery(Guid UserId) : IQuery<Result<UserDto>>;
}
