# AssetTracking.Tests

Pruebas unitarias del backend con xUnit.

## Cobertura

- **Auth**: generación de hash de contraseñas y flujo de login (`AuthServiceTests`, `PasswordHasherTests`).
- **People**: validaciones de negocio del servicio de personas (`PersonServiceTests`).
- **Assets**: validaciones de negocio del servicio de activos (`AssetServiceTests`).
- **Repositories**: consultas y validaciones de unicidad sobre `PersonRepository` y `AssetRepository`.

Los repositorios y servicios se prueban contra una base de datos EF Core InMemory (`TestDbContextFactory`), sin necesidad de PostgreSQL.

## Ejecutar las pruebas

```bash
dotnet test src/AssetTracking.sln
```
