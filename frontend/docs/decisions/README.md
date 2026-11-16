# Decisions

Architecture decision records: why the frontend is built the way it is.

| # | Decision |
| --- | --- |
| 0001 | [One Nx workspace for all frontend apps](0001-nx-workspace.md) |
| 0002 | [Next.js App Router for the public storefront](0002-nextjs-storefront.md) |
| 0003 | [A React single-page app for the admin, never indexed](0003-react-spa-admin.md) |
| 0004 | [A typed API client generated from the backend contract](0004-typed-client-from-openapi.md) |

## Decisions that must have a record

- A new app, or a new framework for an app.
- Hosting or rendering strategy of an app.
- A state management or data transport approach.
- A rule that every change must follow.

New record: copy the newest file, take the next number, and keep the sections Context, Decision, Consequences. A record
that is replaced stays, with its status changed to `superseded by NNNN`.
