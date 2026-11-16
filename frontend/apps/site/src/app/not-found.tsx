import Link from 'next/link';

export default function NotFound() {
  return (
    <section className="hero">
      <h1>Page not found</h1>
      <p>
        The page may have moved. Go to the <Link href="/">home page</Link> or
        try the <Link href="/search">search</Link>.
      </p>
    </section>
  );
}
