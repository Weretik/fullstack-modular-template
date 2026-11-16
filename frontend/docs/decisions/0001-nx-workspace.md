# 0001. One Nx workspace for all frontend apps

Status: accepted, 2026-11-16

## Context

The storefront and the admin app share the API client and tooling, but have different frameworks and release
needs. Separate repositories would duplicate the client and its generation.

## Decision

Keep both apps and the shared libraries in one Nx workspace with npm workspaces. CI runs lint, typecheck, tests
and builds only for the projects a change affects.

## Consequences

- One place to update the generated client and shared tooling.
- CI time grows with the change, not with the workspace.
- Nx is one more tool to learn.
