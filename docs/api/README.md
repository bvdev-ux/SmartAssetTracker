# Documentación de la API

La API expone documentación interactiva vía Swagger/OpenAPI en cada entorno de desarrollo.

## Acceso a Swagger UI

Con el backend en ejecución (`dotnet run --project src/AssetTracking.API`):

```
http://localhost:5000/swagger
```

Desde ahí se puede:

- Explorar todos los endpoints agrupados por controlador (`Auth`, `Assets`, `People`, `Movements`,
  `AccessControl`, `Dashboard`, `Reports`, `Audit`, `Locations`, `AssetCategories`, `AssetBrands`,
  `AssetModels`, `Alerts`).
- Ver el esquema de request/response de cada DTO.
- Autenticar con el botón **Authorize** usando un token JWT (`Bearer {token}`) obtenido en
  `POST /api/v1/auth/login`.
- Ejecutar peticiones de prueba directamente desde el navegador.

## Convenciones de la API

- Rutas versionadas bajo `/api/v1/`.
- Respuestas envueltas en `ApiResponse<T>` (`success`, `message`, `data`, `errors`).
- Autenticación JWT Bearer en los endpoints protegidos con `[Authorize]`.
- Colección de peticiones de ejemplo disponible en
  [`src/AssetTracking.API/AssetTracking.API.http`](../../src/AssetTracking.API/AssetTracking.API.http).
