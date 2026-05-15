# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## 1. Project Overview

**ClinicBooking** (DatLichPhongKham) is an ASP.NET Core 8 application for clinic appointment booking, consisting of a REST API backend and a Web frontend.

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
├── ClinicBooking.Domain                   # Entities, Enums - no dependencies
├── ClinicBooking.Application              # Business logic, CQRS via MediatR, DTOs, validators
├── ClinicBooking.Infrastructure           # EF Core (SQL Server), Identity, external services
├── ClinicBooking.Api                      # ASP.NET Core REST API, controllers, middleware
├── ClinicBooking.Web                      # ASP.NET Core Web frontend (Razor/MVC), demo UI
├── ClinicBooking.Application.UnitTests    # Unit tests for Application layer
├── ClinicBooking.Integration.Tests        # Integration tests
└── ClinicBooking.IntegrationTests         # Integration tests (alternate suite)
```

Dependency direction: Api/Web → Application + Infrastructure → Application → Domain.

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

<!-- gitnexus:start -->
# GitNexus — Code Intelligence

This project is indexed by GitNexus as **DAPM** (12998 symbols, 25893 relationships, 300 execution flows). Use the GitNexus MCP tools to understand code, assess impact, and navigate safely.

> If any GitNexus tool warns the index is stale, run `npx gitnexus analyze` in terminal first.

## Always Do

- **MUST run impact analysis before editing any symbol.** Before modifying a function, class, or method, run `gitnexus_impact({target: "symbolName", direction: "upstream"})` and report the blast radius (direct callers, affected processes, risk level) to the user.
- **MUST run `gitnexus_detect_changes()` before committing** to verify your changes only affect expected symbols and execution flows.
- **MUST warn the user** if impact analysis returns HIGH or CRITICAL risk before proceeding with edits.
- When exploring unfamiliar code, use `gitnexus_query({query: "concept"})` to find execution flows instead of grepping. It returns process-grouped results ranked by relevance.
- When you need full context on a specific symbol — callers, callees, which execution flows it participates in — use `gitnexus_context({name: "symbolName"})`.

## Never Do

- NEVER edit a function, class, or method without first running `gitnexus_impact` on it.
- NEVER ignore HIGH or CRITICAL risk warnings from impact analysis.
- NEVER rename symbols with find-and-replace — use `gitnexus_rename` which understands the call graph.
- NEVER commit changes without running `gitnexus_detect_changes()` to check affected scope.

## Resources

| Resource | Use for |
|----------|---------|
| `gitnexus://repo/DAPM/context` | Codebase overview, check index freshness |
| `gitnexus://repo/DAPM/clusters` | All functional areas |
| `gitnexus://repo/DAPM/processes` | All execution flows |
| `gitnexus://repo/DAPM/process/{name}` | Step-by-step execution trace |

## CLI

| Task | Read this skill file |
|------|---------------------|
| Understand architecture / "How does X work?" | `.claude/skills/gitnexus/gitnexus-exploring/SKILL.md` |
| Blast radius / "What breaks if I change X?" | `.claude/skills/gitnexus/gitnexus-impact-analysis/SKILL.md` |
| Trace bugs / "Why is X failing?" | `.claude/skills/gitnexus/gitnexus-debugging/SKILL.md` |
| Rename / extract / split / refactor | `.claude/skills/gitnexus/gitnexus-refactoring/SKILL.md` |
| Tools, resources, schema reference | `.claude/skills/gitnexus/gitnexus-guide/SKILL.md` |
| Index, status, clean, wiki CLI commands | `.claude/skills/gitnexus/gitnexus-cli/SKILL.md` |

<!-- gitnexus:end -->

<!-- rtk-instructions v2 -->
# RTK (Rust Token Killer) - Token-Optimized Commands

## Golden Rule

**Always prefix commands with `rtk`**. If RTK has a dedicated filter, it uses it. If not, it passes through unchanged. This means RTK is always safe to use.

**Important**: Even in command chains with `&&`, use `rtk`:
```bash
# ❌ Wrong
git add . && git commit -m "msg" && git push

# ✅ Correct
rtk git add . && rtk git commit -m "msg" && rtk git push
```

## RTK Commands by Workflow

### Build & Compile (80-90% savings)
```bash
rtk cargo build         # Cargo build output
rtk cargo check         # Cargo check output
rtk cargo clippy        # Clippy warnings grouped by file (80%)
rtk tsc                 # TypeScript errors grouped by file/code (83%)
rtk lint                # ESLint/Biome violations grouped (84%)
rtk prettier --check    # Files needing format only (70%)
rtk next build          # Next.js build with route metrics (87%)
```

### Test (60-99% savings)
```bash
rtk cargo test          # Cargo test failures only (90%)
rtk go test             # Go test failures only (90%)
rtk jest                # Jest failures only (99.5%)
rtk vitest              # Vitest failures only (99.5%)
rtk playwright test     # Playwright failures only (94%)
rtk pytest              # Python test failures only (90%)
rtk rake test           # Ruby test failures only (90%)
rtk rspec               # RSpec test failures only (60%)
rtk test <cmd>          # Generic test wrapper - failures only
```

### Git (59-80% savings)
```bash
rtk git status          # Compact status
rtk git log             # Compact log (works with all git flags)
rtk git diff            # Compact diff (80%)
rtk git show            # Compact show (80%)
rtk git add             # Ultra-compact confirmations (59%)
rtk git commit          # Ultra-compact confirmations (59%)
rtk git push            # Ultra-compact confirmations
rtk git pull            # Ultra-compact confirmations
rtk git branch          # Compact branch list
rtk git fetch           # Compact fetch
rtk git stash           # Compact stash
rtk git worktree        # Compact worktree
```

Note: Git passthrough works for ALL subcommands, even those not explicitly listed.

### GitHub (26-87% savings)
```bash
rtk gh pr view <num>    # Compact PR view (87%)
rtk gh pr checks        # Compact PR checks (79%)
rtk gh run list         # Compact workflow runs (82%)
rtk gh issue list       # Compact issue list (80%)
rtk gh api              # Compact API responses (26%)
```

### JavaScript/TypeScript Tooling (70-90% savings)
```bash
rtk pnpm list           # Compact dependency tree (70%)
rtk pnpm outdated       # Compact outdated packages (80%)
rtk pnpm install        # Compact install output (90%)
rtk npm run <script>    # Compact npm script output
rtk npx <cmd>           # Compact npx command output
rtk prisma              # Prisma without ASCII art (88%)
```

### Files & Search (60-75% savings)
```bash
rtk ls <path>           # Tree format, compact (65%)
rtk read <file>         # Code reading with filtering (60%)
rtk grep <pattern>      # Search grouped by file (75%)
rtk find <pattern>      # Find grouped by directory (70%)
```

### Analysis & Debug (70-90% savings)
```bash
rtk err <cmd>           # Filter errors only from any command
rtk log <file>          # Deduplicated logs with counts
rtk json <file>         # JSON structure without values
rtk deps                # Dependency overview
rtk env                 # Environment variables compact
rtk summary <cmd>       # Smart summary of command output
rtk diff                # Ultra-compact diffs
```

### Infrastructure (85% savings)
```bash
rtk docker ps           # Compact container list
rtk docker images       # Compact image list
rtk docker logs <c>     # Deduplicated logs
rtk kubectl get         # Compact resource list
rtk kubectl logs        # Deduplicated pod logs
```

### Network (65-70% savings)
```bash
rtk curl <url>          # Compact HTTP responses (70%)
rtk wget <url>          # Compact download output (65%)
```

### Meta Commands
```bash
rtk gain                # View token savings statistics
rtk gain --history      # View command history with savings
rtk discover            # Analyze Claude Code sessions for missed RTK usage
rtk proxy <cmd>         # Run command without filtering (for debugging)
rtk init                # Add RTK instructions to CLAUDE.md
rtk init --global       # Add RTK to ~/.claude/CLAUDE.md
```

## Token Savings Overview

| Category | Commands | Typical Savings |
|----------|----------|-----------------|
| Tests | vitest, playwright, cargo test | 90-99% |
| Build | next, tsc, lint, prettier | 70-87% |
| Git | status, log, diff, add, commit | 59-80% |
| GitHub | gh pr, gh run, gh issue | 26-87% |
| Package Managers | pnpm, npm, npx | 70-90% |
| Files | ls, read, grep, find | 60-75% |
| Infrastructure | docker, kubectl | 85% |
| Network | curl, wget | 65-70% |

Overall average: **60-90% token reduction** on common development operations.
<!-- /rtk-instructions -->

<!-- code-review-graph MCP tools -->
## MCP Tools: code-review-graph

**IMPORTANT: This project has a knowledge graph. ALWAYS use the
code-review-graph MCP tools BEFORE using Grep/Glob/Read to explore
the codebase.** The graph is faster, cheaper (fewer tokens), and gives
you structural context (callers, dependents, test coverage) that file
scanning cannot.

### When to use graph tools FIRST

- **Exploring code**: `semantic_search_nodes` or `query_graph` instead of Grep
- **Understanding impact**: `get_impact_radius` instead of manually tracing imports
- **Code review**: `detect_changes` + `get_review_context` instead of reading entire files
- **Finding relationships**: `query_graph` with callers_of/callees_of/imports_of/tests_for
- **Architecture questions**: `get_architecture_overview` + `list_communities`

Fall back to Grep/Glob/Read **only** when the graph doesn't cover what you need.

### Key Tools

| Tool | Use when |
| ------ | ---------- |
| `detect_changes` | Reviewing code changes — gives risk-scored analysis |
| `get_review_context` | Need source snippets for review — token-efficient |
| `get_impact_radius` | Understanding blast radius of a change |
| `get_affected_flows` | Finding which execution paths are impacted |
| `query_graph` | Tracing callers, callees, imports, tests, dependencies |
| `semantic_search_nodes` | Finding functions/classes by name or keyword |
| `get_architecture_overview` | Understanding high-level codebase structure |
| `refactor_tool` | Planning renames, finding dead code |

### Workflow

1. The graph auto-updates on file changes (via hooks).
2. Use `detect_changes` for code review.
3. Use `get_affected_flows` to understand impact.
4. Use `query_graph` pattern="tests_for" to check coverage.
