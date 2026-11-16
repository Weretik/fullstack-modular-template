# Frontend

An Nx workspace with two apps and one shared library:

| Project | Path | What it is |
| --- | --- | --- |
| `site` | `apps/site` | Public storefront: Next.js App Router, server-rendered and cached, built for search engines |
| `admin` | `apps/admin` | Staff app: React, Vite, Material UI, React Query; never indexed |
| `api-client` | `libs/api-client` | Typed API client generated from the backend contract (`@template/api-client`) |

- [Architecture](docs/architecture.md): how the apps are organized and talk to the API.
- [Rules](docs/rules.md): conventions every change follows.
- [Decisions](docs/decisions/README.md): why it is built this way.

## Commands

Run from this folder after `npm ci` (Node.js 22, npm 11).

| What | Command |
| --- | --- |
| Storefront dev server on http://localhost:3000 | `npx nx dev site` |
| Admin dev server on http://localhost:4200 | `npx nx serve admin` |
| Tests of one project | `npx nx test admin` |
| Lint or typecheck one project | `npx nx lint site`, `npx nx typecheck site` |
| Everything a change affects | `npx nx affected -t lint typecheck test build` |
| Production build of the storefront | `SITE_ENV=production SITE_URL=https://shop.example.com npx nx build site` |
| Regenerate the API client | `npm run contracts:generate` |
| Check the client matches the contract | `npm run contracts:check` |
| Folder rules | `npm run architecture:structure` |
| Code style (Prettier) | `npm run format:check` |

Both apps expect the API on http://localhost:5000 in development (see the backend README).

## Environment variables

| App | Variable | Meaning |
| --- | --- | --- |
| site | `API_URL` | API origin used by server components (default `http://localhost:5000`). |
| site | `NEXT_PUBLIC_API_URL` | API origin used in the browser by the order form. |
| site | `SITE_URL` | Public origin for canonical URLs, the sitemap and structured data. |
| site | `SITE_ENV` | Only `production` makes the site indexable; anything else answers `noindex` and disallows crawling. |
| site | `NEXT_PUBLIC_SITE_NAME`, `NEXT_PUBLIC_CURRENCY` | Shop name and the ISO currency code used to format prices. |
| admin | `VITE_API_URL` | API origin for a deployed admin; empty in development, where Vite proxies `/api`. |
| admin (dev server) | `API_URL` | Where the dev proxy sends `/api` (default `http://localhost:5000`). |

`NEXT_PUBLIC_*` and `VITE_*` values are baked into the build, so set them before building.

## When the backend contract changes

1. Build the backend; it rewrites `backend/docs/contracts/openapi.json`.
2. Run `npm run contracts:generate`. It rewrites `libs/api-client/src/generated/api.ts`.
3. Fix what the compiler reports, and commit the generated file with your change. CI fails when it is out of date.

## Add a feature

A feature is one folder under the app's `src/features/`, split by role once it grows:

```
apps/admin/src/features/customers/
  customers-api.ts       React Query hooks over the typed client
  CustomersPage.tsx      list page
  CustomerEditor.tsx     right-side editor drawer
  customer-status.ts     a small pure function, with customer-status.spec.ts next to it
```

```
apps/site/src/features/catalog/
  data/   server-side reads and pure helpers
  seo/    structured data
  ui/     presentational components
```

1. Write the test for the logic first (pure functions, a form's behavior with Testing Library).
2. Call the API only through the client from `src/platform/`: `apiCall(api.GET('/api/...', { params }))`.
3. Add the route in `apps/admin/src/app/App.tsx` (admin) or a folder under `apps/site/src/app/` (site).
4. A storefront page sets its metadata (title, description, canonical) and, for indexable content, appears in the
   sitemap.
5. Keep at most six production files per folder; the seventh splits the folder by role.

To add a new app, use the Nx generators (`npx nx g @nx/next:application`, `npx nx g @nx/react:application`) and
then trim the output to the workspace conventions.

## Safe repeats

Every form that writes has a pending state that disables a second submit. A request the backend protects with an
`Idempotency-Key` (placing an order) creates one key per attempt, reuses it when retrying after an error, and makes a
new one after success or when the data changes. See `apps/site/src/features/orders/QuickOrderForm.tsx`.
