import { describe, expect, it } from 'vitest';
import { pageNumber, totalPages } from './page-number';

describe('pageNumber', () => {
  it.each([
    [undefined, 1],
    ['3', 3],
    ['0', 1],
    ['-2', 1],
    ['2.5', 1],
    ['abc', 1],
    [['4', '5'], 4],
  ])('reads %j as page %i', (value, expected) => {
    expect(pageNumber(value)).toBe(expected);
  });

  it('counts at least one page', () => {
    expect(totalPages(0, 12)).toBe(1);
    expect(totalPages(25, 12)).toBe(3);
  });
});
