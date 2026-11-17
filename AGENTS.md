# AI navigation: Fullstack Modular Template

Short entry point for AI agents (Claude Code, Codex). The code and the documents linked here are the source of truth; read only what the task needs. When this template becomes a real project, rewrite the product line and keep the rest.

## Product

Example stationery shop: a .NET 10 modular monolith API (`backend/`) and an Nx workspace with a Next.js storefront and a React admin (`frontend/`). The two halves share only the OpenAPI document `backend/docs/contracts/openapi.json`.

## Where to read

| Question | Backend | Frontend |
| --- | --- | --- |
| Layout and commands | [backend/README.md](backend/README.md) | [frontend/README.md](frontend/README.md) |
| Architecture | [backend/docs/architecture/README.md](backend/docs/architecture/README.md) | [frontend/docs/architecture.md](frontend/docs/architecture.md) |
| Decisions (ADR) | [backend/docs/decisions/README.md](backend/docs/decisions/README.md) | [frontend/docs/decisions/README.md](frontend/docs/decisions/README.md) |
| Rules | [backend/docs/rules.md](backend/docs/rules.md) | [frontend/docs/rules.md](frontend/docs/rules.md) |

## Commands (run inside `backend/` or `frontend/`)

- Backend focused test: `dotnet test tests/UnitTests --filter FullyQualifiedName~<Name>`; architecture rules: `dotnet test tests/ArchitectureTests`; build regenerates the OpenAPI document: `dotnet build ModularTemplate.slnx`.
- Frontend one project: `npx nx test <project>`, `npx nx lint <project>`, `npx nx typecheck <project>`; contract client: `npm run contracts:generate`, `npm run contracts:check`; folders: `npm run architecture:structure`.
- Scaffold with generators (`dotnet new`, `npx nx g ...`), then trim the output to the rules.
- Do not run full builds, every test project, or E2E unless asked; CI does that.

## Rules that are easy to miss

- **Modules:** `Api`, `Application`, `Contracts`, `Domain`, `Infrastructure`; another module only through its `Contracts`; one folder per use case; business rules return `Result` with a typed `Error`.
- **Folder axiom:** a folder holds at most 6 code files; split by meaning into named subfolders; no empty, wrapper, or one-file folders; generated code exempt. Architecture tests and `architecture:structure` enforce it.
- **Contract:** OpenAPI comes from the code; never edit it or the generated client by hand.
- **Idempotency:** creates that must not repeat take an `Idempotency-Key`.
- **New feature:** plan → specification → tests first → implementation → audit → docs. A small fix needs only the fix and its focused test.

## Agent hygiene

- Do not commit working files (`usage.json`, `tmp*.json`, `output/`, screenshots); keep them outside the repository.
- `.claude/settings.json` hooks block secrets and dangerous commands, format edited files, and compile changed code before finishing. They do not run in every environment (for example some cloud sessions), so GitHub branch protection and CI stay the real guard.
- Git texts (commits, pull requests) in English. Commit, push, or open a PR only on the user's request.
