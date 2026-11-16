import type { MetadataRoute } from 'next';
import { getSitemap } from '@/features/catalog/data/catalog-api';
import { siteConfig } from '@/platform/site-config';

export const revalidate = 3600;

export default async function sitemap(): Promise<MetadataRoute.Sitemap> {
  const { categories, products } = await getSitemap();

  return [
    { url: siteConfig.url },
    ...categories.map((category) => ({
      url: `${siteConfig.url}/catalog/${category.slug}`,
    })),
    ...products.map((product) => ({
      url: `${siteConfig.url}/products/${product.slug}`,
      lastModified: product.updatedAtUtc ?? undefined,
    })),
  ];
}
