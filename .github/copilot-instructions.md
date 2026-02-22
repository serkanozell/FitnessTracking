# Copilot Instructions

## Project Guidelines
- Domain event'ler her zaman entity/aggregate root domain metodları içinden raise edilmeli. Handler'larda AddDomainEvent gibi metodlar kullanılmamalı. Child entity operasyonları için aggregate root üzerinde wrapper domain metodları oluşturulmalı.