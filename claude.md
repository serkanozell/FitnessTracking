# Claude Instructions — FitnessTracking

> Bu dosya, Claude AI'ın bu projede tutarlı ve mimariye uygun kod üretmesi için referans kuralları içerir.
> Detaylı mimari dokümantasyon için `docs/ARCHITECTURE.md` dosyasına başvur.

---

## Proje Özeti

- **FitnessTracking** — .NET 10 tabanlı **Modular Monolith** fitness takip uygulaması.
- **API**: `FitnessTracking.Api` (host, Minimal API)
- **Web**: `FitnessTracking.Web` (Blazor WebAssembly)
- **Modüller**: Exercises, WorkoutPrograms, WorkoutSessions
- **BuildingBlocks**: Paylaşılan altyapı, domain ve application yapı taşları

---

## Modül Yapısı

Her modül 4+1 katmandan oluşur:

```
src/Modules/{ModuleName}/
├── {ModuleName}.Domain
├── {ModuleName}.Application
├── {ModuleName}.Infrastructure
├── {ModuleName}.Api
└── {ModuleName}.Contracts       (opsiyonel)
```

---

## Katman Bağımlılık Kuralları

```
Domain ← Application ← Infrastructure ← Api
```

- **Domain** hiçbir katmana bağımlı değildir.
- **Application** yalnızca **Domain**'e bağımlıdır.
- **Infrastructure** hem **Domain** hem **Application**'a bağımlıdır.
- **Api** tüm katmanlara bağımlıdır (composition root).
- Modüller arası doğrudan bağımlılık **yasaktır**. İletişim **Contracts** projesi üzerinden yapılır.

> Bu kurallar `tests/ArchitectureTests/LayerDependencyTests.cs` ile NetArchTest kullanılarak otomatik doğrulanır.

---

## Domain Kuralları

### Entity & Aggregate Root

- Entity'ler `Entity<TId>`, aggregate root'lar `AggregateRoot<TId>` base class'ından türer.
- Property'ler **`private set`** kullanır.
- Entity oluşturma **static factory method** (`Create`) ile yapılır.
- Her entity'nin **parametresiz private constructor**'ı vardır (EF Core materialization için).
- Her entity'de `Id`, `IsActive`, `IsDeleted`, audit alanları (`CreatedDate`, `CreatedBy`, `UpdatedDate`, `UpdatedBy`) ve `RowVersion` bulunur.

```csharp
// ✅ Doğru
public static Exercise Create(string name, ...) { ... }

// ❌ Yanlış
var exercise = new Exercise(Guid.NewGuid(), name, ...);
```

### Domain Event Kuralları (KRİTİK)

- **Domain event'ler her zaman entity/aggregate root domain metodları içinden raise edilmelidir.**
- `AddDomainEvent()` çağrısı **yalnızca** domain entity metodları içinde yapılır.
- Application handler'larında **asla** `AddDomainEvent` çağrılmaz.
- Child entity operasyonları için **aggregate root üzerinde wrapper domain metodları** oluşturulur.

```csharp
// ✅ Doğru — Aggregate root üzerinden child operasyonu
public WorkoutSplitExercise AddExerciseToSplit(Guid splitId, Guid exerciseId, ...)
{
    var split = Splits.SingleOrDefault(x => x.Id == splitId)
                ?? throw new DomainNotFoundException(...);
    var exercise = split.AddExercise(exerciseId, ...);
    AddDomainEvent(new SplitExerciseChangedEvent(Id, splitId));
    return exercise;
}

// ❌ Yanlış — Handler'da doğrudan child entity üzerinde işlem
// split.AddExercise(exerciseId, ...);  // Event raise edilmez!
```

### Domain Event Tanımı

- `IDomainEvent` implement eden **sealed record** olarak tanımlanır.
- `{ModuleName}.Domain/Events/` klasöründe tutulur.

```csharp
public sealed record ExerciseCreatedEvent(Guid ExerciseId) : IDomainEvent
{
    public DateTime OccurredOn => DateTime.Now;
}
```

### Domain Exception'lar

| Exception | Kullanım |
|---|---|
| `DomainNotFoundException` | Entity bulunamadığında |
| `BusinessRuleViolationException` | İş kuralı ihlalinde |
| `DomainException` | Genel domain hataları (base class) |

### Repository Interface'leri

- Repository interface'leri **Domain katmanında** tanımlanır (`{ModuleName}.Domain/Repositories/`).
- Her modül kendi `I{ModuleName}UnitOfWork` interface'ine sahiptir.

---

## Application Kuralları

### CQRS Pattern

- **Command**: `ICommand<TResponse>` — `sealed record`
- **Query**: `IQuery<TResponse>` — `sealed record`
- **Handler**: `ICommandHandler` veya `IQueryHandler` — `internal sealed class`
- Tüm handler'lar `Result<T>` veya `Result` döner.

```csharp
public sealed record CreateExerciseCommand(string Name, ...) : ICommand<Result<Guid>>;

internal sealed class CreateExerciseCommandHandler(...)
    : ICommandHandler<CreateExerciseCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(...) { ... }
}
```

### Result Pattern

- Başarı: `Result<T>.Success(data)`
- Hata: `Result<T>.Failure(error)` — exception fırlatmak yerine `Result.Failure` tercih edilir (domain exception'lar hariç).

### Error Tanımları

- `{ModuleName}.Application/Errors/{Entity}Errors.cs` — static method olarak tanımlanır.

```csharp
public static class ExerciseErrors
{
    public static Error NotFound(Guid id) => new("Exercise.NotFound", $"...");
}
```

### Validation

- Her command için **FluentValidation** validator'ı oluşturulur (`public sealed class`).
- Feature klasöründe command ile birlikte tutulur.
- `ValidationBehavior` pipeline davranışı ile otomatik çalışır.

### Endpoint'ler (Vertical Slice)

- `IEndpoint` interface'ini implement eder.
- Minimal API kullanır (`MapGet`, `MapPost`, `MapPut`, `MapDelete`).
- Request/Response DTO'ları endpoint içinde **nested sealed record** olarak tanımlanır.
- `WithName`, `WithTags`, `WithSummary`, `WithDescription`, `Accepts`, `Produces`, `ProducesProblem` kullanılır.

### Feature Klasör Yapısı

```
Features/{Entity}/{FeatureName}/
├── {FeatureName}Command.cs          (veya Query)
├── {FeatureName}CommandHandler.cs   (veya QueryHandler)
├── {FeatureName}CommandValidator.cs
└── {FeatureName}Endpoint.cs
```

### Caching

- Cache'lenecek query'ler `ICacheableQuery` implement eder.
- Cache invalidation domain event handler'ları (`IDomainEventHandler<TEvent>`) ile yapılır.
- Cache key formatı: `{entity}:{id}` veya `{entity}:all`

---

## Infrastructure Kuralları

- Her modülün kendi `{ModuleName}DbContext`'i vardır.
- Entity config'leri `IEntityTypeConfiguration<T>` ile ayrı dosyalarda tanımlanır.
- `OutboxInterceptor` domain event'leri Outbox tablosuna yazar.
- `AuditableEntityInterceptor` audit alanlarını otomatik doldurur.
- Optimistic concurrency: `RowVersion` property'si ile.

---

## API Katmanı

- Her modül `IModule` implement eder (DI kayıt + endpoint mapping).
- MediatR pipeline sırası: **ValidationBehavior → LoggingBehavior → CachingBehavior**
- `GlobalExceptionHandler` tüm exception'ları `ProblemDetails` formatında döner:

| Exception | HTTP Status |
|---|---|
| `ValidationException` | 400 Bad Request |
| `DomainNotFoundException` | 404 Not Found |
| `BusinessRuleViolationException` | 409 Conflict |
| `DbUpdateConcurrencyException` | 409 Conflict |
| `DomainException` | 422 Unprocessable Entity |
| Diğer | 500 Internal Server Error |

### Cross-Cutting Concerns

- **CORS**: Blazor client için `BlazorClient` policy
- **Rate Limiting**: IP tabanlı fixed window (20 request/dakika, 5 queue)
- **Health Checks**: SQL Server, Redis, SMTP (`/health`, `/health/ready`, `/health/live`)
- **Logging**: Serilog (configuration-based)

---

## Blazor WebAssembly (FitnessTracking.Web)

- Standalone Blazor WebAssembly uygulaması.
- API'ye `HttpClient` tabanlı servisler üzerinden erişir.
- Servisler: `IExercisesService`, `IWorkoutProgramsService`, `IWorkoutSessionsService`
- Model'ler ve DTO'lar Web projesinde ayrı tanımlanır (API DTO'larından bağımsız).

---

## Test Stratejisi

```
tests/
├── ArchitectureTests/
├── BuildingBlocks/
│   ├── BuildingBlocks.Application.UnitTests/
│   └── BuildingBlocks.Infrastructure.UnitTests/
├── Modules/
│   ├── Exercises/          (Domain.UnitTests, Application.UnitTests, Infrastructure.IntegrationTests)
│   ├── WorkoutPrograms/    (Domain.UnitTests, Application.UnitTests, Infrastructure.IntegrationTests)
│   └── WorkoutSessions/    (Domain.UnitTests, Application.UnitTests, Infrastructure.IntegrationTests)
└── FitnessTracking.Api.IntegrationTests/
```

- **Domain Unit Tests**: Entity business logic, domain event raise kontrolü
- **Application Unit Tests**: Handler logic, validation rule testleri
- **Infrastructure Integration Tests**: Repository + DB testleri
- **API Integration Tests**: End-to-end endpoint testleri
- **Architecture Tests**: Katman bağımlılık, naming convention, class design kuralları

---

## Yeni Modül Ekleme Kontrol Listesi

1. **Domain** projesi oluştur (Entity, Event, Repository interface, Enum)
2. **Application** projesi oluştur (`GlobalUsings.cs`, `AssemblyReference.cs`, Features, DTOs, Errors)
3. **Infrastructure** projesi oluştur (DbContext, Repository impl, UnitOfWork, Config, Migration, DI extension)
4. **Api** projesi oluştur (`IModule` implementasyonu)
5. **Contracts** projesi oluştur (opsiyonel)
6. `Program.cs`'e modülü kayıt et
7. Test projeleri oluştur
8. Architecture test'lere ekle

## Yeni Feature Ekleme Kontrol Listesi

1. Gerekiyorsa Domain entity'ye yeni domain metodu ekle (event raise dahil)
2. Gerekiyorsa yeni Domain Event oluştur
3. Command/Query record'u oluştur
4. Handler oluştur (`internal sealed class`)
5. Validator oluştur (FluentValidation)
6. Endpoint oluştur (Minimal API, `IEndpoint`)
7. Gerekiyorsa DTO ve Error tanımı ekle
8. Gerekiyorsa cache invalidation handler'ı güncelle
9. Unit test yaz

---

## Kodlama Standartları Özeti

| Kural | Detay |
|---|---|
| Entity property'leri | `private set` |
| Entity oluşturma | Static factory method (`Create`) |
| EF Constructor | Parametresiz `private` constructor |
| Domain Event raise | Yalnızca domain metodu içinde |
| Child entity operasyonları | Aggregate root wrapper metodu üzerinden |
| Command/Query | `sealed record` |
| Handler | `internal sealed class` |
| Validator | `public sealed class` |
| Endpoint request/response | Endpoint içinde nested `sealed record` |
| Hata dönüşü | `Result<T>.Failure(error)` (exception yerine) |
| Domain hataları | `BusinessRuleViolationException`, `DomainNotFoundException` |
| Cache key | `{entity}:{id}` veya `{entity}:all` |
| Minimal API | `WithName`, `WithTags`, `WithSummary`, `Produces`, `ProducesProblem` |
