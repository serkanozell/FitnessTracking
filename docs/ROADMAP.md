# FitnessTracking — Refactoring & Feature Roadmap

> Bu doküman, yapılan refactoring'lerin kaydını ve gelecek feature/iyileştirme planını içerir.

---

## 1. Tamamlanan Refactoring'ler

### R1 — Entity Constructor Erişilebilirliği ✅
- **Sorun:** Aggregate root'larda ve child entity'lerde `public` parameterized constructor'lar mevcuttu. Mimari kural gereği entity oluşturma yalnızca `static Create()` factory method veya parent aggregate üzerinden yapılmalı.
- **Düzeltme:** 3 aggregate root'ta (`Exercise`, `WorkoutProgram`, `WorkoutSession`) constructor `private`, 3 child entity'de (`WorkoutProgramSplit`, `WorkoutSplitExercise`, `SessionExercise`) constructor `internal` yapıldı.
- **Etkilenen dosyalar:** 6 entity dosyası

### R2 — ARCHITECTURE.md Güncellemesi ✅
- **Sorun:** Doküman "Blazor WebAssembly (`FitnessTracking.Web`)" referansı içeriyordu, projede `FitnessTracking.Mvc` mevcut. Modül listesi eksikti (3/6).
- **Düzeltme:** Blazor → MVC, modül tablosu 6 modüle genişletildi, test ağacı güncellendi, Dashboard lightweight modül notu eklendi.

### R3 — Dashboard Modül Dokümantasyonu ✅
- **Sorun:** Dashboard'un Domain/Infrastructure katmanlarının olmama nedeni belgelenmemişti.
- **Düzeltme:** "Mevcut Modüller" tablosuna Tip (Tam/Lightweight) kolonu eklendi. Lightweight modül mimarisi açıklandı.

### R4 — BodyMetrics Infrastructure Integration Tests ✅
- **Sorun:** Diğer tüm tam modüllerde Infrastructure.IntegrationTests varken BodyMetrics'te yoktu.
- **Düzeltme:** `BodyMetrics.Infrastructure.IntegrationTests` projesi oluşturuldu — `SqlServerContainerFixture`, `BodyMetricRepositoryTests` (10 test).

### R5 — Dashboard Application Unit Tests Genişletme ✅
- **Sorun:** Dashboard testleri sadece happy path ve empty data kapsamındaydı. Edge case'ler (completion hesabı, dayCount, sınır değerler) eksikti.
- **Düzeltme:** 4 yeni test eklendi: CompletionPercentage halfway, programEnded %100 cap, DayCount hesabı, null metrics + dolu stats.

### R6 — Caching Güvenlik Düzeltmesi ✅
- **Sorun (KRİTİK):** `CachingBehavior` handler'dan önce çalışır → cache hit olduğunda handler çalışmaz → user-scoped query'lerde `OwnershipGuard` atlanır ve cross-user data leak oluşur.
- **Düzeltme:** 7 user-scoped query'den `ICacheableQuery` kaldırıldı (WorkoutPrograms: 4, WorkoutSessions: 3). `CacheKeys` sınıfından kullanılmayan key'ler temizlendi. ARCHITECTURE.md'ye güvenlik kuralı eklendi.
- **Güvenli kalan:** Exercises modülü query'leri (global data, user-scope yok).

### R7 — Ownership Guard Tutarlılığı ✅
- **Sorun:** `CreateWorkoutSessionCommandHandler`, `WorkoutProgramId` alıyor ama programın mevcut kullanıcıya ait olduğunu doğrulamıyordu. User A, User B'nin programına session oluşturabiliyordu.
- **Düzeltme:** `IWorkoutProgramModule`'e `IsOwnedByUserAsync` eklendi. Handler'a program varlık + ownership kontrolü eklendi (Admin bypass destekli). 4 unit test yazıldı.

### R8 — Dashboard GetDashboardQueryHandler Paralellik ✅
- **Konum:** `Dashboard.Application/Features/Dashboard/GetDashboard/GetDashboardQueryHandler.cs`
- **Sorun:** 4 çağrının tamamı `Task.WhenAll` ile paralel yapıldığında aynı `IWorkoutSessionModule` (aynı scoped DbContext) üzerinden iki concurrent sorgu çalışıyordu → EF Core thread-safety ihlali.
- **Düzeltme:** Farklı modüller (`_programModule`, `_bodyMetricModule`) fire-and-forget olarak başlatılır, aynı modül (`_sessionModule`) çağrıları sıralı kalır. Session çağrıları biterken diğer modül task'ları zaten tamamlanmış olur → güvenli paralellik.

### R9 — CORS Policy Adı Yeniden Adlandırma ✅
- **Konum:** `FitnessTracking.Api/Extensions/ProgramExtensions.cs`
- **Sorun:** CORS policy `"BlazorClient"` olarak adlandırılmıştı, proje artık MVC kullanıyor.
- **Düzeltme:** Policy adı `"WebClient"` olarak değiştirildi (tanım + kullanım). ARCHITECTURE.md güncellendi.

### R10 — Value Object Kullanımını Genişletme ✅
- **Sorun:** BodyMetrics'te `Weight`, `Height`, `BodyFatPercentage` primitive `decimal?` olarak tutuluyordu — domain seviyesinde değer doğrulaması yoktu.
- **Düzeltme:** 3 Value Object oluşturuldu:
  - `BodyWeight` — kg, > 0, ≤ 500
  - `BodyHeight` — cm, > 0, ≤ 300
  - `Percentage` — 0–100
- Entity property tipleri `decimal?` → `BodyWeight?`, `BodyHeight?`, `Percentage?` olarak değiştirildi. `Create`/`Update` metot imzaları `decimal?` kaldı (blast radius minimizasyonu — dönüşüm entity içinde yapılır).
- EF Core `OwnsOne` mapping ile mevcut kolon adları korundu (migration uyumlu).
- 9 yeni domain validation testi (sınır değerler, negatif, max aşımı).

### R13 — DB Şema İzolasyonu & Outbox Sahipliği ✅
- **Sorun:**
  1. DI extension method'larında tutarsız adlandırma (`WorkoutProgramsInfrastructure`, `WorkoutSessionsInfrastructure` — `Add` prefix'i yok).
  2. Modül context kayıtları tekrarlı (`AddDbContext` + interceptor + connection string her modülde elle).
  3. `__EFMigrationsHistory` tüm modüllerde `dbo`'da ortaktı (şema izolasyonu yarım).
  4. Şema isimleri string literal olarak dağınıktı.
  5. **Outbox sahipliği belirsiz:** `OutboxMessage`, `ModuleDbContext` üzerinden **her** modül modeline dahil ediliyordu ama tabloyu yalnızca Exercises migration'ı oluşturuyordu → çift oluşturma/drift riski.
- **Düzeltme:**
  - `AddWorkoutProgramsInfrastructure` / `AddWorkoutSessionsInfrastructure` olarak yeniden adlandırıldı.
  - `BuildingBlocks.Infrastructure.Persistence.PersistenceExtensions.AddModuleDbContext<TContext>` helper'ı eklendi: ortak connection string + `ISaveChangesInterceptor`'lar + şema-bazlı `__EFMigrationsHistory` izolasyonu.
  - Her modül için `*Schema` sabit sınıfı (`users`, `exercises`, `nutrition`, `bodymetrics`, `workoutprograms`, `workoutsessions`, `outbox`); literal'ler bu sabitlere taşındı.
  - `ModuleDbContext`'e `abstract Schema` (→ `HasDefaultSchema`) ve `virtual OwnsOutboxTable` eklendi. Tüm context'ler şema override eder.
  - **Outbox 2(b) modeli:** `OutboxMessageConfiguration` artık `excludeFromMigrations` parametresi alır. Yalnızca `OutboxDbContext` (`OwnsOutboxTable => true`) tabloyu migration'da oluşturur; diğer modüller mapping'i korur ama `ExcludeFromMigrations` ile DDL üretmez.
  - `OutboxDbContext` için `InitOutboxModule` baseline migration üretildi; eski Exercises migration'ından `OutboxMessages` CreateTable/DropTable kaldırıldı.
  - **Mevcut DB için:** `Migrations/Scripts/outbox-baseline-existing-db.sql` — tabloyu yeniden oluşturmadan `InitOutboxModule`'u `outbox.__EFMigrationsHistory`'ye baseline olarak işaretler (tablolar zaten oluşturulmuş durumda).

### R11 — Specification Pattern ✅
- **Sorun:** Repository'lerde sorgular inline predicate'ler veya neredeyse aynı `Where`/`Include`/`OrderBy` zincirleriyle tekrarlanıyordu (örn: `WorkoutSessionRepository`'de 5 adet `GetPagedBy*` metodu yalnızca predicate'te farklılaşıyordu).
- **Düzeltme:**
  - `BuildingBlocks.Domain/Abstractions`'a EF-bağımsız `ISpecification<T>` + abstract `Specification<T>` base eklendi (criteria, includes [expr/string], ordering, `AsNoTracking`, `AsSplitQuery` + protected builder metotları).
  - `BuildingBlocks.Infrastructure/Specifications`'a `SpecificationEvaluator.GetQuery<T>` (criteria → includes → ordering → split → no-tracking) ve spec-aware `ToListAsync`/`ToPagedListAsync` overload'ları eklendi.
  - `WorkoutSessions.Infrastructure/Specifications`'a 4 concrete spec (`ByUser`, `ByUserAndProgram`, `ByProgram`, `Paged`) eklendi; `WorkoutSessionRepository`'nin 4 paged metodu spec'lere delege edildi. **Public interface değişmedi → handler blast radius sıfır.**
  - 5 yeni unit test (`SpecificationEvaluatorTests` — criteria, ordering, no-criteria, paged, ToListAsync). ARCHITECTURE.md §6.4 eklendi.

### R12 — Idempotency Key ✅
- **Sorun:** Create command'larda tekrarlanan request'lerde (örn: client timeout sonrası retry) duplicate kayıt oluşabiliyordu.
- **Düzeltme:**
  - `BuildingBlocks.Application/Abstractions/Idempotency`'e marker `IIdempotentCommand` (`string? IdempotencyKey`) ve `IdempotencyOptions` (`ExpirationMinutes`, varsayılan 60) eklendi.
  - `IdempotencyBehavior<TRequest, TResponse>` pipeline behavior eklendi: key boşsa no-op; cache hit'te orijinal yanıtı replay eder; cache miss'te handler'ı çalıştırır ve yalnızca **başarılı** `Result`'ı `idempotency:{key}` altında cache'ler (başarısız denemeler retry edilebilir). Mevcut `ICacheService` kullanılır.
  - `ProgramExtensions.AddMediatR`'a `Validation → Logging → Idempotency → Caching → CacheInvalidation` sırasıyla register edildi; `IdempotencyOptions` `"Idempotency"` section'ından bind edildi.
  - **Referans implementasyon:** `CreateWorkoutSessionCommand` artık `IIdempotentCommand`; endpoint `X-Idempotency-Key` header'ını okuyup command'a propagate eder. Public davranış değişmez (header yoksa eski akış).
  - **Modül geneli yaygınlaştırma:** Aynı pattern tüm create/add command'larına uygulandı:
    - **WorkoutSessions:** `CreateWorkoutSession`, `AddExerciseToSession`
    - **WorkoutPrograms:** `CreateWorkoutProgram`, `AddWorkoutProgramSplit`, `AddExerciseToSplit`
    - **Exercises:** `CreateExercise` (mevcut `ICacheInvalidatingCommand` korunarak)
    - **BodyMetrics:** `CreateBodyMetric`
    - **Nutrition:** `CreateFood`, `CreateMealPlan`, `CreateDailyLog`, `AddLogEntry`, `AddMeal`, `AddMealItem`
    - **Users:** `CreateUser`, `CreateRole`, `AssignRole`
  - Her command'a trailing optional `string? IdempotencyKey = null` parametresi eklendi (kaynak uyumluluğu korunur); her endpoint `X-Idempotency-Key` header'ını propagate eder.
  - 4 yeni unit test (`IdempotencyBehaviorTests` — null key bypass, cached replay, success store, failure no-store). Tüm modül test suite'leri yeşil.

---

## 2. Bekleyen Refactoring'ler

> Tüm planlanan refactoring'ler tamamlandı. 🎉

---

## 3. Eklenmesi Gereken Özellikler

### Yüksek Öncelik

| # | Özellik | Modül | Açıklama |
|---|---|---|---|
| F1 | **Search / Filter** | Exercises, WorkoutPrograms, WorkoutSessions | Egzersiz isim/kas grubu araması, program tarih/aktiflik filtreleme, session filtreleme |
| F2 | **Exercise History** | WorkoutSessions (+ yeni endpoint) | Bir egzersizin tüm session'lardaki geçmişi — zaman içindeki ilerleme (weight/reps trend) |
| F3 | **Personal Records (PR) Takibi** | WorkoutSessions (+ yeni entity/endpoint) | Her egzersiz için max weight/reps kaydı, PR kırıldığında otomatik tespit |
| F4 | **BMI / Kalori Hesaplama** | BodyMetrics | BodyMetrics verisinden otomatik BMI hesaplama, günlük kalori ihtiyacı (TDEE) |

### Orta Öncelik

| # | Özellik | Modül | Açıklama |
|---|---|---|---|
| F5 | **Goals / Hedefler Modülü** | Yeni modül | Kilo hedefi, yağ oranı hedefi, kuvvet hedefi belirleme ve ilerleme takibi |
| F6 | **Workout Templates** | WorkoutPrograms | Hazır antrenman program şablonları (PPL, Upper/Lower, Full Body vb.) |
| F7 | **Notifications Modülü** | Yeni modül | Antrenman hatırlatıcıları, streak uyarıları, hedef bildirimleri (`IEmailSender` altyapısı mevcut) |
| F8 | **Session Duration / Rest Timer** | WorkoutSessions | Antrenman süresi takibi, setler arası dinlenme süresi kaydı |
| F9 | **Workout Completion Status** | WorkoutSessions | Session'lara "tamamlandı/devam ediyor/planlandı" durum alanı |
| F10 | **Superset / Circuit Support** | WorkoutPrograms, WorkoutSessions | Egzersizler arası superset ve devre antrenman desteği |
| F16 | **Excel Import/Export (Raporlama)** | Tüm modüller + yeni Reporting | Antrenman seansları, vücut ölçümleri ve analytics verileri için Excel (`.xlsx`) export; toplu egzersiz/program/ölçüm import. ClosedXML veya EPPlus ile. Raporlama dashboard'una entegre. |
| F17 | **Hangfire / Background Service Altyapısı** | BuildingBlocks + Api | Zamanlanmış işler (haftalık özet e-postası, eski outbox temizliği, periyodik istatistik/PR hesaplama) için Hangfire veya `IHostedService` tabanlı job altyapısı + dashboard ile job izleme. Mevcut Outbox `BackgroundService` ile birlikte konumlandırılır. |

### Düşük Öncelik

| # | Özellik | Modül | Açıklama |
|---|---|---|---|
| F11 | **Export / Import** | Tüm modüller | PDF/CSV olarak veri dışa aktarma, antrenman programı içe aktarma (bkz. F16 — Excel raporlama) |
| F12 | **Media Upload** | Exercises | Egzersiz görselleri/videoları için dosya yükleme (şu an sadece URL) — Azure Blob Storage |
| F13 | **Audit Log Modülü** | Yeni modül | Kullanıcı aksiyonlarının detaylı loglanması (kim, ne zaman, ne yaptı) |
| F14 | **Social / Sharing** | Yeni modül | Antrenman paylaşımı, arkadaş sistemi, lider tablosu |
| F15 | **Multi-language (i18n)** | API + MVC | Çoklu dil desteği |

---

## 4. Performans İyileştirmeleri

> Proje geneli inceleme sonucu tespit edilen performans maddeleri. Sırayla ele alınır.
> Öncelik: **P1 (Yüksek)** → **P2 (Orta)** → **P3 (Düşük / mimari karar)**.
> Tamamlanınca `[ ]` → `[x]` yapılır ve "Açıklama" sütununa commit/PR referansı eklenir.
>
> ⚠️ **Cross-cutting tarama zorunluluğu:** Buradaki maddeler genellikle **tüm modülleri** ilgilendirir. Bir madde uygulanırken iş tek modülle sınırlı bırakılmamalı; **önce tüm ilgili dosyalar (örn. tüm `*Repository` sınıfları) taranmalı**, kural uygun olan her yere uygulanmalı, bilinçli olarak uygulanmayan yerler gerekçesiyle belgelenmelidir. Detaylı prensip: `docs/ARCHITECTURE.md` §13.

| # | Durum | Öncelik | Başlık | Çözüm |
|---|---|---|---|---|
| P1 | [x] | P1 | **Cartesian explosion (`Include`+`ThenInclude`)** | İki seviyeli koleksiyon içeren tüm repository sorgularına `.AsSplitQuery()` eklendi: `WorkoutProgramRepository` (`Splits → Exercises`, 6 metot) ve `MealPlanRepository` (`Meals → MealItems`, 4 metot). Tek koleksiyonlu repository'ler (`WorkoutSessionRepository` → `SessionExercises`, `DailyNutritionLogRepository` → `Entries`) ve referans navigasyon (`UserRepository` → `UserRoles.Role`) cartesian explosion üretmediğinden ve gereksiz ek round-trip yaratmamak için dokunulmadı. |
| P2 | [x] | P1 | **Tutarsız `AsNoTracking`** | §13 kullanım-doğrulamalı tarama tüm modüllere uygulandı. Yalnızca **read-only query handler'larında** kullanılan listeleme sorgularına `.AsNoTracking()` eklendi: `WorkoutProgramRepository` (`GetListAsync`, `GetPagedAsync`, `GetPagedByUserAsync`), `BodyMetricRepository.GetPagedByUserAsync`, `FoodRepository` (`GetAllActiveAsync`, `GetPagedAsync`), `DailyNutritionLogRepository.GetPagedByUserAsync`, `MealPlanRepository.GetPagedByUserAsync`. **ROADMAP'in önerdiği `GetByIdWithExercisesAsync` BİLİNÇLİ olarak hariç tutuldu** — `AddExerciseToSplit`/`UpdateSplitExercise`/`Activate`/`RemoveSplitExercise` command handler'larında entity yüklenip değiştirilip `SaveChanges` çağrıldığı için `AsNoTracking` sessiz veri kaybı yaratırdı. Aynı nedenle tüm `GetById*`/`GetActive*`/`GetByUserAndDate` (mutation/cascade-delete path) hariç. Exercises ve WorkoutSessions repository'leri zaten `AsNoTracking`/spec ile tutarlıydı. 104 unit test yeşil. |
| P3 | [x] | P1 | **Handler'da in-memory DTO mapping (projeksiyon yok)** | §13 kullanım-doğrulamalı tarama tüm `FromEntity` list/paged handler'larına uygulandı. Repository Domain katmanında kalsın diye DTO coupling olmadan **generic selector-expression overload** pattern'i kullanıldı (`GetPagedAsync<TResult>(..., Expression<Func<TEntity,TResult>> selector, ...)`); DTO'ya `static readonly Expression<Func<Entity,Dto>> Projection` eklendi, handler projeksiyonu doğrudan tüketiyor. **Uygulanan modüller:** `FoodRepository.GetPagedAsync` (enum→string + owned `Macros`), `ExerciseRepository.GetPagedAsync` (enum→string), `BodyMetricRepository.GetPagedByUserAsync` (owned nullable VO `?.Value`), `RoleRepository.GetAllAsync` (flat scalar). Her biri **gerçek SQL Server'a karşı (Testcontainers MsSql) integration testiyle doğrulandı** — InMemory provider SQL çevirisini kanıtlayamadığı için; Foods 1/1, Exercises 2/2, BodyMetrics 4/4 (yeni owned-VO testi dahil), Roles 1/1 yeşil. **BİLİNÇLİ hariç tutulanlar:** nested aggregate map eden / hesaplanan toplam içeren handler'lar — `GetWorkoutSessions`/`GetExercisesBySession` (`WorkoutSessionDto.Exercises`), `GetDailyLogs` (`DailyNutritionLogDto` nested `Entries` + toplamlar), `GetMealPlans` (`MealPlanDto` nested `Meals`/`MealItems` + toplamlar); cross-module bağımlı `GetWorkoutProgramList` (`IExerciseModule` lookup ile birleştiriyor); ve aggregate root zaten yüklenip child map edilen `GetWorkoutProgramSplits`. Bunlar ayrı bir nested-projection/redesign çalışması gerektirir. **Not:** 5 BodyMetrics integration testi (`AddAsync`/`GetByIdAsync`/`Update`/`GetActiveByUserIdAsync`) P3 ile ilgisiz **mevcut (pre-existing) test hatası** — `entity.Weight.Should().Be(80m)` value object'i (`BodyWeight`) decimal'e karşı assert ediyor (implicit conversion yok), bu yüzden hiç geçmemişlerdi; düzeltmesi `.Value` ile yapılmalı. |
| P4 | [x] | P1 | **In-memory aggregation (`GetStatsByUserAsync`)** | §13 kullanım-doğrulamalı tarama tüm modül servislerine uygulandı; **iki** metotta `Include` + tüm grafiği belleğe yükleyip in-memory `Sum`/`Count` anti-pattern'i tespit edildi ve SQL'e taşındı. **Uygulanan:** (1) `WorkoutSessionModuleService.GetStatsByUserAsync` — `Include(SessionExercises)` + `SelectMany().Count()/Sum()` yerine per-session `.Select(s => new { s.Date, SetCount = s.SessionExercises.Count(), RepCount = s.SessionExercises.Sum(e => (int?)e.Reps) ?? 0 })` ile SQL aggregation; yalnızca SQL'e çevrilemeyen `CalculateStreak` için distinct tarihler bellekte. (2) `NutritionModuleService.GetDailySummaryAsync` — `Include(Meals).ThenInclude(MealItems)` + in-memory `allItems.Sum(...)` yerine owned collection üzerinde `x.Meals.SelectMany(m => m.MealItems).Sum(i => (decimal?)i.Macros.X) ?? 0m` ile doğrudan SQL projeksiyonu; `FirstOrDefaultAsync` null davranışı korundu. Referans örnek `GetVolumeTrendAsync` pattern'i (`(int?)`/`(decimal?)` null-guard cast) izlendi. **Doğrulama:** her iki metot **gerçek SQL Server'a karşı (Testcontainers MsSql) integration testiyle** doğrulandı — owned collection `Sum` çevirisi kritik olduğundan InMemory yeterli değil; WorkoutSessions 11/11 (2 yeni stats testi dahil), Nutrition 4/4 (3 yeni daily-summary testi dahil), 75 unit test (Dashboard/WorkoutSessions/Nutrition) yeşil. `WorkoutSessions.Infrastructure` ve `Nutrition.Infrastructure` csproj'larına `InternalsVisibleTo` eklendi (codebase konvansiyonu). **BİLİNÇLİ hariç tutulanlar:** `BodyMetricModuleService` (zaten `Select` projeksiyon, aggregation yok), `WorkoutProgramModuleService` (ownership/exists kontrolü + domain metot `ContainsExercise` + nested DTO map — P3'te de hariç), `ExerciseModuleService` (basit flat map, aggregation yok). |
| P5 | [ ] | P2 | **Dashboard ikili stats round-trip** | `GetDashboardQueryHandler` weekly + all-time istatistiğini tek sorguda döndüren contract metodu ile birleştir (conditional aggregation). İki DB round-trip → bir. |
| P6 | [ ] | P2 | **Cache stampede koruması** | `CacheAsideService.GetOrAddAsync` için key-bazlı `SemaphoreSlim` (in-process) veya Redis distributed lock ekle. Thundering herd'i önle. |
| P7 | [ ] | P2 | **Outbox: type cache + drain mantığı** | `OutboxBackgroundService`'te `Type.GetType` sonuçlarını `ConcurrentDictionary` ile cache'le; batch doluysa delay'i atlayıp sonraki batch'i işle. |
| P8 | [ ] | P2 | **Eksik composite DB index'leri** | `WorkoutSessions(UserId, Date)`, `(WorkoutProgramId)`, `(WorkoutProgramSplitId)` için `HasIndex` ekle. Soft-delete için filtered index (`HasFilter("[IsDeleted] = 0")`). |
| P9 | [ ] | P2 | **Rate limit değeri çok düşük** | IP başına 20 req/dk değeri dashboard'un çoklu analytics çağrısında yetersiz. Limiti artır veya endpoint başına policy uygula. |
| P10 | [ ] | P3 | **MVC N+1 HTTP çağrısı** | `DashboardService` analytics için ayrı endpoint çağrılarını `GetAnalyticsPageAsync` tek aggregate endpoint ile değiştir; gerekirse `Task.WhenAll` ile paralelleştir. |
| P11 | [ ] | P3 | **`DateTime.Now` → `DateTime.UtcNow`** | Sunucu zaman dilimi bağımlılığını kaldırmak için UTC'ye geçişi değerlendir. Global mimari karar — tüm projede tutarlı yapılmalı. |
 