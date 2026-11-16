using System.Reflection;
using System.Xml.Linq;

namespace ArchitectureTests;

/// <summary>Finds the backend folder and reads the solution's projects and assemblies.</summary>
internal static class Repository
{
    public static readonly string Root = FindRoot();

    public static readonly string[] Modules = ["Identity", "Catalog", "Orders"];

    public static readonly string[] Layers = ["Domain", "Application", "Infrastructure", "Api", "Contracts"];

    public static IEnumerable<string> ProjectFiles(string folder) =>
        Directory.EnumerateFiles(Path.Combine(Root, folder), "*.csproj", SearchOption.AllDirectories);

    /// <summary>The names of the projects a project file references directly.</summary>
    public static IReadOnlyList<string> ProjectReferences(string projectFile) =>
        XDocument.Load(projectFile)
            .Descendants("ProjectReference")
            .Select(r => Path.GetFileNameWithoutExtension(r.Attribute("Include")!.Value.Replace('\\', '/')))
            .ToList();

    public static Assembly Assembly(string name) => System.Reflection.Assembly.Load(name);

    private static string FindRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "ModularTemplate.slnx")))
            directory = directory.Parent;
        return directory?.FullName ?? throw new InvalidOperationException("ModularTemplate.slnx not found.");
    }
}
