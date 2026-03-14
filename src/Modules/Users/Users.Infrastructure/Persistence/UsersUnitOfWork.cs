using BuildingBlocks.Infrastructure.Persistence;
using Users.Domain.Repositories;

namespace Users.Infrastructure.Persistence
{
    public sealed class UsersUnitOfWork(UsersDbContext context) : UnitOfWork<UsersDbContext>(context), IUsersUnitOfWork;
}
