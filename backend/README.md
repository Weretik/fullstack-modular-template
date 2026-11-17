# Backend

A .NET 10 modular monolith on PostgreSQL. One deployable API (`Host.Api`) hosts three modules; each module owns its
own schema, its own use cases and its own public surface.

- [Architecture](docs/architecture/README.md): modules, layers and what may reference what.
- [C4 diagrams](docs/architecture/c4.md): the system, its containers and the inside of the API.
- [Rules](docs/rules.md): the conventions every change follows; most are checked by tests.
- [Decisions](docs/decisions/README.md): why it is built this way.
- [API contract](docs/contracts/openapi.json): generated from the code on every build.

## Layout

```
src/
  BuildingBlocks/   Result and Error, validation pipeline, outbox, persistence helpers, problem responses
  Modules/
    Identity/       Identity.Api, .Application, .Contracts, .Domain, .Infrastructure
    Catalog/        same five projects
    Orders/         same five projects
  Host/Host.Api/    Composition only: registers modules, middleware, OpenAPI
tests/
  UnitTests/          domain rules and handlers, no database
  ArchitectureTests/  boundaries, layer dependencies, folder and naming rules, OpenAPI rules
  IntegrationTests/   the real API against PostgreSQL in Testcontainers
docs/
```

Package versions live in `Directory.Packages.props`, shared build settings in `Directory.Build.props` (nullable
reference types, warnings as errors, XML docs).

## Commands

| What | Command |
| --- | --- |
| Start PostgreSQL | `docker compose up -d` |
| Run the API on http://localhost:5000 | `dotnet run --project src/Host/Host.Api` |
| Build (regenerates `docs/contracts/openapi.json`) | `dotnet build ModularTemplate.slnx` |
| Unit tests | `dotnet test tests/UnitTests` |
| Architecture tests | `dotnet test tests/ArchitectureTests` |
| Integration tests (need Docker) | `dotnet test tests/IntegrationTests` |
| One test class | `dotnet test tests/UnitTests --filter FullyQualifiedName~PlaceOrderHandlerTests` |
| Restore local tools (`dotnet-ef`) | `dotnet tool restore` |

In Development the API applies migrations and seeds a staff account and a sample catalog on start
(`Database:InitializeOnStartup`). In other environments run migrations as a separate step before the deploy.

## Configuration

| Key | Meaning |
| --- | --- |
| `ConnectionStrings:Database` | PostgreSQL connection string; all modules share the database, each in its own schema. |
| `Database:InitializeOnStartup` | Apply migrations and seed data on start. Development only. |
| `Identity:SeedAdmin:Email`, `:Password` | The first staff account, created when it does not exist. |
| `Cors:Origins` | Browser origins allowed to call the API with credentials. |
| `OutboxTick:Key` | Shared secret for `POST /internal/outbox/tick`; empty disables the endpoint. |
| `RateLimiting:PublicWrite`, `:SignIn` | `PermitLimit` and `WindowSeconds` per client address for anonymous writes and sign-in ([ADR 0012](docs/decisions/0012-rate-limits-on-anonymous-writes.md)). |

Keep real values in user secrets or environment variables (`ConnectionStrings__Database=...`), never in the
repository.

## Add a use case

A use case is one folder in the module's `Application` project and one endpoint file in its `Api` project.
For example, "archive a product":

```
Catalog.Application/Features/Products/Commands/ArchiveProduct/
  ArchiveProductCommand.cs     record ArchiveProductCommand(Guid Id) : ICommand<Result>
  ArchiveProductValidator.cs   FluentValidation rules for the input shape
  ArchiveProductHandler.cs     loads the product, calls the domain method, saves once
Catalog.Api/Endpoints/Products/
  ArchiveProductEndpoint.cs    maps the route, sends the command, returns result.ToHttpResult(http)
```

1. Write the test first: a domain test for the new rule and a handler test in `tests/UnitTests`.
2. Put the rule in the domain (`product.Archive()` returns a `Result`); return a typed error from
   `ProductErrors` when it fails.
3. The handler loads, calls the domain, and calls `SaveChangesAsync` once.
4. The endpoint gives the operation a unique camelCase `WithName`, a `WithSummary`, and lists what it produces
   (`Produces<T>`, `ProducesValidationProblem`, `ProducesProblem`). The HTTP request record is separate from the
   command.
5. Build. The OpenAPI document changes; commit it with the code. The frontend picks it up with
   `npm run contracts:generate`.

A folder holds at most six code files. When a seventh arrives, split the folder by meaning (for example
`Features/Products/Commands` and `Features/Products/Queries`).

## Add a module

1. Create the five projects under `src/Modules/<Name>/` with `dotnet new classlib` (and add them to
   `ModularTemplate.slnx` in a solution folder that mirrors the disk). `Api` references its own `Application` and
   `Infrastructure`; `Application` references its own `Domain`, its own `Contracts` and other modules' `Contracts`
   only.
2. Give the module a schema: a `<Name>DbContext : ModuleDbContext` with `Schema = "<name>"`, an
   `I<Name>DbContext` port in `Application`, and a registration method in `Infrastructure`.
3. Add a `<Name>Module` class in `Api` with `Add<Name>Module` and `Map<Name>Module`, then call both from
   `Program.cs`, and add the `Application` assembly to `Composition/MediatorSetup.cs`.
4. Create the first migration:
   `dotnet ef migrations add Initial --project src/Modules/<Name>/<Name>.Infrastructure --startup-project src/Host/Host.Api --context <Name>DbContext --output-dir Persistence/Migrations`.
5. Add the module name to `Modules` in `tests/ArchitectureTests/Repository.cs` so the boundary tests cover it, and write an ADR
   if the module brings a new external system or way of running work.

## Talk to another module

Only through its `Contracts` project. `Orders` needs product prices, so `Catalog.Contracts` exposes
`ICatalogProductReader` and `Catalog.Application` implements it; `Orders.Application` depends on the interface
only. No module reads another module's tables, and there are no foreign keys between schemas.

To react to something that happened in another module without waiting for it, publish a message through the outbox
(below) instead of calling the other module inside the request.

## Outbox and the scheduled tick

`PlaceOrder` adds an `OrderPlaced` message to the Orders outbox table in the same `SaveChanges` as the order, so
either both are stored or neither is. Delivery happens later:

1. A scheduler (Cloud Scheduler, cron, or anything that can send HTTP) calls `POST /internal/outbox/tick` every few
   minutes with the header `X-Scheduler-Key: <OutboxTick:Key>`.
2. Each module claims up to 20 due messages with one conditional `UPDATE` (so two ticks never take the same
   message), runs their handlers, and records the result.
3. A failed message is retried after 1, 4, 9 and 16 minutes, then left for a person to look at; delivered
   messages are deleted after seven days.

There is no background thread, so the API can scale to zero between requests. A handler must be safe to run twice
(check state before acting), because a crash after the work and before the record is always possible.

To add a message: put the record in the publishing module's `Contracts`, a handler implementing
`IOutboxMessageHandler<T>` in the receiving module's `Application`, and register it with
`services.AddOutboxMessage<TMessage, THandler>("<module>.<event-name>")`. The name is stored in the table, so never
change it once released.

## Idempotent requests

`POST /api/orders` takes an `Idempotency-Key` header. The key is stored with the order under a unique index in
the same transaction, with a fingerprint of the request body. A repeat with the same body returns the first order;
the same key with a different body answers `409 Conflict`; two racing copies end with one order. Any create that
moves money or stock, numbers a document, or sends something outside should follow the same pattern.

## Errors

Business rules return `Result` or `Result<T>` with an `Error(code, message, type)`. `ToHttpResult` turns the error
type into a status (`Validation` 400, `Unauthorized` 401, `Forbidden` 403, `NotFound` 404, `Conflict` 409) and a
problem response with `code` and, for validation, `errors` per field. Exceptions are for bugs and outages only;
`GlobalExceptionHandler` logs them and answers 500 without details.

## Extracting a module into a service

Because modules already behave like separate services that happen to share a process, extracting one is mostly
moving code. Take `Orders` as the example. Its schema already has no foreign keys to other schemas, so move the
`orders` tables to their own database. Create a new host that calls `AddOrdersModule` and `MapOrdersModule`; the
module projects do not change. Replace the in-process `ICatalogProductReader` with an HTTP client that implements
the same `Catalog.Contracts` interface against the catalog API, so `Orders.Application` keeps compiling unchanged.
Messages that crossed modules through the outbox now go through a broker or a webhook: the outbox stays, only the
delivery step changes. Do this when a module needs to scale, deploy or fail independently, not before: a single
process is cheaper to run and simpler to debug.
