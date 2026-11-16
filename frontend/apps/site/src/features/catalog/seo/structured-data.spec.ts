import { describe, expect, it } from 'vitest';
import { breadcrumbJsonLd, productJsonLd } from './structured-data';

const product = {
  id: '0f8fad5b-d9cb-469f-a165-70867728950e',
  name: 'Fountain pen',
  slug: 'fountain-pen',
  description: null,
  price: 690,
  categoryName: 'Pens',
  categorySlug: 'pens',
  updatedAtUtc: '2026-01-10T09:00:00Z',
};

describe('productJsonLd', () => {
  it('describes the product with an offer in the site currency', () => {
    const data = productJsonLd(product);

    expect(data['@type']).toBe('Product');
    expect(data.offers).toMatchObject({
      price: '690.00',
      priceCurrency: 'USD',
    });
    expect(data.url).toMatch(/\/products\/fountain-pen$/);
    expect(data.description).toBeUndefined();
  });
});

describe('breadcrumbJsonLd', () => {
  it('numbers the trail from one with absolute URLs', () => {
    const data = breadcrumbJsonLd([
      { name: 'Home', path: '/' },
      { name: 'Pens', path: '/catalog/pens' },
    ]);

    expect(data.itemListElement[1]).toMatchObject({
      position: 2,
      name: 'Pens',
    });
    expect(data.itemListElement[1].item).toMatch(
      /^https?:\/\/.+\/catalog\/pens$/,
    );
  });
});
