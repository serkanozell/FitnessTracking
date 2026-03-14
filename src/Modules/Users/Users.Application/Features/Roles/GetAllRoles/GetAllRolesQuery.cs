using Users.Application.Dtos;

namespace Users.Application.Features.Roles.GetAllRoles
{
    public sealed record GetAllRolesQuery() : IQuery<Result<IReadOnlyList<RoleDto>>>;
}
