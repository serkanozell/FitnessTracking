using Users.Application.Dtos;

namespace Users.Application.Features.Roles.GetAllRoles
{
    internal sealed class GetAllRolesQueryHandler(
        IRoleRepository _roleRepository) : IQueryHandler<GetAllRolesQuery, Result<IReadOnlyList<RoleDto>>>
    {
        public async Task<Result<IReadOnlyList<RoleDto>>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
        {
            var roleDtos = await _roleRepository.GetAllAsync(RoleDto.Projection, cancellationToken);

            return Result<IReadOnlyList<RoleDto>>.Success(roleDtos);
        }
    }
}
