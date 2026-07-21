# Integración continua

Este proyecto usa GitHub Actions para validar automáticamente cada cambio.

## Workflow: `.github/workflows/dotnet.yml`

Se ejecuta en:

- `push` a la rama `main`
- `pull_request` con destino a `main`

### Pasos

1. **Checkout** del repositorio.
2. **Setup .NET 8** (`actions/setup-dotnet`).
3. **Cache de paquetes NuGet** para acelerar builds sucesivos.
4. **Restore**: `dotnet restore src/AssetTracking.sln`.
5. **Build**: `dotnet build src/AssetTracking.sln --no-restore --configuration Release`.
6. **Test**: `dotnet test src/AssetTracking.sln --no-build --configuration Release`.

Si todos los pasos anteriores son exitosos, el job termina mostrando `✓ Restore, ✓ Build, ✓ Test, ✓ Success`.

## Ejecutar la misma validación localmente

```bash
dotnet restore src/AssetTracking.sln
dotnet build src/AssetTracking.sln --configuration Release
dotnet test src/AssetTracking.sln --configuration Release
```
