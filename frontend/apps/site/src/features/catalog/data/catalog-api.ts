import { apiCall, ApiError, type Schema } from '@template/api-client';
import { connection } from 'next/server';
import { serverApi } from '@/platform/server-api';

export const PAGE_SIZE = 12;

/*
 * Every read first opts the page into request-time rendering, so the site builds without a running API. The
 * fetches themselves stay cached for CATALOG_REVALIDATE_SECONDS, so the API still sees few requests.
 */

export async function getCategories(): Promise<Schema<'CategoryResponse'>[]> {
  await connection();
  return apiCall(serverApi.GET('/api/catalog/categories'));
}

export async function getProducts(query: {
  category?: string;
  search?: string;
  page?: number;
}): Promise<Schema<'PagedListOfProductCard'>> {
  await connection();
  return apiCall(
    serverApi.GET('/api/catalog/products', {
      params: {
        query: {
          category: query.category,
          search: query.search,
          page: query.page ?? 1,
          pageSize: PAGE_SIZE,
        },
      },
    }),
  );
}

/** The product, or null when it does not exist or is not published. */
export async function findProduct(
  slug: string,
): Promise<Schema<'PublishedProductResponse'> | null> {
  await connection();
  try {
    return await apiCall(
      serverApi.GET('/api/catalog/products/{slug}', {
        params: { path: { slug } },
      }),
    );
  } catch (error) {
    if (error instanceof ApiError && error.status === 404) return null;
    throw error;
  }
}

export async function getSitemap(): Promise<Schema<'SitemapResponse'>> {
  await connection();
  return apiCall(serverApi.GET('/api/catalog/sitemap'));
}
