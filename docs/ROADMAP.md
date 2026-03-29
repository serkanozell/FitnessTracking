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

---

## 2. Bekleyen Refactoring'ler

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

### R11 — Specification Pattern 🟡
- **Sorun:** Repository'lerde sorgular inline predicate'ler veya sabit metotlarla yapılıyor.
- **Öneri:** Karmaşık filtreleme ihtiyacı arttığında (search, multi-filter) Specification pattern uygulanabilir.
- **Risk:** Orta
- **Öncelik:** Düşük (ihtiyaç doğduğunda)

### R12 — Idempotency Key 🟡
- **Sorun:** Create command'larda tekrarlanan request'lerde duplicate kayıt oluşabilir.
- **Öneri:** Request header'dan `X-Idempotency-Key` okuyup pipeline behavior olarak implement etmek.
- **Risk:** Orta
- **Öncelik:** Düşük

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

### Düşük Öncelik

| # | Özellik | Modül | Açıklama |
|---|---|---|---|
| F11 | **Export / Import** | Tüm modüller | PDF/CSV olarak veri dışa aktarma, antrenman programı içe aktarma |
| F12 | **Media Upload** | Exercises | Egzersiz görselleri/videoları için dosya yükleme (şu an sadece URL) — Azure Blob Storage |
| F13 | **Audit Log Modülü** | Yeni modül | Kullanıcı aksiyonlarının detaylı loglanması (kim, ne zaman, ne yaptı) |
| F14 | **Social / Sharing** | Yeni modül | Antrenman paylaşımı, arkadaş sistemi, lider tablosu |
| F15 | **Multi-language (i18n)** | API + MVC | Çoklu dil desteği |
