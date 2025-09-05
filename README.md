# ScheduleIt

Clean, production-lean scheduling app demonstrating .NET 9 Clean Architecture with DDD, CQRS, EF Core, domain events, validation, React (TypeScript) frontend, and Docker Compose.

### Why this project
- Show practical application of DDD and CQRS in a compact, end-to-end app
- Balance correctness with pragmatism: simple DTOs, explicit domain rules, and clear API contracts
- Be easy to run locally via Docker, with a pleasant minimal UI to explore features

## Highlights
- **Clean Architecture**: `Domain`, `Application`, `Infrastructure`, `Api`, `App` (host), `FrontEnd`
- **DDD**: Rich `Appointment` aggregate with invariants and a dedicated `DomainRuleViolationException`
- **CQRS with MediatR**: Commands/Queries for appointments and customers
- **Validation**: FluentValidation on command layer (executed via MediatR pipeline behavior) + value object guards in domain
- **Domain events**: `AppointmentBooked`, `AppointmentCanceled` published via EF Core + MediatR
- **EF Core**: SQL Server provider, code-first migrations, repository + unit-of-work
- **API**: Swagger/OpenAPI with XML comments, typed error mapping via `ProblemDetailsMapper`
- **Frontend**: React + Vite + `react-big-calendar` with customer autocomplete and inline booking
- **Testing**: xUnit tests across Domain and Application layers
- **Docker Compose**: API, DB, and Frontend wired together

## Repository layout
```text
BackEnd/
├─ ScheduleIt.sln
├─ Domain/                   # Entities, ValueObjects, Domain events, exceptions
├─ Application/              # CQRS handlers, DTOs, validators, app exceptions
├─ Infrastructure/           # EF Core DbContext, repositories, migrations, UoW
├─ Api/                      # Controllers, error mapping
├─ App/                      # ASP.NET Core host (Program.cs, DI, Swagger)
├─ db-scripts/               # SQL bootstrap used by Docker db-init
└─ Tests/
   └─ ScheduleIt.XUnit/      # Domain and Application unit tests

FrontEnd/
└─ scheduleit/               # React + Vite app (TypeScript)

docker-compose.yml           # API + Frontend + SQL Server services
```

## Architecture overview
- **Domain**
  - `Appointment` aggregate enforces rules like: cannot cancel after start, cannot move to invalid states, only scheduled appointments can be deleted.
  - `AppointmentTimeSlot` value object guards input (no past scheduling, min 30 minutes, max 12 hours).
  - Domain rule breaches use `DomainRuleViolationException` (aggregate state violations).
  - Events: `AppointmentBooked`, `AppointmentCanceled`.
- **Application**
  - CQRS via MediatR: e.g., `BookAppointmentCommand`, `UpdateAppointmentStatusCommand`, `GetAppointmentsInRangeQuery`, `SearchCustomersQuery`.
  - Validators (FluentValidation) for commands; throws `ValidationException` for application-layer validation.
  - App-specific errors include `NotFoundException` (mapped to 404).
- **Infrastructure**
  - `AppDbContext` publishes domain events after `SaveChangesAsync` using MediatR.
  - SQL Server provider; repositories implement `ICustomerRepository` and `IAppointmentRepository` with `UnitOfWork`.
  - Migrations generated and tracked; `BackEnd/db-scripts/migrations.sql` helps bootstrap in Docker.
- **API**
  - Controllers are small and delegate to MediatR. Errors are mapped centrally by `ProblemDetailsMapper` via global middleware:
    - `NotFoundException` → 404
    - `DomainRuleViolationException`, `ValidationException` → 400
    - others → 500
  - Swagger UI enabled at `/swagger` in Development.
- **Frontend**
  - React + Vite. Calendar view with `react-big-calendar` to browse and manage appointments.
  - Book appointments with customer autocomplete and inline new customer creation.
  - Update status and delete directly in the UI with optimistic refresh.

## Running the stack

### Option A: Docker (recommended)
Prereqs: Docker Desktop

```bash
docker compose up --build
```

Services:
- API host (`App`): `http://localhost:8080`
- Swagger UI: `http://localhost:8080/swagger`
- Frontend (Vite dev): `http://localhost:5173`
- SQL Server: `localhost,3333` (DB `ScheduleIt`, user `sa`, pass `MyP@ssw0rd123`)

Compose sets `ConnectionStrings__Database` so the API connects to the `sqlserver` container.

The `db-init` service creates the DB and runs `BackEnd/db-scripts/migrations.sql` on first startup.

### Option B: Local development
Prereqs: .NET 9 SDK, Node 20+, SQL Server (or run the DB via Docker Compose)

1) Start API (Development)
```bash
dotnet run --project BackEnd/App/App.csproj
```

2) Start Frontend
```bash
cd FrontEnd/scheduleit
npm ci
npm run dev
```

3) Configure Frontend API base URL
- By default, frontend uses `VITE_API_BASE_URL` if provided; otherwise it falls back to `http://localhost:5522`. When running Docker Compose, it is set to `http://localhost:8080`.
- To override locally:
```bash
# in FrontEnd/scheduleit
set VITE_API_BASE_URL=http://localhost:8080
npm run dev
```

More details: see `SQL_SERVER_SETUP.md` and `SWAGGER_SETUP.md`.

## Core scenarios
- **Create customer**: `POST /api/customers` with `{ name, email }`
- **Search customers**: `GET /api/customers?search=jo`
- **Book appointment**: `POST /api/appointments` with `{ customerId, startUtc, endUtc, notes? }`
- **Get appointments in range**: `GET /api/appointments/range?startUtc=...&endUtc=...`
- **Update status**: `PATCH /api/appointments/{id}/status` with `{ appointmentId, status }`
- **Delete appointment**: `DELETE /api/appointments/{id}`

Open the frontend calendar to visualize, book, edit, and delete appointments.

## Error handling and validation
- Domain invariants throw `DomainRuleViolationException` (mapped to 400).
- Application validation failures throw `ValidationException` (400).
- Missing aggregates throw `NotFoundException` (404).
- Unexpected errors return 500 with a generic message.


## Testing
- Project: `BackEnd/Tests/ScheduleIt.XUnit`
- Coverage:
  - Domain: `Appointment` invariants, `AppointmentTimeSlot` guards
  - Application: command handlers for booking, updating status, deleting, and queries

Run tests:
```bash
dotnet test
```

## Design decisions and trade-offs
- Keep the domain expressive: domain events and exceptions live in `Domain`; app-level concerns in `Application`.
- Use repositories and unit-of-work to isolate EF Core from the application layer.
- Prefer explicit error mapping over global filters to keep controller intent visible for this small app.
- Frontend chooses simplicity over heavy state management; server remains source of truth.

## API exploration
- Swagger UI at `http://localhost:8080/swagger`


## Security and environment notes
- Dev-only SQL credentials in Docker Compose for local convenience.
- CORS allows `http://localhost:5173` by default; adjust in `App/Program.cs` for other origins.

## Future enhancements (roadmap)
- Add authentication/authorization and multi-tenant boundaries
- Introduce pagination and filtering for large datasets
- Add integration tests (API level) and contract tests for the frontend services
- Implement background processing for reminders/notifications using domain events

—
Built with care to highlight clear architecture, readable code, and a smooth dev experience.