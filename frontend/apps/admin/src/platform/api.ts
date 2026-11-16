import { createApiClient } from '@template/api-client';

/**
 * The typed API client. In development Vite proxies `/api` to the backend, so the session cookie stays on one
 * origin; a deployed admin app sets VITE_API_URL to the API origin.
 */
export const api = createApiClient({
  baseUrl: import.meta.env.VITE_API_URL ?? '',
});
