import type { Schema } from '@template/api-client';
import { siteConfig } from '@/platform/site-config';

/** schema.org Product with its offer, so search engines can show the price in results. */
export function productJsonLd(product: Schema<'PublishedProductResponse'>) {
  return {
    '@context': 'https://schema.org',
    '@type': 'Product',
    name: product.name,
    description: product.description ?? undefined,
    sku: product.id,
    url: `${siteConfig.url}/products/${product.slug}`,
    category: product.categoryName,
    offers: {
      '@type': 'Offer',
      price: product.price.toFixed(2),
      priceCurrency: siteConfig.currency,
      availability: 'https://schema.org/InStock',
      url: `${siteConfig.url}/products/${product.slug}`,
    },
  };
}

/** schema.org BreadcrumbList for the trail shown on the page. */
export function breadcrumbJsonLd(items: { name: string; path: string }[]) {
  return {
    '@context': 'https://schema.org',
    '@type': 'BreadcrumbList',
    itemListElement: items.map((item, index) => ({
      '@type': 'ListItem',
      position: index + 1,
      name: item.name,
      item: `${siteConfig.url}${item.path}`,
    })),
  };
}
