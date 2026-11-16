import { siteConfig } from './site-config';

const formatter = new Intl.NumberFormat('en-US', {
  style: 'currency',
  currency: siteConfig.currency,
});

export function formatPrice(amount: number): string {
  return formatter.format(amount);
}
