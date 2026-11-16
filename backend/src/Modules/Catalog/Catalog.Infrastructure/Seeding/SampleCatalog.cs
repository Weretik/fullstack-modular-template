namespace Catalog.Infrastructure.Seeding;

/// <summary>A small stationery store, so the site and the admin app have something to show on first run.</summary>
internal static class SampleCatalog
{
    public static readonly IReadOnlyList<((string Name, string Slug) Category, (string Name, string Slug, decimal Price)[] Products)> Items =
    [
        (("Notebooks", "notebooks"),
        [
            ("Dotted notebook A5", "dotted-notebook-a5", 189m),
            ("Lined notebook A4", "lined-notebook-a4", 215m),
            ("Pocket sketchbook", "pocket-sketchbook", 149m),
        ]),
        (("Pens", "pens"),
        [
            ("Blue gel pen", "blue-gel-pen", 39m),
            ("Fountain pen", "fountain-pen", 690m),
            ("Fineliner set", "fineliner-set", 249m),
        ]),
        (("Desk", "desk"),
        [
            ("Desk organizer", "desk-organizer", 420m),
            ("Paper clips", "paper-clips", 29m),
        ]),
    ];
}
