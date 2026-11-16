# 0002. Next.js App Router for the public storefront

Status: accepted, 2026-11-16

## Context

The storefront must be found by search engines: real HTML on first load, per-page titles and canonical URLs,
structured data, a sitemap, and fast pages under load.

## Decision

Build the storefront with the Next.js App Router. Pages are rendered on the server; catalog reads are cached and
refreshed in the background every five minutes. Each page sets metadata and canonical URLs, product pages add
JSON-LD, internal search answers `noindex, follow`, and only `SITE_ENV=production` may be indexed.

## Consequences

- Search engines and social previews get complete pages.
- The API sees a few requests per page per cache period, not one per visitor.
- Interactive pieces (the order form) are small client components; everything else stays on the server.
