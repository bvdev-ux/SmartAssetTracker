# Guía de despliegue

## Requisitos

- .NET SDK 8+
- Node.js 20+
- PostgreSQL 16 (o Docker/Docker Compose)

## Base de datos

```bash
docker compose up -d
```

Esto levanta PostgreSQL en el puerto `5433` con las credenciales definidas en `docker-compose.yml`.

Aplicar las migraciones de EF Core:

```bash
cd src
dotnet ef database update --project AssetTracking.Infrastructure --startup-project AssetTracking.API
```

## Backend (API)

```bash
cd src/AssetTracking.API
cp appsettings.Development.json.example appsettings.Development.json
# Ajustar ConnectionStrings y Jwt según el entorno
dotnet publish -c Release -o ./publish
dotnet ./publish/AssetTracking.API.dll
```

Variables de configuración relevantes (`appsettings.json` / variables de entorno):

- `ConnectionStrings:DefaultConnection`
- `Jwt:Key`, `Jwt:Issuer`, `Jwt:Audience`
- `Cors:Origins`

## Frontend

```bash
cd frontend
npm install
npm run build
npm run start
```

Configurar la URL del backend en `frontend/.env.local` (ver `.env.local.example`).

## Integración continua

Cada `push` o `pull request` hacia `main` ejecuta el workflow `.github/workflows/dotnet.yml`
(restore, build y test del backend). Ver [docs/CI.md](CI.md).
