# Copilot Instructions

## Project Guidelines
- Domain event'ler her zaman entity/aggregate root domain metodları içinden raise edilmeli. Handler'larda AddDomainEvent gibi metodlar kullanılmamalı. Child entity operasyonları için aggregate root üzerinde wrapper domain metodları oluşturulmalı.
- Proje mimarisi, katman kuralları, kodlama standartları ve yeni modül/feature ekleme adımları için **`docs/ARCHITECTURE.md`** dosyasına başvurun.
- Refactoring geçmişi, bekleyen iyileştirmeler ve feature roadmap için **`docs/ROADMAP.md`** dosyasına başvurun.
- **Cross-cutting / performans değişiklikleri tüm modüllerde tutarlı uygulanmalıdır.** Bir performans iyileştirmesi, repository/query pattern düzeltmesi veya başka bir toplu (cross-cutting) değişiklik yapılırken iş tek bir modülle sınırlı bırakılmamalı; **önce tüm modüllerdeki ilgili dosyalar (örn. tüm `*Repository` sınıfları) taranmalı**, ardından aynı kural uygun olan her yere uygulanmalıdır. Bilinçli olarak uygulanmayan yerler (örn. tek koleksiyonlu sorguda `AsSplitQuery` gereksizliği) gerekçesiyle birlikte belgelenmelidir. Detaylı prensip için `docs/ARCHITECTURE.md` §13'e bakın.
- User-scoped query'lerde (OwnershipGuard içeren veya UserId ile filtreleyen) `ICacheableQuery` **kullanılmamalıdır**. Yalnızca global, user-scope'suz query'ler cache'lenebilir.