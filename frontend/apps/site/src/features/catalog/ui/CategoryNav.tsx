import Link from 'next/link';
import { getCategories } from '../data/catalog-api';

export async function CategoryNav() {
  const categories = await getCategories();

  return (
    <nav aria-label="Categories" className="category-nav">
      {categories.map((category) => (
        <Link key={category.slug} href={`/catalog/${category.slug}`}>
          {category.name}
        </Link>
      ))}
      <Link href="/search">Search</Link>
    </nav>
  );
}
