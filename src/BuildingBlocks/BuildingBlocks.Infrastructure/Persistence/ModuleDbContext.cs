using BuildingBlocks.Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.Infrastructure.Persistence
{
    public abstract class ModuleDbContext(DbContextOptions options) : DbContext(options)
    {
        /// <summary>
        /// Modülün varsayılan veritabanı şeması. Şema açıkça verilmeyen her tablo
        /// bu şemaya düşer; böylece yanlışlıkla <c>dbo</c> kullanımı engellenir.
        /// </summary>
        protected abstract string Schema { get; }

        /// <summary>
        /// Outbox tablosunun migration sahipliği. Yalnızca tek bir context (OutboxDbContext)
        /// tabloyu migration'da oluşturur; diğer tüm modül context'leri yalnızca yazma için
        /// mapping tutar ve tabloyu migration'dan hariç tutar (çakışmayı önler).
        /// </summary>
        protected virtual bool OwnsOutboxTable => false;

        public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(Schema);

            modelBuilder.ApplyConfiguration(
                new OutboxMessageConfiguration(excludeFromMigrations: !OwnsOutboxTable));

            base.OnModelCreating(modelBuilder);
        }
    }
}