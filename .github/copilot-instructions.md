# Copilot Instructions

## Project Guidelines
- Domain event'ler her zaman entity/aggregate root domain metodları içinden raise edilmeli. Handler'larda AddDomainEvent gibi metodlar kullanılmamalı. Child entity operasyonları için aggregate root üzerinde wrapper domain metodları oluşturulmalı.
- Proje mimarisi, katman kuralları, kodlama standartları ve yeni modül/feature ekleme adımları için **`docs/ARCHITECTURE.md`** dosyasına başvurun.
- Refactoring geçmişi, bekleyen iyileştirmeler ve feature roadmap için **`docs/ROADMAP.md`** dosyasına başvurun.
- User-scoped query'lerde (OwnershipGuard içeren veya UserId ile filtreleyen) `ICacheableQuery` **kullanılmamalıdır**. Yalnızca global, user-scope'suz query'ler cache'lenebilir.