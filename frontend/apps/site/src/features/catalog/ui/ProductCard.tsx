import Link from 'next/link';
import type { Schema } from '@template/api-client';
import { formatPrice } from '@/platform/format-price';

export function ProductCard({ product }: { product: Schema<'ProductCard'> }) {
  return (
    <li className="card">
      <Link href={`/products/${product.slug}`} className="card-link">
        <span className="card-category">{product.categoryName}</span>
        <span className="card-name">{product.name}</span>
        <span className="card-price">{formatPrice(product.price)}</span>
      </Link>
    </li>
  );
}
