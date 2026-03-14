namespace Users.Application.Features.Users.UpdateProfile
{
    public sealed record UpdateProfileCommand(Guid UserId, string FirstName, string LastName) : ICommand<Result>;
}
