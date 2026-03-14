using Users.Application.Dtos;

namespace Users.Application.Features.Roles.GetAllRoles
{
    internal sealed class GetAllRolesQueryHandler(
        IRoleRepository _roleRepository) : IQueryHandler<GetAllRolesQuery, Result<IReadOnlyList<RoleDto>>>
    {
        public async Task<Result<IReadOnlyList<RoleDto>>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
        {
            var roles = await _roleRepository.GetAllAsync(cancellationToken);

            var roleDtos = roles.Select(RoleDto.FromEntity).ToList() as IReadOnlyList<RoleDto>;

            return Result<IReadOnlyList<RoleDto>>.Success(roleDtos);
        }
    }
}
