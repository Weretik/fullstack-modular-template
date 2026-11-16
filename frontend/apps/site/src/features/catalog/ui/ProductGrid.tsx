import type { Schema } from '@template/api-client';
import { ProductCard } from './ProductCard';

export function ProductGrid({
  products,
}: {
  products: Schema<'ProductCard'>[];
}) {
  if (products.length === 0) return <p className="muted">Nothing found.</p>;

  return (
    <ul className="grid">
      {products.map((product) => (
        <ProductCard key={product.slug} product={product} />
      ))}
    </ul>
  );
}
