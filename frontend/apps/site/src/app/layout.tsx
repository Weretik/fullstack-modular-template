import type { Metadata } from 'next';
import Link from 'next/link';
import type { ReactNode } from 'react';
import { CategoryNav } from '@/features/catalog/ui/CategoryNav';
import { siteConfig } from '@/platform/site-config';
import './global.css';

export const metadata: Metadata = {
  metadataBase: new URL(siteConfig.url),
  title: { default: siteConfig.name, template: `%s | ${siteConfig.name}` },
  description: siteConfig.description,
  openGraph: { siteName: siteConfig.name, type: 'website' },
  robots: siteConfig.indexable ? undefined : { index: false, follow: false },
};

export default function RootLayout({ children }: { children: ReactNode }) {
  return (
    <html lang="en">
      <body>
        <header className="site-header">
          <Link href="/" className="logo">
            {siteConfig.name}
          </Link>
          <CategoryNav />
        </header>
        <main className="container">{children}</main>
        <footer className="site-footer">© {siteConfig.name}</footer>
      </body>
    </html>
  );
}
