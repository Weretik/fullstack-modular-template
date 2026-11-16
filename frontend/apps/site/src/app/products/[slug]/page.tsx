import type { Metadata } from 'next';
import { notFound } from 'next/navigation';
import { findProduct } from '@/features/catalog/data/catalog-api';
import { JsonLd } from '@/features/catalog/seo/JsonLd';
import {
  breadcrumbJsonLd,
  productJsonLd,
} from '@/features/catalog/seo/structured-data';
import { Breadcrumbs } from '@/features/catalog/ui/Breadcrumbs';
import { QuickOrderForm } from '@/features/orders/QuickOrderForm';
import { formatPrice } from '@/platform/format-price';

interface Props {
  params: Promise<{ slug: string }>;
}

export async function generateMetadata({ params }: Props): Promise<Metadata> {
  const product = await findProduct((await params).slug);
  if (!product) return {};

  const description =
    product.description ?? `${product.name} for ${formatPrice(product.price)}.`;
  return {
    title: product.name,
    description,
    alternates: { canonical: `/products/${product.slug}` },
    openGraph: {
      title: product.name,
      description,
      url: `/products/${product.slug}`,
    },
  };
}

export default async function ProductPage({ params }: Props) {
  const product = await findProduct((await params).slug);
  if (!product) notFound();

  const trail = [
    { name: 'Home', path: '/' },
    { name: product.categoryName, path: `/catalog/${product.categorySlug}` },
    { name: product.name, path: `/products/${product.slug}` },
  ];

  return (
    <>
      <JsonLd data={productJsonLd(product)} />
      <JsonLd data={breadcrumbJsonLd(trail)} />
      <Breadcrumbs items={trail} />
      <article className="product">
        <div>
          <h1>{product.name}</h1>
          <p className="product-price">{formatPrice(product.price)}</p>
          {product.description && <p>{product.description}</p>}
        </div>
        <QuickOrderForm productId={product.id} />
      </article>
    </>
  );
}
