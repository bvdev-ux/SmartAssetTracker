# Asset Tracking

[![.NET CI](https://github.com/bvdev-ux/SmartAssetTracker/actions/workflows/dotnet.yml/badge.svg)](https://github.com/bvdev-ux/SmartAssetTracker/actions/workflows/dotnet.yml)

Plataforma web para la **gestión y trazabilidad de activos tecnológicos** en instituciones educativas.

## Stack

| Capa | Tecnología |
|------|------------|
| Backend | ASP.NET Core 8, EF Core, PostgreSQL, Clean Architecture |
| Frontend | Next.js 16, React 19, TypeScript, Tailwind CSS, React Hook Form, Zod |
| Base de datos | PostgreSQL 16 (Docker) |

## Estructura del monorepo

```text
asset-tracking/
├── docker-compose.yml          # PostgreSQL
├── docs/database/              # Modelo de datos y diccionario
├── frontend/                   # Next.js (Feature-Based Architecture)
└── src/                        # Backend .NET (Clean Architecture)
    ├── AssetTracking.API/
    ├── AssetTracking.Application/
    ├── AssetTracking.Domain/
    └── AssetTracking.Infrastructure/
```

## Requisitos

- .NET SDK 8+
- Node.js 20+
- Docker y Docker Compose

## Inicio rápido

### 1. Base de datos

```bash
docker compose up -d
```

### 2. Backend

```bash
cd src
dotnet restore
dotnet run --project AssetTracking.API
```

API disponible en `http://localhost:5000` — Swagger en `/swagger`.

**Usuario inicial (seed):**

- Email: `admin@institucion.edu`
- Contraseña: `Admin123!`

### 3. Frontend

```bash
cd frontend
cp .env.local.example .env.local
npm install
npm run dev
```

App disponible en `http://localhost:3000`.

## Fases del roadmap

| Fase | Estado | Descripción |
|------|--------|-------------|
| 1 | ✅ En progreso | Arquitectura y configuración del proyecto |
| 2 | Pendiente | Design System UX/UI completo |
| 3 | Pendiente | Modelo de datos y migraciones EF |
| 4 | Pendiente | Backend (módulos completos) |
| 5 | Pendiente | API REST (CRUD completo) |
| 6 | Pendiente | Frontend (módulos completos) |
| 7 | Pendiente | Integración |
| 8 | Pendiente | Pruebas |
| 9 | Pendiente | Despliegue |

## Arquitectura

### Backend — Clean Architecture

- **Domain**: entidades, enums, interfaces
- **Application**: DTOs, servicios, Result Pattern, Specification Pattern
- **Infrastructure**: EF Core, repositorios, Unit of Work, JWT, auditoría
- **API**: controladores versionados (`/api/v1`), Swagger, middleware de errores

### Frontend — Feature-Based Architecture

```text
frontend/src/
├── app/           # Rutas Next.js
├── features/      # Módulos por dominio (auth, dashboard, ...)
├── components/    # Design System reutilizable
├── layouts/       # Layouts compartidos
├── services/      # Cliente HTTP y servicios API
├── types/         # Tipos TypeScript
└── utils/         # Utilidades
```

## Extensibilidad futura

La arquitectura está preparada para integrar:

- **RFID**: campo `RfidTag` en activos y `ValidationMethod` en movimientos
- **Visión por computadora**: validación externa vía API
- **IA**: capa de servicios desacoplada en Application

## Licencia

Proyecto académico / institucional.
