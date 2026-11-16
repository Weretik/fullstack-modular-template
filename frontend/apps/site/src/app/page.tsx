import { getProducts } from '@/features/catalog/data/catalog-api';
import { ProductGrid } from '@/features/catalog/ui/ProductGrid';
import { siteConfig } from '@/platform/site-config';

export default async function HomePage() {
  const products = await getProducts({});

  return (
    <>
      <section className="hero">
        <h1>{siteConfig.name}</h1>
        <p>{siteConfig.description}</p>
      </section>
      <h2>New in the store</h2>
      <ProductGrid products={products.items} />
    </>
  );
}
