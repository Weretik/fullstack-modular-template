import Link from 'next/link';

/** Plain links, so crawlers can reach every page of a category. */
export function Pagination({
  href,
  page,
  pages,
}: {
  href: (page: number) => string;
  page: number;
  pages: number;
}) {
  if (pages <= 1) return null;

  return (
    <nav className="pagination" aria-label="Pages">
      {page > 1 && (
        <Link href={href(page - 1)} rel="prev">
          ← Previous
        </Link>
      )}
      <span>
        Page {page} of {pages}
      </span>
      {page < pages && (
        <Link href={href(page + 1)} rel="next">
          Next →
        </Link>
      )}
    </nav>
  );
}
