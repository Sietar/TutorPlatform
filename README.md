# TutorPlatform

![CI](https://github.com/Sietar/TutorPlatform/actions/workflows/ci.yml/badge.svg)

A personal cabinet for a private tutor and their students: lesson schedule, homework and progress tracking in one place.

The project is built for a real user — a working tutor whose students currently juggle schedules, materials and assignments across messengers and shared drives. The goal is a small, reliable tool she can use every day.

> **Status:** work in progress. The domain model and REST API are implemented and covered by unit tests. Authentication, the React frontend and deployment are next (see [Roadmap](#roadmap)).

## Features

- **Teachers and students** — a teacher manages their own list of students.
- **Lesson scheduling** — create, complete, cancel and reschedule lessons. Overlapping lessons for the same teacher are rejected; canceled lessons do not block the time slot.
- **Unmarked lessons** — lessons that ended a while ago but were never marked as completed or canceled are detected, so the teacher can be reminded.
- **Homework workflow** — `Assigned → Submitted → Completed`, with the option to return work for revision (`Returned → Submitted`) or cancel it. Overdue status is calculated from the deadline rather than stored.
- **Consistent error handling** — domain rule violations return `400`, duplicates return `409`, all in the standard [ProblemDetails](https://www.rfc-editor.org/rfc/rfc9457) format.

## Tech stack

| Area | Technologies |
|---|---|
| Backend | C#, .NET 10, ASP.NET Core Web API (controllers) |
| Data | PostgreSQL 17, Entity Framework Core 10 (Npgsql), code-first migrations |
| Tests | xUnit |
| Infrastructure | Docker Compose, GitHub Actions |
| Frontend (planned) | React, TypeScript, Vite |

## Architecture

```
src/
├── TutorPlatform.Core/   Domain: entities, business rules, no framework dependencies
└── TutorPlatform.Api/    Web API: controllers, DTOs, EF Core, error handling
tests/
└── TutorPlatform.Core.Tests/   Unit tests for the domain
frontend/                 React + TypeScript client (in progress)
```

Key decisions:

- **The domain knows nothing about the web or the database.** `Core` has no references to ASP.NET or EF Core, so business rules are tested in isolation and mapping is configured separately with the Fluent API.
- **Rules live in the entities.** Status transitions (`Complete()`, `Cancel()`, `Submit()`, …) validate the current state and throw a `DomainException` when a transition is not allowed. Controllers only load an entity, call a method and save.
- **Encapsulated state.** Properties have private setters and collections are exposed as read-only, so an entity cannot be put into an invalid state from the outside.
- **Time is stored in UTC.** The teacher and students live in different time zones; local time is a presentation concern.
- **Time-dependent state is computed, not stored.** "Overdue" and "awaiting mark" are calculated for a given moment, so no background job is needed to keep them correct.

## Getting started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/products/docker-desktop/)
- [EF Core CLI](https://learn.microsoft.com/ef/core/cli/dotnet): `dotnet tool install --global dotnet-ef`

### Run locally

1. Create a `.env` file from the example and set the database credentials:

   ```bash
   cp .env.example .env
   ```

2. Start PostgreSQL:

   ```bash
   docker compose up -d
   ```

3. Store the connection string in User Secrets (use the values from `.env`):

   ```bash
   dotnet user-secrets set "ConnectionStrings:Default" \
     "Host=localhost;Port=5432;Database=tutorplatform;Username=<user>;Password=<password>" \
     --project src/TutorPlatform.Api
   ```

4. Apply migrations and run the API:

   ```bash
   dotnet ef database update -p src/TutorPlatform.Api
   dotnet run --project src/TutorPlatform.Api
   ```

The API listens on `http://localhost:5266`. Example requests are in [`TutorPlatform.Api.http`](src/TutorPlatform.Api/TutorPlatform.Api.http) and can be run from Rider or Visual Studio.

### Run tests

```bash
dotnet test
```

## API overview

| Method | Endpoint | Description |
|---|---|---|
| `GET` | `/api/teachers` | List teachers |
| `GET` | `/api/teachers/{id}` | Get a teacher |
| `POST` | `/api/teachers` | Create a teacher |
| `GET` | `/api/teachers/{id}/students` | List a teacher's students |
| `POST` | `/api/teachers/{id}/students` | Add a student to a teacher |
| `GET` | `/api/lessons?teacherId=&from=&to=` | Teacher's schedule for a period |
| `GET` | `/api/lessons/{id}` | Get a lesson |
| `POST` | `/api/lessons` | Schedule a lesson |
| `POST` | `/api/lessons/{id}/complete` | Mark a lesson as completed |
| `POST` | `/api/lessons/{id}/cancel` | Cancel a lesson |
| `POST` | `/api/lessons/{id}/reschedule` | Move a lesson to a new time |
| `GET` | `/api/homeworks?studentId=&teacherId=` | List homework |
| `GET` | `/api/homeworks/{id}` | Get a homework assignment |
| `POST` | `/api/homeworks` | Assign homework |
| `POST` | `/api/homeworks/{id}/submit` | Submit an answer |
| `POST` | `/api/homeworks/{id}/return` | Return for revision with a comment |
| `POST` | `/api/homeworks/{id}/complete` | Accept the work |
| `POST` | `/api/homeworks/{id}/cancel` | Cancel the assignment |

All dates are ISO 8601 in UTC (for example `2026-12-01T15:00:00Z`).

## Roadmap

- [x] Domain model with business rules and unit tests
- [x] REST API for teachers, students, lessons and homework
- [x] PostgreSQL with EF Core migrations, Docker Compose
- [x] Global error handling, CI on GitHub Actions
- [ ] Authentication and roles with ASP.NET Core Identity
- [ ] React frontend
- [ ] Deployment to a VPS with HTTPS and database backups
- [ ] Learning materials attached to lessons and homework
- [ ] Interactive whiteboard and video calls
