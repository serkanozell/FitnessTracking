# FitnessTracking

.NET 10 tabanlı **Modular Monolith** mimarisiyle geliştirilmiş fitness takip API'si.

## Teknoloji Yığını

| Katman | Teknoloji |
|--------|-----------|
| Framework | .NET 10, Minimal API |
| Mimari | Modular Monolith, CQRS, Vertical Slice |
| Veritabanı | SQL Server, EF Core 10 |
| Mesajlaşma | Outbox Pattern (retry + dead letter) |
| Cache | Redis (cache-aside) |
| Loglama | Serilog (Console + File) |
| Doğrulama | FluentValidation |
| API Versioning | Asp.Versioning.Http (URL segment: `/api/v1/...`) |
| API Dokümantasyon | Scalar (`/scalar/v1`) |
| Test | xUnit, FluentAssertions, NSubstitute, Testcontainers, NetArchTest |

## Modüller

- **Exercises** — Egzersiz tanımları (kas grubu, açıklama)
- **WorkoutPrograms** — Antrenman programları, split'ler ve split egzersizleri
- **WorkoutSessions** — Antrenman seansları ve seans egzersizleri

## Başlangıç

### Gereksinimler

- .NET 10 SDK
- SQL Server
- Redis
- Docker Desktop (integration testleri için)

### Çalıştırma

```bash
dotnet run --project src/FitnessTracking.Api
```

Uygulama başladığında Scalar API dokümanı otomatik açılır: `https://localhost:7211/scalar/v1`

### Testler

```bash
dotnet test
```

> Integration testleri Docker üzerinde SQL Server container'ı başlatır (Testcontainers).

## Dokümantasyon

Detaylı mimari kılavuz için: [`docs/ARCHITECTURE.md`](docs/ARCHITECTURE.md)