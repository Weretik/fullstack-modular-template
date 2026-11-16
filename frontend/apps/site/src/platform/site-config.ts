/** Site-wide settings, read once from the environment. */
export const siteConfig = {
  name: process.env.NEXT_PUBLIC_SITE_NAME ?? 'Paper & Ink',
  description: 'Notebooks, pens and desk accessories, delivered to your door.',
  /** The public origin, used for canonical URLs, the sitemap and structured data. */
  url: (process.env.SITE_URL ?? 'http://localhost:3000').replace(/\/$/, ''),
  currency: process.env.NEXT_PUBLIC_CURRENCY ?? 'USD',
  /** Only production may be indexed; previews and staging answer noindex. */
  indexable: process.env.SITE_ENV === 'production',
} as const;
