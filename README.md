# DeportivoUCN Backend

API REST for the DeportivoUCN management system, built with .NET 10 using a layered architecture and containerized with Docker.

## Technologies

- .NET 10
- PostgreSQL 16
- Entity Framework Core
- Docker / Docker Compose

## Architecture

The project is split into 4 independent layers following the **Layered Architecture** pattern. Unlike having folders inside a single project, each layer is a separate .NET project, meaning dependencies between layers are **enforced by the compiler** — if a layer doesn't explicitly reference another, it simply won't compile.

```
DeportivoUCN.API
       ↓
DeportivoUCN.Application
       ↓
DeportivoUCN.Infrastructure
       ↓
DeportivoUCN.Models
```

The main rule is that **Models depends on nobody** — it is the core of the system. Everything else depends inward.

| Project | Responsibility |
|---|---|
| **API** | Controllers, HTTP configuration, middlewares |
| **Application** | Use cases, interfaces, DTOs, business logic |
| **Infrastructure** | EF Core, repositories, external services |
| **Models** | Entities, enums, value objects |

This ensures that, for example, a controller can never access the database directly bypassing business logic — something that with folders in a single project nobody stops you from doing.

## Project Structure

```
deportivo-ucn-backend/
 ├── src/
 │    ├── DeportivoUCN.API            → Controllers, HTTP configuration, Program.cs
 │    ├── DeportivoUCN.Application    → Use cases, interfaces, DTOs, services
 │    ├── DeportivoUCN.Infrastructure → EF Core, repositories, data access
 │    └── DeportivoUCN.Models         → Entities, enums, value objects
 ├── Dockerfile
 ├── docker-compose.yml
 ├── docker-compose.override.yml
 ├── .dockerignore
 └── DeportivoUCN.slnx
```

## Requirements

- Docker
- Docker Compose

No need to install .NET or PostgreSQL on your machine.

## Setup

Create a `.env` file in the project root:

```env
DB_PASSWORD=your_password
```

> ⚠️ The `.env` file must never be pushed to the repository.

## Running the project

### Development (hot reload)

```bash
docker compose up
```

The API reloads automatically when code changes are detected.

### Production

```bash
docker compose -f docker-compose.yml up --build
```

## Services

| Service | URL |
|---|---|
| API | http://localhost:5059 |
| PostgreSQL | localhost:5432 |

## Environment Variables

| Variable | Description |
|---|---|
| `DB_PASSWORD` | PostgreSQL password |
| `ASPNETCORE_ENVIRONMENT` | Runtime environment (`Development` / `Production`) |
| `ConnectionStrings__DefaultConnection` | Full DB connection string |

## Useful Commands

```bash
# See running containers
docker compose ps

# Stream logs
docker compose logs -f

# Stream API logs only
docker compose logs -f api

# Stop everything
docker compose down

# Stop and delete volumes (⚠️ deletes DB data)
docker compose down -v
```

## Migrations

```bash
# Apply migrations
docker exec deportivo_api dotnet ef database update

# Create new migration
docker exec deportivo_api dotnet ef migrations add MigrationName
```

## Database

PostgreSQL data persists in a Docker volume called `pgdata`. Data is preserved even if containers are restarted.

To create a backup:

```bash
docker exec deportivo_db pg_dump -U postgres DeportivoUCN > backup.sql
```

To restore a backup:

```bash
docker exec -i deportivo_db psql -U postgres DeportivoUCN < backup.sql
```

---

# DeportivoUCN Backend

API REST del sistema de gestión del Deportivo UCN, desarrollada en .NET 10 con arquitectura en capas y contenerizada con Docker.

## Tecnologías

- .NET 10
- PostgreSQL 16
- Entity Framework Core
- Docker / Docker Compose

## Arquitectura

El proyecto está separado en 4 capas independientes siguiendo el patrón de **Arquitectura en Capas**. A diferencia de tener carpetas dentro de un solo proyecto, cada capa es un proyecto .NET separado, lo que significa que las dependencias entre capas son **forzadas por el compilador** — si una capa no referencia a otra explícitamente, simplemente no compila.

```
DeportivoUCN.API
       ↓
DeportivoUCN.Application
       ↓
DeportivoUCN.Infrastructure
       ↓
DeportivoUCN.Models
```

La regla principal es que **Models no depende de nadie** — es el núcleo del sistema. Los demás dependen hacia adentro.

| Proyecto | Responsabilidad |
|---|---|
| **API** | Controllers, configuración HTTP, middlewares |
| **Application** | Casos de uso, interfaces, DTOs, lógica de negocio |
| **Infrastructure** | EF Core, repositorios, servicios externos |
| **Models** | Entidades, enums, value objects |

Esto garantiza que, por ejemplo, un controlador nunca pueda acceder directamente a la base de datos saltándose la lógica de negocio — algo que con carpetas en un solo proyecto nadie te impide hacer.

## Estructura del proyecto

```
deportivo-ucn-backend/
 ├── src/
 │    ├── DeportivoUCN.API            → Controllers, configuración HTTP, Program.cs
 │    ├── DeportivoUCN.Application    → Casos de uso, interfaces, DTOs, servicios
 │    ├── DeportivoUCN.Infrastructure → EF Core, repositorios, acceso a datos
 │    └── DeportivoUCN.Models         → Entidades, enums, value objects
 ├── Dockerfile
 ├── docker-compose.yml
 ├── docker-compose.override.yml
 ├── .dockerignore
 └── DeportivoUCN.slnx
```

## Requisitos

- Docker
- Docker Compose

No necesitas instalar .NET ni PostgreSQL en tu máquina.

## Configuración

Crea un archivo `.env` en la raíz del proyecto:

```env
DB_PASSWORD=tu_password
```

> ⚠️ El archivo `.env` nunca debe subirse al repositorio.

## Levantar el proyecto

### Desarrollo (hot reload)

```bash
docker compose up
```

La API se recarga automáticamente al detectar cambios en el código.

### Producción

```bash
docker compose -f docker-compose.yml up --build
```

## Servicios

| Servicio | URL |
|---|---|
| API | http://localhost:5059 |
| PostgreSQL | localhost:5432 |

## Variables de entorno

| Variable | Descripción |
|---|---|
| `DB_PASSWORD` | Contraseña de PostgreSQL |
| `ASPNETCORE_ENVIRONMENT` | Entorno de ejecución (`Development` / `Production`) |
| `ConnectionStrings__DefaultConnection` | Connection string completo de la DB |

## Comandos útiles

```bash
# Ver contenedores corriendo
docker compose ps

# Ver logs en tiempo real
docker compose logs -f

# Ver logs solo de la API
docker compose logs -f api

# Detener todo
docker compose down

# Detener y borrar volúmenes (⚠️ borra los datos de la DB)
docker compose down -v
```

## Migraciones

```bash
# Aplicar migraciones
docker exec deportivo_api dotnet ef database update

# Crear nueva migración
docker exec deportivo_api dotnet ef migrations add NombreMigracion
```

## Base de datos

Los datos de PostgreSQL persisten en un volumen Docker llamado `pgdata`. Se conservan aunque se reinicien los contenedores.

Para hacer un backup:

```bash
docker exec deportivo_db pg_dump -U postgres DeportivoUCN > backup.sql
```

Para restaurar un backup:

```bash
docker exec -i deportivo_db psql -U postgres DeportivoUCN < backup.sql
```