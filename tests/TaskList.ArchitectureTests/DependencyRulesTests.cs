using System.Reflection;
using NetArchTest.Rules;
using Shouldly;
using TaskList.Application.Abstractions;
using TaskList.Domain.Tasks;

namespace TaskList.ArchitectureTests;

public sealed class DependencyRulesTests
{
    private static readonly Assembly _domainAssembly = typeof(TaskItem).Assembly;
    private static readonly Assembly _applicationAssembly = typeof(ICommandHandler<,>).Assembly;

    [Fact]
    public void Domain_ShouldNot_DependOn_Application()
    {
        // Act
        var result = Types.InAssembly(_domainAssembly)
            .ShouldNot()
            .HaveDependencyOn("TaskList.Application")
            .GetResult();

        // Assert
        AssertSuccess(result);
    }

    [Fact]
    public void Domain_ShouldNot_DependOn_Infrastructure()
    {
        // Act
        var result = Types.InAssembly(_domainAssembly)
            .ShouldNot()
            .HaveDependencyOn("TaskList.Infrastructure")
            .GetResult();

        // Assert
        AssertSuccess(result);
    }

    [Fact]
    public void Domain_ShouldNot_DependOn_Api()
    {
        // Act
        var result = Types.InAssembly(_domainAssembly)
            .ShouldNot()
            .HaveDependencyOn("TaskList.Api")
            .GetResult();

        // Assert
        AssertSuccess(result);
    }

    [Fact]
    public void Application_ShouldNot_DependOn_Infrastructure()
    {
        // Act
        var result = Types.InAssembly(_applicationAssembly)
            .ShouldNot()
            .HaveDependencyOn("TaskList.Infrastructure")
            .GetResult();

        // Assert
        AssertSuccess(result);
    }

    [Fact]
    public void Application_ShouldNot_DependOn_Api()
    {
        // Act
        var result = Types.InAssembly(_applicationAssembly)
            .ShouldNot()
            .HaveDependencyOn("TaskList.Api")
            .GetResult();

        // Assert
        AssertSuccess(result);
    }

    private static void AssertSuccess(NetArchTest.Rules.TestResult result)
    {
        var failing = result.FailingTypeNames is null ? string.Empty : string.Join(", ", result.FailingTypeNames);
        result.IsSuccessful.ShouldBeTrue(failing);
    }
}
