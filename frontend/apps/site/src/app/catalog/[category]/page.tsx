import type { Metadata } from 'next';
import { notFound } from 'next/navigation';
import {
  getCategories,
  getProducts,
  PAGE_SIZE,
} from '@/features/catalog/data/catalog-api';
import { pageNumber, totalPages } from '@/features/catalog/data/page-number';
import { JsonLd } from '@/features/catalog/seo/JsonLd';
import { breadcrumbJsonLd } from '@/features/catalog/seo/structured-data';
import { Breadcrumbs } from '@/features/catalog/ui/Breadcrumbs';
import { Pagination } from '@/features/catalog/ui/Pagination';
import { ProductGrid } from '@/features/catalog/ui/ProductGrid';

interface Props {
  params: Promise<{ category: string }>;
  searchParams: Promise<{ page?: string }>;
}

async function findCategory(slug: string) {
  return (await getCategories()).find((category) => category.slug === slug);
}

export async function generateMetadata({
  params,
  searchParams,
}: Props): Promise<Metadata> {
  const [{ category: slug }, { page }] = await Promise.all([
    params,
    searchParams,
  ]);
  const category = await findCategory(slug);
  if (!category) return {};

  const current = pageNumber(page);
  const path =
    current === 1 ? `/catalog/${slug}` : `/catalog/${slug}?page=${current}`;
  return {
    title: current === 1 ? category.name : `${category.name}, page ${current}`,
    description: `${category.name} in our store: prices, descriptions and quick order.`,
    // Every page of a category is its own canonical page, so crawlers index the products on all of them.
    alternates: { canonical: path },
  };
}

export default async function CategoryPage({ params, searchParams }: Props) {
  const [{ category: slug }, { page }] = await Promise.all([
    params,
    searchParams,
  ]);
  const category = await findCategory(slug);
  if (!category) notFound();

  const current = pageNumber(page);
  const products = await getProducts({ category: slug, page: current });
  const pages = totalPages(products.totalCount, PAGE_SIZE);
  if (current > pages) notFound();

  const trail = [
    { name: 'Home', path: '/' },
    { name: category.name, path: `/catalog/${slug}` },
  ];

  return (
    <>
      <JsonLd data={breadcrumbJsonLd(trail)} />
      <Breadcrumbs items={trail} />
      <h1>{category.name}</h1>
      <ProductGrid products={products.items} />
      <Pagination
        href={(target) =>
          target === 1 ? `/catalog/${slug}` : `/catalog/${slug}?page=${target}`
        }
        page={current}
        pages={pages}
      />
    </>
  );
}
