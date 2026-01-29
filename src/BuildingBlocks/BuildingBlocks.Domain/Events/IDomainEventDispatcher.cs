using Microsoft.EntityFrameworkCore;

public interface IDomainEventDispatcher
{
    Task DispatchDomainEvents(DbContext context);
}