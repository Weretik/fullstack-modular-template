import { describe, expect, it } from 'vitest';
import { slugFromName } from './slug-from-name';

describe('slugFromName', () => {
  it.each([
    ['Dotted notebook A5', 'dotted-notebook-a5'],
    ['  Café crème  ', 'cafe-creme'],
    ['Pens & pencils!!', 'pens-pencils'],
    ['---', ''],
  ])('turns %j into %j', (name, slug) => {
    expect(slugFromName(name)).toBe(slug);
  });
});
