using NetArchTest.Rules;

namespace ArchitectureTests.Boundaries;

/// <summary>Dependencies point inwards: Api → Infrastructure → Application → Domain.</summary>
public sealed class LayerDependencyTests
{
    public static TheoryData<string> Modules => new(Repository.Modules);

    [Theory]
    [MemberData(nameof(Modules))]
    public void Domain_DependsOnNoFrameworks(string module)
    {
        var result = Types.InAssembly(Repository.Assembly($"{module}.Domain"))
            .ShouldNot()
            .HaveDependencyOnAny("Microsoft.EntityFrameworkCore", "Microsoft.AspNetCore", "Mediator", "FluentValidation", "Npgsql",
                $"{module}.Application", $"{module}.Infrastructure", $"{module}.Api")
            .GetResult();

        Assert.True(result.IsSuccessful, string.Join(", ", result.FailingTypeNames ?? []));
    }

    [Theory]
    [MemberData(nameof(Modules))]
    public void Application_DoesNotKnowTheDatabaseDriverOrTheWeb(string module)
    {
        var result = Types.InAssembly(Repository.Assembly($"{module}.Application"))
            .ShouldNot()
            .HaveDependencyOnAny("Npgsql", "Microsoft.AspNetCore", $"{module}.Infrastructure", $"{module}.Api")
            .GetResult();

        Assert.True(result.IsSuccessful, string.Join(", ", result.FailingTypeNames ?? []));
    }

    [Theory]
    [MemberData(nameof(Modules))]
    public void Infrastructure_DoesNotDependOnApi(string module)
    {
        var result = Types.InAssembly(Repository.Assembly($"{module}.Infrastructure"))
            .ShouldNot()
            .HaveDependencyOn($"{module}.Api")
            .GetResult();

        Assert.True(result.IsSuccessful, string.Join(", ", result.FailingTypeNames ?? []));
    }

    [Fact]
    public void The_Host_OnlyComposes_ItReferencesModuleApisAndSharedInfrastructure()
    {
        var references = Repository.ProjectReferences(Path.Combine(Repository.Root, "src/Host/Host.Api/Host.Api.csproj"));

        Assert.All(references, reference => Assert.True(
            reference.EndsWith(".Api", StringComparison.Ordinal) || reference == "BuildingBlocks.Infrastructure", reference));
    }
}
