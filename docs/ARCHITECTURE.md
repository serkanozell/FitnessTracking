# FitnessTracking — Mimari & Geliştirme Kılavuzu

> Bu doküman, projede tutarlılığı korumak ve yeni geliştirmelerde mimariyi bozmamak için referans olarak kullanılmalıdır.

---

## 1. Genel Bakış

**FitnessTracking**, .NET 10 tabanlı **Modular Monolith** mimarisiyle geliştirilmiş bir fitness takip uygulamasıdır.

| Katman | Rol |
|---|---|
| **API** (`FitnessTracking.Api`) | Host uygulama — modülleri yükler, middleware, CORS, health check, rate limiting |
| **Web** (`FitnessTracking.Web`) | Blazor WebAssembly istemci uygulaması |
| **Modules** | Her bounded context için bağımsız modüller (Exercises, WorkoutPrograms, WorkoutSessions) |
| **BuildingBlocks** | Tüm modüller tarafından paylaşılan altyapı, domain ve application yapı taşları |
| **Tests** | Unit, integration ve architecture testleri |

---

## 2. Modüler Yapı

Her modül aşağıdaki 4+1 katmanlı yapıya sahiptir:

```
src/Modules/{ModuleName}/
├── {ModuleName}.Domain          → Entity, AggregateRoot, ValueObject, DomainEvent, Repository interface
├── {ModuleName}.Application     → Command/Query, Handler, Validator, Endpoint, DTO, Error tanımları
├── {ModuleName}.Infrastructure  → EF Core DbContext, Repository implementasyonu, Migration, UnitOfWork
├── {ModuleName}.Api             → IModule implementasyonu (DI kayıt + endpoint mapping)
└── {ModuleName}.Contracts       → Modüller arası paylaşılan kontratlar (varsa)
```

### Mevcut Modüller
- **Exercises** — Egzersiz tanımları (kas grubu, açıklama vb.)
- **WorkoutPrograms** — Antrenman programları, split'ler ve split egzersizleri
- **WorkoutSessions** — Antrenman seansları ve seans egzersizleri

---

## 3. Katman Bağımlılık Kuralları

```
Domain ← Application ← Infrastructure ← Api
```

- **Domain** hiçbir katmana bağımlı değildir (saf domain modeli).
- **Application** yalnızca **Domain**'e bağımlıdır.
- **Infrastructure** hem **Domain** hem **Application**'a bağımlıdır.
- **Api** tüm katmanlara bağımlıdır (composition root).
- Modüller arası doğrudan bağımlılık **yasaktır**. Modüller arası iletişim **Contracts** projesi üzerinden yapılır.

> Bu kurallar `tests/ArchitectureTests/LayerDependencyTests.cs` ile NetArchTest kütüphanesi kullanılarak otomatik olarak doğrulanır.

---

## 4. Domain Katmanı Kuralları

### 4.1 Entity & Aggregate Root

- Tüm entity'ler `Entity<TId>` base class'ından türer.
- Aggregate root'lar `AggregateRoot<TId>` base class'ından türer.
- Her entity'de `Id`, `IsActive`, `IsDeleted`, audit alanları (`CreatedDate`, `CreatedBy`, `UpdatedDate`, `UpdatedBy`) ve `RowVersion` (concurrency) bulunur.
- Property'ler **`private set`** kullanır — dışarıdan doğrudan değiştirilemez.
- Entity oluşturma **static factory method** (`Create`) ile yapılır.
- Her entity'nin **parametresiz private constructor**'ı vardır (EF Core materialization için).

```csharp
// ✅ Doğru
public static Exercise Create(string name, ...) { ... }

// ❌ Yanlış — Dışarıdan new ile oluşturma
var exercise = new Exercise(Guid.NewGuid(), name, ...);
```

### 4.2 Domain Event Kuralları (KRİTİK)

> **Domain event'ler her zaman entity/aggregate root domain metodları içinden raise edilmelidir.**

- `AddDomainEvent()` çağrısı **yalnızca** domain entity'lerinin kendi metodları içinde yapılır.
- Application katmanındaki handler'larda **asla** `AddDomainEvent` çağrılmaz.
- Child entity operasyonları (örn: Split'e egzersiz ekleme) için **aggregate root üzerinde wrapper domain metodları** oluşturulur.

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
// var split = program.Splits.First(s => s.Id == splitId);
// split.AddExercise(exerciseId, ...);  // Event raise edilmez!
```

### 4.3 Domain Event Tanımı

- Domain event'ler `IDomainEvent` interface'ini implement eden **sealed record** olarak tanımlanır.
- `{ModuleName}.Domain/Events/` klasöründe tutulur.

```csharp
public sealed record ExerciseCreatedEvent(Guid ExerciseId) : IDomainEvent
{
    public DateTime OccurredOn => DateTime.Now;
}
```

### 4.4 Domain Exception'lar

| Exception | Kullanım |
|---|---|
| `DomainNotFoundException` | Entity bulunamadığında (parent-child ilişkili overload mevcut) |
| `BusinessRuleViolationException` | İş kuralı ihlalinde (duplicate split adı, aktif olmayan program vb.) |
| `DomainException` | Genel domain hataları (base class) |

### 4.5 Repository Interface'leri

- Repository interface'leri **Domain katmanında** tanımlanır (`{ModuleName}.Domain/Repositories/`).
- UnitOfWork interface'i (`IUnitOfWork`) BuildingBlocks.Domain'de tanımlıdır.
- Her modül kendi `I{ModuleName}UnitOfWork` interface'ine sahiptir.

---

## 5. Application Katmanı Kuralları

### 5.1 CQRS Pattern

- **Command'lar**: `ICommand<TResponse>` interface'ini implement eder (MediatR `IRequest<T>` tabanlı).
- **Query'ler**: `IQuery<TResponse>` interface'ini implement eder.
- **Handler'lar**: `ICommandHandler<TCommand, TResponse>` veya `IQueryHandler<TQuery, TResponse>` implement eder.
- Handler'lar **`internal sealed`** olarak tanımlanır.
- Command/Query sınıfları **`sealed record`** olarak tanımlanır.

```csharp
// Command
public sealed record CreateExerciseCommand(string Name, ...) : ICommand<Result<Guid>>;

// Handler
internal sealed class CreateExerciseCommandHandler(...) : ICommandHandler<CreateExerciseCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateExerciseCommand request, CancellationToken cancellationToken) { ... }
}
```

### 5.2 Result Pattern

- Tüm command/query handler'lar `Result<T>` veya `Result` döner.
- Başarılı sonuç: `Result<T>.Success(data)`
- Hatalı sonuç: `Result<T>.Failure(error)` veya implicit conversion ile `Error` nesnesi doğrudan dönülebilir.
- Exception fırlatmak yerine `Result.Failure` tercih edilir (domain exception'lar hariç).

### 5.3 Error Tanımları

- Her modül kendi error sınıfına sahiptir: `{ModuleName}.Application/Errors/{Entity}Errors.cs`
- Error'lar **static method** olarak tanımlanır.

```csharp
public static class ExerciseErrors
{
    public static Error NotFound(Guid id) => new("Exercise.NotFound", $"Exercise with ID '{id}' was not found.");
    public static Error DuplicateName(string name) => new("Exercise.DuplicateName", $"...");
}
```

### 5.4 Validation

- Her command için **FluentValidation** validator'ı oluşturulur.
- Validator'lar `{Feature}/` klasöründe command ile birlikte tutulur.
- Validator sınıfları `public sealed class` olarak tanımlanır.
- Validation, `ValidationBehavior` pipeline davranışı ile otomatik çalışır.

### 5.5 Endpoint Tanımı (Vertical Slice)

- Endpoint'ler Application katmanında `IEndpoint` interface'ini implement eder.
- Her endpoint kendi feature klasöründe tutulur (Vertical Slice Architecture).
- Request/Response DTO'ları endpoint sınıfı içinde **nested sealed record** olarak tanımlanır.
- Endpoint'ler Minimal API kullanır (`MapGet`, `MapPost`, `MapPut`, `MapDelete`).

```csharp
public sealed class CreateExerciseEndpoint : IEndpoint
{
    public void Map(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/exercises", async (...) => { ... })
            .WithName("CreateExercise")
            .WithTags("Exercises")
            .WithSummary("...")
            .WithDescription("...")
            .Accepts<CreateExerciseRequest>("application/json")
            .Produces<CreateExerciseResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest);
    }

    public sealed record CreateExerciseRequest(...);
    public sealed record CreateExerciseResponse(...);
}
```

### 5.6 Caching

- Cache'lenecek query'ler `ICacheableQuery` interface'ini implement eder.
- `CacheKey` ve opsiyonel `Expiration` tanımlanır.
- Cache invalidation, domain event handler'ları (`IDomainEventHandler<TEvent>`) ile yapılır.
- Cache servisi: Redis (`ICacheService`, `ICacheAsideService`).
- Cache key formatı: `{entity}:{id}` veya `{entity}:all` (liste için).

### 5.7 Feature Klasör Yapısı

```
Features/{Entity}/{FeatureName}/
├── {FeatureName}Command.cs          (veya Query)
├── {FeatureName}CommandHandler.cs   (veya QueryHandler)
├── {FeatureName}CommandValidator.cs
└── {FeatureName}Endpoint.cs
```

### 5.8 Global Usings

Her modülün Application katmanında `GlobalUsings.cs` dosyası bulunur:

```csharp
global using BuildingBlocks.Application.CQRS;
global using BuildingBlocks.Application.Results;
global using {ModuleName}.Domain.Repositories;
global using FluentValidation;
global using MediatR;
global using Microsoft.AspNetCore.Builder;
```

---

## 6. Infrastructure Katmanı Kuralları

### 6.1 EF Core & DbContext

- Her modülün kendi DbContext'i vardır (`{ModuleName}DbContext`).
- Entity konfigürasyonları `IEntityTypeConfiguration<T>` ile ayrı dosyalarda tanımlanır.
- `OutboxInterceptor` domain event'leri Outbox tablosuna yazar (SaveChanges sırasında).
- `AuditableEntityInterceptor` audit alanlarını otomatik doldurur.

### 6.2 Outbox Pattern

- Domain event'ler `OutboxInterceptor` tarafından yakalanır ve `OutboxMessage` tablosuna serialize edilir.
- `OutboxBackgroundService` arka planda outbox mesajlarını işler ve MediatR ile publish eder.
- Bu sayede domain event'ler transactional olarak garanti altına alınır.

### 6.3 Repository Implementasyonu

- Repository'ler Infrastructure katmanında `{ModuleName}.Infrastructure/Repositories/` altında implement edilir.
- `UnitOfWork<TContext>` abstract sınıfı kullanılır.

### 6.4 Concurrency Control

- Entity'lerde `RowVersion` property'si ile optimistic concurrency uygulanır.
- `DbUpdateConcurrencyException` global exception handler'da 409 Conflict olarak döner.

---

## 7. API Katmanı (Composition Root)

### 7.1 Modül Kayıt Sistemi

Her modül `IModule` interface'ini implement eder:

```csharp
public sealed class ExercisesModule : IModule
{
    public Assembly ApplicationAssembly => typeof(Application.AssemblyReference).Assembly;
    public void Register(IServiceCollection services, IConfiguration configuration) { }
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        app.MapEndpointsFromAssembly(ApplicationAssembly);
    }
}
```

`Program.cs`'de modüller bir dizi olarak tanımlanır ve döngüyle yüklenir:

```csharp
IModule[] modules = [new ExercisesModule(), new WorkoutProgramsModule(), new WorkoutSessionsModule()];
```

### 7.2 MediatR Pipeline Davranışları

Pipeline sırası:
1. **ValidationBehavior** — FluentValidation ile komut doğrulama
2. **LoggingBehavior** — Request/Response loglama (Serilog), 3 saniyeyi aşan request'ler için uyarı
3. **CachingBehavior** — `ICacheableQuery` implement eden query'ler için cache-aside

### 7.3 Exception Handling

`GlobalExceptionHandler` (`IExceptionHandler`) tüm exception'ları `ProblemDetails` formatında döner:

| Exception | HTTP Status |
|---|---|
| `ValidationException` | 400 Bad Request |
| `DomainNotFoundException` | 404 Not Found |
| `BusinessRuleViolationException` | 409 Conflict |
| `DbUpdateConcurrencyException` | 409 Conflict |
| `DomainException` | 422 Unprocessable Entity |
| Diğer | 500 Internal Server Error |

### 7.4 Cross-Cutting Concerns

- **CORS**: Blazor client için `BlazorClient` policy
- **Rate Limiting**: IP tabanlı fixed window (20 request/dakika, 5 queue)
- **Health Checks**: SQL Server, Redis, SMTP (`/health`, `/health/ready`, `/health/live`)
- **Logging**: Serilog (configuration-based)

---

## 8. Blazor WebAssembly (FitnessTracking.Web)

- Standalone Blazor WebAssembly uygulaması
- API'ye `HttpClient` tabanlı servisler üzerinden erişir
- Servisler: `IExercisesService`, `IWorkoutProgramsService`, `IWorkoutSessionsService`
- Model'ler ve DTO'lar Web projesinde ayrı tanımlanır (API DTO'larından bağımsız)

---

## 9. Test Stratejisi

```
tests/
├── ArchitectureTests/              → NetArchTest ile mimari kural testleri
├── BuildingBlocks/
│   ├── BuildingBlocks.Application.UnitTests/
│   └── BuildingBlocks.Infrastructure.UnitTests/
├── Modules/
│   ├── Exercises/
│   │   ├── Exercises.Domain.UnitTests/
│   │   ├── Exercises.Application.UnitTests/
│   │   └── Exercises.Infrastructure.IntegrationTests/
│   ├── WorkoutPrograms/
│   │   ├── WorkoutPrograms.Domain.UnitTests/
│   │   ├── WorkoutPrograms.Application.UnitTests/
│   │   └── WorkoutPrograms.Infrastructure.IntegrationTests/
│   └── WorkoutSessions/
│       ├── WorkoutSessions.Domain.UnitTests/
│       ├── WorkoutSessions.Application.UnitTests/
│       └── WorkoutSessions.Infrastructure.IntegrationTests/
└── FitnessTracking.Api.IntegrationTests/
```

### Test Katmanları
- **Domain Unit Tests**: Entity business logic, domain event raise kontrolü
- **Application Unit Tests**: Handler logic, validation rule testleri
- **Infrastructure Integration Tests**: Repository + DB testleri
- **API Integration Tests**: End-to-end endpoint testleri
- **Architecture Tests**: Katman bağımlılık, naming convention, class design kuralları

---

## 10. Yeni Modül Ekleme Kontrol Listesi

Yeni bir modül eklerken aşağıdaki adımları takip edin:

1. **Domain projesi oluştur**: `src/Modules/{ModuleName}/{ModuleName}.Domain`
   - Entity'ler (`Entity/`), Event'ler (`Events/`), Repository interface'leri (`Repositories/`), Enum'lar (`Enums/`)
2. **Application projesi oluştur**: `src/Modules/{ModuleName}/{ModuleName}.Application`
   - `GlobalUsings.cs`, `AssemblyReference.cs`, Features, DTOs, Errors
3. **Infrastructure projesi oluştur**: `src/Modules/{ModuleName}/{ModuleName}.Infrastructure`
   - DbContext, Repository impl, UnitOfWork, Configurations, Migrations, DI extension metodu
4. **Api projesi oluştur**: `src/Modules/{ModuleName}/{ModuleName}.Api`
   - `IModule` implementasyonu
5. **Contracts projesi (opsiyonel)**: `src/Modules/{ModuleName}/{ModuleName}.Contracts`
6. **`Program.cs`'e kayıt et**: Modül dizisine ve infrastructure DI'a ekle
7. **Test projeleri oluştur**: Domain.UnitTests, Application.UnitTests, Infrastructure.IntegrationTests
8. **Architecture test'lere ekle**: `LayerDependencyTests`'e yeni modülü ekle

---

## 11. Yeni Feature Ekleme Kontrol Listesi

1. Gerekiyorsa **Domain entity'ye** yeni domain metodu ekle (event raise dahil)
2. Gerekiyorsa yeni **Domain Event** oluştur (`Events/` altında)
3. **Command/Query** record'u oluştur
4. **Handler** oluştur (`internal sealed class`)
5. **Validator** oluştur (FluentValidation)
6. **Endpoint** oluştur (Minimal API, `IEndpoint`)
7. Gerekiyorsa **DTO** oluştur
8. Gerekiyorsa **Error** tanımı ekle
9. Gerekiyorsa **Cache invalidation** handler'ı güncelle
10. **Unit test** yaz

---

## 12. Kodlama Standartları Özeti

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
