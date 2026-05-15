using Shouldly;
using TaskList.Api.Features.Tasks.Mapping;

namespace TaskList.ArchitectureTests;

public sealed class HateoasResponseShapeTests
{
    [Fact]
    public void ResponseTypes_Should_ExposeLinksProperty()
    {
        var responseTypes = typeof(LinkResponse).Assembly.GetTypes()
            .Where(t => t.IsPublic && !t.IsAbstract)
            .Where(t => t.Name.EndsWith("Response", StringComparison.Ordinal))
            .Where(t => t != typeof(LinkResponse))
            .Where(t => t.Namespace?.Contains(".Features.", StringComparison.Ordinal) == true)
            .ToList();

        responseTypes.ShouldNotBeEmpty();
        responseTypes.ShouldAllBe(t => HasLinksOfExpectedType(t));
    }

    private static bool HasLinksOfExpectedType(Type type)
    {
        var property = type.GetProperty("Links");
        return property is not null
            && property.PropertyType == typeof(IReadOnlyList<LinkResponse>);
    }
}
