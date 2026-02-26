# CT — Rental Management System

Full-stack web application for managing a tourist complex: bookings, guests, apartments, payments, and users. Built with **Clean Architecture**, **ASP.NET Core 8**, **Blazor Server**, and **SQL Server** (Azure SQL in production).

---

## Features

- Booking management with overlap detection and availability control
- Payment tracking (deposits, balances, extras) with ACID transactions
- Quarterly occupancy calendar
- Occupancy statistics: occupancy rate, revenue, today's check-ins/check-outs
- Guest and apartment management
- Cookie-based authentication (PBKDF2 + timing-safe verification)
- Rate limiting on login endpoint to prevent brute-force attacks
- Global exception handling middleware with domain-aware HTTP status mapping
- User administration panel with strong password policy

---

## Tech Stack

| Component | Technology |
|---|---|
| Backend / Web | ASP.NET Core 8, Blazor Server |
| ORM | Entity Framework Core 8 |
| Database | SQL Server (Azure SQL in production) / SQLite in development |
| Authentication | Cookie Authentication (PBKDF2 SHA-256) |
| Testing | xUnit, Moq, FluentAssertions |
| Coverage | coverlet |

---

## Architecture

The project follows **Clean Architecture** with 4 well-defined layers:

```
CT.Domain          ← Entities, interfaces, enums, domain exceptions
CT.Application     ← Use Cases, DTOs, validators
CT.Infraestructure ← EF Core, repositories, PasswordHasher, UnitOfWork
CT.Web             ← Blazor Server, controllers, DI composition root
CT.Tests           ← xUnit + Moq + FluentAssertions
```

### Dependency diagram

```
CT.Web
  └── CT.Application
  └── CT.Infraestructure
        └── CT.Application
              └── CT.Domain
```

`CT.Domain` has no dependencies on any other project in the solution.

### Applied patterns

- **Repository Pattern** — `IRepositorioDepartamento`, `IRepositorioReserva`, etc.
- **Unit of Work** — atomic transactions in `PagoRegistrarUseCase` and `ReservaAltaUseCase`
- **Use Case Pattern** — each business operation is an independent class
- **Fluent API (EF Core)** — entity configurations separated by file
- **DTO Pattern** — clean separation between domain model and transfer model
- **Dependency Inversion** — interfaces defined in Domain, implemented in Infrastructure

---

## Project Structure

```
CT/
├── CT.Domain/
│   ├── Entities/          # Cliente, Departamento, Reserva, Pago, User
│   ├── Enums/             # EstadoReserva, MedioPago, OrigenReserva, etc.
│   ├── Exceptions/        # Typed domain exceptions
│   └── Interfaces/        # IRepositorio*, IUnitOfWork, IPasswordHasher
│
├── CT.Application/
│   ├── DTOs/              # 22 DTOs + MappingExtensions
│   ├── UseCases/          # 29 use cases organized by entity
│   └── Validations/       # Validators using Regex source generators
│
├── CT.Infraestructure/
│   ├── Data/
│   │   ├── CTDbContext.cs
│   │   └── Configurations/ # Fluent API per entity
│   ├── Migrations/
│   ├── Repositories/      # IRepositorio* implementations
│   └── Services/          # PasswordHasher (PBKDF2), UnitOfWork
│
├── CT.Web/
│   ├── Controllers/       # AuthController (login/logout + rate limiting)
│   ├── Middleware/         # GlobalExceptionMiddleware
│   ├── Services/          # CustomAuthStateProvider
│   ├── Components/        # 20 Blazor components
│   │   ├── Pages/         # Home, Bookings, Guests, Apartments, Payments, Users
│   │   ├── Layout/
│   │   └── Shared/        # QuarterlyCalendar
│   └── Program.cs
│
└── CT.Tests/
    ├── UseCases/          # Use case tests with Moq
    └── Validators/        # Parametric tests with [Theory]
```

---

## Security

- **Password hashing**: PBKDF2 with SHA-256, 100,000 iterations, 16-byte random salt.
- **Timing-safe comparison**: `CryptographicOperations.FixedTimeEquals` to prevent timing attacks.
- **Cookie auth**: `HttpOnly`, `SameSite=Strict`, 7-day sliding expiration.
- **User enumeration prevention**: identical error message for wrong email and wrong password.
- **Rate limiting**: Fixed-window limiter on `/api/auth/login` (5 requests/minute) to prevent brute-force attacks.
- **Global exception middleware**: Catches all domain exceptions and maps them to appropriate HTTP status codes (400, 401, 404, 409), preventing stack trace leakage to clients.
- **Password policy**: minimum 8 characters, at least one uppercase letter and one digit.
- **Unique indexes on DNI and Email** to prevent duplicates at the database level.
- **ACID transactions**: Overlap check + booking creation wrapped in a single transaction in `ReservaAltaUseCase` to prevent race conditions.

---

## Testing

Stack: **xUnit** + **Moq** + **FluentAssertions** + **coverlet**

| Module | Tests |
|---|---|
| `UserLoginUseCase` | 8 tests |
| `UserCambiarPasswordUseCase` | 5 tests |
| `ReservaAltaUseCase` | 9 tests |
| `ReservaCancelar/Confirmar/FinalizarUseCase` | 14 tests |
| `PagoRegistrarUseCase` | 6 tests (includes rollback) |
| `UserValidator` | 12 parametric tests |
| `ClienteValidator` | 7 tests |
| `ReservaValidator` | 5 tests |

`PagoRegistrarUseCase` tests verify that on a persistence error the transaction is rolled back (never committed). Login tests verify that the error message is identical for wrong email and wrong password (anti user enumeration).

---

## Getting Started

### Requirements

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### Steps

```bash
# 1. Clone the repository
git clone https://github.com/Juanarena29/CT.git
cd CT

# 2. Set your connection string in CT.Web/appsettings.Development.json
# (defaults to SQLite locally; replace with your SQL Server connection string for production)

# 3. Apply migrations and create the database
dotnet ef database update --project CT.Infraestructure --startup-project CT.Web

# 4. Run the application
dotnet run --project CT.Web
```

The application will be available at `http://localhost:5003` (or `https://localhost:7020` for HTTPS).

### Default user

| Field | Value |
|---|---|
| Email | admin@ct.com |
| Password | *(you will be prompted to change it on first login)* |

### Run the tests

```bash
dotnet test CT.Tests
```

---

## Design Decisions

**Why Use Cases instead of Services?**
Each Use Case encapsulates exactly one business operation. This makes unit testing straightforward, allows replacing individual logic pieces independently, and improves onboarding for new developers.

**Why manual mapping instead of AutoMapper?**
An external dependency was avoided for a mapping that is direct and fully controlled. `MappingExtensions` as extension methods keeps the code explicit with no "magic" behavior.

**Why SQL Server / Azure SQL in production?**
In production, **Azure SQL Server** is used to leverage high availability, automated backups, and managed cloud scalability. In local development, SQLite can be used (by changing the connection string and EF Core provider) to simplify setup without requiring an external server. The separation between configuration and EF Core implementation makes the switch a single-line change in `appsettings.json`.

**Why store enums as strings in the DB?**
Direct readability when querying the database. The space overhead is negligible for the expected data volume.

---

## Planned Improvements

- [x] Global exception handling middleware
- [x] Rate limiting on the login endpoint
- [x] ACID transaction in `ReservaAltaUseCase` (overlap check + creation)
- [ ] `ILogger` logging in critical use cases
- [ ] Pagination on list endpoints
- [ ] Granular roles and authorization
- [ ] Integration tests with SQL Server in-memory / SQLite in-memory
- [ ] Extension methods for dependency registration in `Program.cs`

---

## License

MIT
