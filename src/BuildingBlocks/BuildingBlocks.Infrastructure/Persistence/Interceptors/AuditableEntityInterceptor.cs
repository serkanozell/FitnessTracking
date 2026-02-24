using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BuildingBlocks.Infrastructure.Persistence.Interceptors
{
    public sealed class AuditableEntityInterceptor : SaveChangesInterceptor
    {
        private readonly ICurrentUser _currentUser;

        public AuditableEntityInterceptor(ICurrentUser currentUser)
        {
            _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
        }

        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            UpdateEntities(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            UpdateEntities(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void UpdateEntities(DbContext? context)
        {
            if (context == null) return;

            var now = DateTime.Now;
            var actor = GetCurrentActor();

            foreach (var entry in context.ChangeTracker.Entries<IEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedBy = entry.Entity.CreatedBy ?? actor;
                    entry.Entity.CreatedDate = entry.Entity.CreatedDate ?? now;
                    entry.Property(nameof(IEntity.IsActive)).CurrentValue = true;
                    entry.Property(nameof(IEntity.IsDeleted)).CurrentValue = false;
                }

                else if (entry.State == EntityState.Modified || entry.HasChangedOwnedEntities())
                {
                    entry.Entity.UpdatedBy = actor;
                    entry.Entity.UpdatedDate = now;
                }

                else if (entry.State == EntityState.Deleted)
                {
                    entry.State = EntityState.Modified;
                    entry.Property(nameof(IEntity.IsActive)).CurrentValue = false;
                    entry.Property(nameof(IEntity.IsDeleted)).CurrentValue = true;
                    entry.Entity.UpdatedBy = actor;
                    entry.Entity.UpdatedDate = now;
                }
            }
        }
        private string GetCurrentActor()
        {
            if (!_currentUser.IsAuthenticated) return "system";

            var id = _currentUser.UserId ?? "system";
            return id.Length <= 100 ? id : id[..100];
        }
    }

    public static class Extensions
    {
        public static bool HasChangedOwnedEntities(this EntityEntry entry) =>
            entry.References.Any(r =>
            r.TargetEntry != null &&
            r.TargetEntry.Metadata.IsOwned() &&
            (r.TargetEntry.State == EntityState.Added || r.TargetEntry.State == EntityState.Modified)
            );
    }
}