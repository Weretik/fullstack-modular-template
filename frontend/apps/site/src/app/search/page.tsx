import type { Metadata } from 'next';
import { getProducts, PAGE_SIZE } from '@/features/catalog/data/catalog-api';
import { pageNumber, totalPages } from '@/features/catalog/data/page-number';
import { Pagination } from '@/features/catalog/ui/Pagination';
import { ProductGrid } from '@/features/catalog/ui/ProductGrid';

// Search results are endless combinations of words: crawlers may follow the links but not index the pages.
export const metadata: Metadata = {
  title: 'Search',
  robots: { index: false, follow: true },
};

interface Props {
  searchParams: Promise<{ q?: string; page?: string }>;
}

export default async function SearchPage({ searchParams }: Props) {
  const { q, page } = await searchParams;
  const query = q?.trim() ?? '';
  const current = pageNumber(page);
  const products = query
    ? await getProducts({ search: query, page: current })
    : null;

  return (
    <>
      <h1>Search</h1>
      <form className="search" action="/search">
        <input
          name="q"
          defaultValue={query}
          placeholder="Notebook, pen…"
          aria-label="Search products"
          maxLength={100}
        />
        <button type="submit">Search</button>
      </form>
      {products && (
        <>
          <ProductGrid products={products.items} />
          <Pagination
            href={(target) =>
              `/search?q=${encodeURIComponent(query)}${target > 1 ? `&page=${target}` : ''}`
            }
            page={current}
            pages={totalPages(products.totalCount, PAGE_SIZE)}
          />
        </>
      )}
    </>
  );
}
