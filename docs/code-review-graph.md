# Code Review Graph

This graph is tailored to the current codebase and highlights dependency direction, runtime request flow, and high-risk review checkpoints.

## 1) Project Dependency Graph (Static)

```mermaid
flowchart LR
    API[ClinicBooking.Api] --> APP[ClinicBooking.Application]
    API --> INF[ClinicBooking.Infrastructure]
    INF --> APP
    APP --> DOM[ClinicBooking.Domain]

    classDef review fill:#fff4cc,stroke:#946200,color:#222,stroke-width:1px;
    class API,APP,INF,DOM review;
```

Review focus:
- Ensure dependency direction remains inward-only toward Domain.
- Prevent business rules in API and Infrastructure layers.

## 2) Runtime Auth Flow Graph (CQRS)

```mermaid
flowchart TD
    HTTP[HTTP Request /api/auth/*]
    CTRL[AuthController]
    MED[IMediator.Send]
    VB[ValidationBehavior]

    subgraph APP[Application Auth Commands]
      DK[DangKyCommand + DangKyHandler]
      DN[DangNhapCommand + DangNhapHandler]
      LMT[LamMoiTokenCommand + LamMoiTokenHandler]
      DX[DangXuatCommand + DangXuatHandler]
    end

    subgraph ABS[Application Abstractions]
      IDB[IAppDbContext]
      ISEC[ITokenService / IPasswordHasher / ICurrentUserService / IDateTimeProvider]
    end

    subgraph INF[Infrastructure Implementations]
      DB[AppDbContext]
      TOK[TokenService]
      PWD[PasswordHasher]
      CUR[CurrentUserService]
    end

    subgraph DOM[Domain]
      TK[TaiKhoan]
      RT[RefreshToken]
      BN[BenhNhan]
      VT[VaiTro enum/constants mapping]
    end

    EXH[GlobalExceptionHandler + ProblemDetails]

    HTTP --> CTRL --> MED --> VB
    VB --> DK
    VB --> DN
    VB --> LMT
    VB --> DX

    DK --> IDB
    DK --> ISEC
    DN --> IDB
    DN --> ISEC
    LMT --> IDB
    LMT --> ISEC
    DX --> IDB
    DX --> ISEC

    IDB --> DB
    ISEC --> TOK
    ISEC --> PWD
    ISEC --> CUR

    DB --> TK
    DB --> RT
    DB --> BN
    TOK --> VT
    CUR --> VT

    CTRL -. exceptions .-> EXH
    VB -. validation exceptions .-> EXH
    DK -. domain/app exceptions .-> EXH
    DN -. domain/app exceptions .-> EXH
    LMT -. domain/app exceptions .-> EXH
    DX -. domain/app exceptions .-> EXH

    classDef risk fill:#ffe4e1,stroke:#b42318,color:#222,stroke-width:1px;
    classDef review fill:#e8f4ff,stroke:#175cd3,color:#222,stroke-width:1px;

    class VB,DK,DN,LMT,DX risk;
    class CTRL,IDB,DB,TOK,CUR,EXH review;
```

Review focus:
- Validation order and completeness in validators.
- Authorization and ownership checks in `DangXuatHandler`.
- Refresh token rotation and replay handling in `LamMoiTokenHandler`.
- Transaction boundaries and persistence consistency around token issuance.
- ProblemDetails mapping correctness and leakage-safe error details.

## 3) Build/Config Flow Graph

```mermaid
flowchart LR
    CFG[appsettings*.json Jwt + ConnectionStrings] --> PROG[Program.cs]
    PROG --> DIAPP[AddApplication]
    PROG --> DIINF[AddInfrastructure]
    PROG --> DIAUTH[AddJwtAuthentication]
    DIINF --> SQL[SQL Server via EF Core]
    DIAUTH --> JWT[JwtBearer middleware]
    PROG --> PIPE[UseExceptionHandler -> UseAuthentication -> UseAuthorization -> MapControllers]
```

Review focus:
- Required configuration keys are validated at startup.
- Authentication/authorization middleware order remains correct.
- DB provider and migration compatibility remain aligned with runtime environment.

## Suggested Review Order

1. API entrypoints (`Program.cs`, controllers, middleware wiring).
2. Application pipeline (`ValidationBehavior`, command handlers, validators).
3. Security-critical infrastructure (`TokenService`, `CurrentUserService`).
4. Persistence contracts and EF implementation (`IAppDbContext`, `AppDbContext`).
5. Domain entities/enums affected by reviewed feature.
