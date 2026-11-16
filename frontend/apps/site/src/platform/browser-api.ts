import { createApiClient } from '@template/api-client';

/** The API client for client components, such as the order form. */
export const browserApi = createApiClient({
  baseUrl: process.env.NEXT_PUBLIC_API_URL ?? 'http://localhost:5000',
});
