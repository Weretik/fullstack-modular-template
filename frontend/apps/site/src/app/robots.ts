import type { MetadataRoute } from 'next';
import { siteConfig } from '@/platform/site-config';

export default function robots(): MetadataRoute.Robots {
  if (!siteConfig.indexable)
    return { rules: { userAgent: '*', disallow: '/' } };

  return {
    rules: { userAgent: '*', allow: '/', disallow: ['/search'] },
    sitemap: `${siteConfig.url}/sitemap.xml`,
  };
}
