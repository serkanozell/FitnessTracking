namespace Users.Application.Features.Users.DeleteUser
{
    public sealed record DeleteUserCommand(Guid UserId) : ICommand<Result>;
}
