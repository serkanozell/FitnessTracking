namespace Users.Application.Features.Users.ActivateUser
{
    public sealed record ActivateUserCommand(Guid UserId) : ICommand<Result>;
}
