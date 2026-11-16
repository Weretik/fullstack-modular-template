# Frontend rules

## Structure

- Code is grouped by feature (`features/products/`), then by role inside a feature when it grows (`data/`, `seo/`,
  `ui/`). Framework wiring lives in `app/`, environment and clients in `platform/`.
- **A folder holds at most six production files** (tests next to them and `index.ts` do not count; generated code is
  exempt). The seventh file splits the folder by meaning, never one folder per file.
- A folder holds either files or subfolders, not both, and never a single file alone.
- No dump folders or files: `utils`, `helpers`, `common`, `shared`, `types`, `constants`, `lib`. Name things by what
  they do (`format-price.ts`, `slug-from-name.ts`).
- One component or one responsibility per file; component files are PascalCase, the rest kebab-case.
- `npm run architecture:structure` checks the folder rules in CI. The Next.js `app/` routing folder is exempt
  because the framework defines its shape.

## API

- Call the API only through `@template/api-client`; no raw `fetch` or Axios for API calls.
- Never edit `libs/api-client/src/generated/`; regenerate it from the backend contract.
- Show the problem response's message to the user and map field errors onto form fields.

## Writes

- Every mutation disables its submit while pending.
- A write the backend protects with `Idempotency-Key` makes one key per attempt, reuses it on retry, and renews it
  after success or a change of the data.

## Storefront

- Server components by default; a client component only for interactivity.
- Every page sets title, description and canonical URL; indexable pages are in the sitemap.
- Real status codes: unknown content calls `notFound()`.
- Nothing is indexable unless `SITE_ENV=production`.

## Admin

- Server state in React Query, not in component state copies.
- The app is never indexed.
- Mark only problems in lists, with the reason; no "OK" badges.

## Tests and checks

- Test logic first: pure functions with Vitest, forms and pages with Testing Library.
- During work, run the tests of the project you changed (`npx nx test <project>`); CI runs everything affected.
- Use Nx and other CLI generators to scaffold, then trim their output to these rules.
- A new app, framework, rendering or hosting approach, or state/transport approach gets an ADR in the same change.
