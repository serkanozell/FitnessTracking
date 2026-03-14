namespace Users.Application.Features.Users.RemoveRole
{
    public sealed class RemoveRoleCommandValidator : AbstractValidator<RemoveRoleCommand>
    {
        public RemoveRoleCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty();

            RuleFor(x => x.RoleId)
                .NotEmpty();
        }
    }
}
