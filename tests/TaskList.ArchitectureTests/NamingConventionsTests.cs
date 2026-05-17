using System.Reflection;
using FluentValidation;
using Shouldly;
using TaskList.Api.Common.Endpoints;
using TaskList.Application.Abstractions;

namespace TaskList.ArchitectureTests;

public sealed class NamingConventionsTests
{
    private static readonly Assembly _apiAssembly = typeof(IEndpointGroup).Assembly;

    [Fact]
    public void Handlers_Should_EndWith_HandlerSuffix()
    {
        // Act
        var handlers = _apiAssembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.GetInterfaces().Any(IsHandlerInterface))
            .ToList();

        // Assert
        handlers.ShouldNotBeEmpty();
        handlers.ShouldAllBe(t => t.Name.EndsWith("Handler", StringComparison.Ordinal));
    }

    [Fact]
    public void Endpoints_Should_ImplementIEndpointGroup()
    {
        // Act
        var endpoints = _apiAssembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith("Endpoint", StringComparison.Ordinal))
            .ToList();

        // Assert
        endpoints.ShouldNotBeEmpty();
        endpoints.ShouldAllBe(t => typeof(IEndpointGroup).IsAssignableFrom(t));
    }

    [Fact]
    public void Validators_Should_EndWith_ValidatorSuffix()
    {
        // Act
        var validators = _apiAssembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && InheritsAbstractValidator(t))
            .ToList();

        // Assert
        validators.ShouldNotBeEmpty();
        validators.ShouldAllBe(t => t.Name.EndsWith("Validator", StringComparison.Ordinal));
    }

    private static bool IsHandlerInterface(Type i)
        => i.IsGenericType && (i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>) || i.GetGenericTypeDefinition() == typeof(IQueryHandler<,>));

    private static bool InheritsAbstractValidator(Type t)
    {
        var current = t.BaseType;

        while (current is not null)
        {
            if (current.IsGenericType && current.GetGenericTypeDefinition() == typeof(AbstractValidator<>))
                return true;

            current = current.BaseType;
        }

        return false;
    }
}
