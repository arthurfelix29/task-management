using Shouldly;
using TaskList.Api.Features.Tasks.Mapping;

namespace TaskList.ArchitectureTests;

public sealed class HateoasResponseShapeTests
{
    [Fact]
    public void ResponseTypes_Should_ExposeLinksProperty()
    {
        // Act
        var responseTypes = typeof(LinkResponse).Assembly.GetTypes()
            .Where(t => t.IsPublic
                && !t.IsAbstract
                && t.Name.EndsWith("Response", StringComparison.Ordinal)
                && t != typeof(LinkResponse)
                && t.Namespace?.Contains(".Features.", StringComparison.Ordinal) == true)
            .ToList();

        // Assert
        responseTypes.ShouldNotBeEmpty();
        responseTypes.ShouldAllBe(t => HasLinksOfExpectedType(t));
    }

    private static bool HasLinksOfExpectedType(Type type)
    {
        var property = type.GetProperty("Links");
        return property is not null && property.PropertyType == typeof(IReadOnlyList<LinkResponse>);
    }
}
