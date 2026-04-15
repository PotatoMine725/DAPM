# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## 1. Project Overview

**ClinicBooking** (DatLichPhongKham) is an ASP.NET Core 8 Web API for clinic appointment booking.

This system supports:

- Patients (Bệnh nhân)
- Receptionists (Lễ tân)
- Doctors (Bác sĩ)
- Admin

Core features:

- Online appointment booking
- Check-in and queue management
- Doctor scheduling
- Medical records and prescriptions
- Notifications (email / in-app)
- Admin management and reporting

---

## 2. Architecture

### Chosen Architecture

**Modular Monolith + Clean Architecture**

```text
Presentation (API)
↓
Application (Use Cases)
↓
Domain (Entities and Business Rules)
↓
Infrastructure (DB, External Services)
```

### Solution Structure

```text
DatLichPhongKham.slnx
├── ClinicBooking.Domain          # Entities, Enums - no dependencies
├── ClinicBooking.Application     # Business logic, CQRS via MediatR, DTOs, validators
├── ClinicBooking.Infrastructure  # EF Core (SQL Server), Identity, external services
└── ClinicBooking.Api             # ASP.NET Core Web API, controllers, middleware
```

Dependency direction: Api → Application + Infrastructure → Application → Domain.

### Responsibilities by Layer

#### API Layer (ClinicBooking.Api)

- Controllers (thin only)
- Middleware
- Authentication / Authorization
- Swagger
- Request validation
- Mapping Request → Application

MUST NOT contain business logic.

#### Application Layer (ClinicBooking.Application)

- Use cases (Commands / Queries)
- DTOs
- Validators
- Interfaces (abstractions)
- Business workflows

Feature structure:

```text
Features/
├─ Auth/
├─ Appointments/
├─ Doctors/
├─ Patients/
├─ Reception/
├─ Admin/
├─ MedicalRecords/
└─ Notifications/
```

CQRS pattern:

- `Features/<Entity>/Commands/<Action>/` for command + handler
- `Features/<Entity>/Queries/<Action>/` for query + handler

#### Domain Layer (ClinicBooking.Domain)

- Entities
- Enums
- Value Objects
- Domain Events
- Business rules

No dependency on other layers.

#### Infrastructure Layer (ClinicBooking.Infrastructure)

- EF Core / DB access
- JWT authentication
- Email / SMS services
- Logging / caching
- Repository implementations

---

## 3. Build and Run

```bash
# Build entire solution
dotnet build DatLichPhongKham.slnx

# Run the API (Swagger available at /swagger)
dotnet run --project ClinicBooking.Api

# Apply EF Core migrations (run from solution root)
dotnet ef database update --project ClinicBooking.Infrastructure --startup-project ClinicBooking.Api

# Add a new migration
dotnet ef migrations add <MigrationName> --project ClinicBooking.Infrastructure --startup-project ClinicBooking.Api
```

---

## 4. Design Principles

### MUST FOLLOW

- Separation of concerns
- Single Responsibility Principle
- Dependency Injection everywhere
- Feature-based structure in Application
- Thin Controllers
- Use async/await for all I/O operations
- Use DTOs for all API communication

### MUST NOT

- Put business logic in Controllers
- Access DbContext directly from API
- Duplicate logic across modules
- Use magic strings (use enums/constants)
- Mix UI logic into backend
- Hardcode configuration values

---

## 5. Authentication and Authorization

### Use

- JWT Access Token
- Refresh Token
- Role-based Authorization

### Roles

- `admin`
- `le_tan`
- `bac_si`
- `benh_nhan`

### Example

```csharp
[Authorize(Roles = "admin")]
```

---

## 6. Development Workflow

### Standard Flow

1. Define Use Case
2. Create DTO
3. Implement Command/Query
4. Write Validator
5. Implement Handler
6. Add API Endpoint
7. Test (Unit + Integration)
8. Commit

### Code to Test to Build

```text
Code → Run Unit Tests → Run Integration Tests → Build → Run API → Manual Test
```

---

## 7. Testing Strategy

### Unit Tests

- Test business logic
- Mock dependencies

### Integration Tests

- Test DB + API flow

### API Tests

- Endpoint validation
- Auth behavior

---

## 8. Configuration Rules

### MUST use

- appsettings.json
- appsettings.Development.json
- Environment variables

### NEVER

- Hardcode secrets
- Store credentials in code

---

## 9. Deployment Strategy

### Use

- Docker
- Docker Compose

Services:

- API
- Database (SQL Server / PostgreSQL)
- Redis (optional)

### CI/CD (GitHub Actions)

Pipeline:

```text
Push → Test → Build → Dockerize → Deploy
```

---

## 10. Tech Stack

### Allowed

- .NET 8 Web API
- Entity Framework Core
- SQL Server / PostgreSQL
- JWT Authentication
- MediatR (optional but recommended)
- FluentValidation
- AutoMapper
- Docker

### Optional

- Redis (caching)
- Hangfire / Quartz (background jobs)
- SignalR (real-time queue)

### Not Allowed

- Microservices (overkill for this project)
- Direct SQL string everywhere
- Massive Controllers (>300 lines)
- Business logic inside Infrastructure
- Tight coupling between layers

---

## 11. Naming Conventions

### Vietnamese Naming Rule

All variables, classes, methods, and properties MUST be named in:

- Vietnamese language
- Without diacritics (khong dau)
- Using meaningful domain-based names

### Examples

| Type | Example |
|---|---|
| Class | `BenhNhan`, `BacSi`, `LichHen`, `HoaDon` |
| Variable | `tenBenhNhan`, `ngayKham`, `trangThai` |
| Method | `taoLichHen()`, `huyLichHen()` |
| DTO | `TaoLichHenRequest`, `LichHenResponse` |

### Not Allowed

- English naming (e.g., `Patient`, `Appointment`)
- Vietnamese with diacritics (e.g., `BệnhNhân`, `LịchHẹn`)
- Abbreviations without meaning (e.g., `bn`, `lh`, `tmp`)
- Mixed language naming (e.g., `benhNhanService` + `PatientController`)

### Exceptions

The following MUST remain in English:

- Framework-related classes/folders (Controller, Service, Repository, Middleware)
- Third-party libraries
- Standard technical terms (e.g., `Token`, `Cache`, `Email`)

### Good Example

```csharp
public class BenhNhan
{
    public string TenBenhNhan { get; set; }
    public DateTime NgaySinh { get; set; }
}

public class TaoLichHenRequest
{
    public int BenhNhanId { get; set; }
    public DateTime NgayKham { get; set; }
}
```

### Bad Example

```csharp
public class Patient
{
    public string TenBenhNhanCoDau { get; set; }
}

public class bn
{
}
```

---

## 12. Database Rules

- Use EF Core Migrations
- One DbContext only
- Use Fluent API, not Data Annotations for complex config
- Normalize data properly

### Database ID Strategy

- All primary keys MUST use `int` with auto-increment
- GUID/UUID MUST NOT be used as primary keys
- Optional: public identifiers may be added for external exposure

### Database Schema Source of Truth

The database schema is defined in `/database/clinic.dbml`.

Use this file as the source of truth to generate:

- Entities (Domain layer)
- DbContext (Infrastructure layer)
- Relationships

---

## 13. Notifications System

Supports:

- Email
- SMS (optional)
- In-app

Must be:

- Async
- Decoupled via interface

---

## 14. Background Jobs

Use for:

- Appointment reminders
- Cleaning expired holds (giu_cho)
- OTP cleanup
- Status updates

---

## 15. Anti-Patterns to Avoid

- God Controller
- Fat Service Layer without separation
- Copy-paste logic
- Tight coupling between modules
- Ignoring validation

---

## 16. Scalability Strategy

Future upgrades:

- Extract modules to microservices
- Add message queue (RabbitMQ/Kafka)
- Add caching layer
- Add API Gateway

---

## 17. Coding Rules for AI / Claude

When generating code:

### MUST

- Follow Clean Architecture
- Respect folder structure
- Use DTOs
- Keep methods small
- Use async methods
- Add basic validation

### MUST NOT

- Generate monolithic classes
- Skip layers
- Mix concerns
- Ignore error handling

---

## 18. Error Handling and API Responses

- MUST use a standard wrapper or standardized responses, for example `Result<T>` or .NET 8 `ProblemDetails`.
- Global Exception Handler is REQUIRED, using standard Middleware or `IExceptionHandler`.
- HTTP Status Codes MUST be semantically correct: 200 OK, 201 Created, 400 Bad Request, 401/403, 404 Not Found, 409 Conflict.

### Language Rules

- Code syntax, class names, variables, and comments MUST be in English.
- Validation messages, error messages, and UI text returned to the client MUST be in Vietnamese.

---

## 19. Final Principles

- Keep it simple first
- Optimize later
- Avoid premature complexity
- Write maintainable code over clever code


