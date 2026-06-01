-- =============================================================================
-- OUTBOX BASELINE SCRIPT (yalnızca MEVCUT veritabanları için)
-- =============================================================================
-- Bağlam:
--   OutboxMessages tablosu, Outbox sahipliği OutboxDbContext'e taşınmadan önce
--   Exercises modülünün "20260406203132_exercise-full-table" migration'ı
--   tarafından oluşturuluyordu. Artık tablonun tek migration sahibi
--   OutboxDbContext ("20260601200857_InitOutboxModule") olmuştur.
--
-- Sorun:
--   Mevcut (üretim/geliştirme) veritabanlarında [outbox].[OutboxMessages]
--   tablosu ZATEN VAR, ancak [outbox].[__EFMigrationsHistory] içinde
--   InitOutboxModule kaydı YOK. Uygulama açıldığında / migrate edildiğinde
--   InitOutboxModule "CREATE TABLE" çalıştırmaya çalışır ve
--   "There is already an object named 'OutboxMessages'" hatası verir.
--
-- Çözüm:
--   Bu script tabloyu YENİDEN OLUŞTURMAZ. Yalnızca InitOutboxModule
--   migration'ını "uygulanmış" olarak işaretler (baseline).
--
-- Kullanım:
--   Bu script'i SADECE outbox tablosu zaten oluşturulmuş mevcut
--   veritabanlarında BİR KEZ çalıştırın. Yeni/boş veritabanlarında
--   çalıştırmayın; orada normal migration (InitOutboxModule) tabloyu
--   oluşturmalıdır.
-- =============================================================================

IF OBJECT_ID(N'[outbox].[__EFMigrationsHistory]') IS NULL
BEGIN
    IF SCHEMA_ID(N'outbox') IS NULL EXEC(N'CREATE SCHEMA [outbox];');
    CREATE TABLE [outbox].[__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

-- InitOutboxModule'u baseline olarak işaretle (tablo halihazırda var).
IF NOT EXISTS (
    SELECT 1 FROM [outbox].[__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260601200857_InitOutboxModule'
)
BEGIN
    INSERT INTO [outbox].[__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260601200857_InitOutboxModule', N'10.0.5');
END;
GO
