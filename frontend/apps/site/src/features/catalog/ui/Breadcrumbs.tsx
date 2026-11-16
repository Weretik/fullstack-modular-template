import Link from 'next/link';

export function Breadcrumbs({
  items,
}: {
  items: { name: string; path: string }[];
}) {
  return (
    <nav aria-label="Breadcrumbs" className="breadcrumbs">
      {items.map((item, index) =>
        index === items.length - 1 ? (
          <span key={item.path} aria-current="page">
            {item.name}
          </span>
        ) : (
          <span key={item.path}>
            <Link href={item.path}>{item.name}</Link> /{' '}
          </span>
        ),
      )}
    </nav>
  );
}
