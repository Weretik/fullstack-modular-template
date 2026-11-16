import { createApiClient } from '@template/api-client';

/** How long a catalog page may be served from cache before Next.js fetches it again. */
export const CATALOG_REVALIDATE_SECONDS = 300;

/**
 * The API client for server components. Catalog reads are cached and refreshed in the background (ISR), so
 * pages stay fast and the API sees a few requests per page per five minutes, not one per visitor.
 */
export const serverApi = createApiClient({
  baseUrl: process.env.API_URL ?? 'http://localhost:5000',
  fetch: (input) =>
    fetch(input, { next: { revalidate: CATALOG_REVALIDATE_SECONDS } }),
});
