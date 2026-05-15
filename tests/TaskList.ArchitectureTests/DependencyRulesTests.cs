using System.Reflection;
using NetArchTest.Rules;
using Shouldly;
using TaskList.Application.Abstractions;
using TaskList.Domain.Tasks;

namespace TaskList.ArchitectureTests;

public sealed class DependencyRulesTests
{
    private static readonly Assembly DomainAssembly = typeof(TaskItem).Assembly;
    private static readonly Assembly ApplicationAssembly = typeof(ICommandHandler<,>).Assembly;

    [Fact]
    public void Domain_ShouldNot_DependOn_Application()
    {
        var result = Types.InAssembly(DomainAssembly)
            .ShouldNot()
            .HaveDependencyOn("TaskList.Application")
            .GetResult();

        AssertSuccess(result);
    }

    [Fact]
    public void Domain_ShouldNot_DependOn_Infrastructure()
    {
        var result = Types.InAssembly(DomainAssembly)
            .ShouldNot()
            .HaveDependencyOn("TaskList.Infrastructure")
            .GetResult();

        AssertSuccess(result);
    }

    [Fact]
    public void Domain_ShouldNot_DependOn_Api()
    {
        var result = Types.InAssembly(DomainAssembly)
            .ShouldNot()
            .HaveDependencyOn("TaskList.Api")
            .GetResult();

        AssertSuccess(result);
    }

    [Fact]
    public void Application_ShouldNot_DependOn_Infrastructure()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .ShouldNot()
            .HaveDependencyOn("TaskList.Infrastructure")
            .GetResult();

        AssertSuccess(result);
    }

    [Fact]
    public void Application_ShouldNot_DependOn_Api()
    {
        var result = Types.InAssembly(ApplicationAssembly)
            .ShouldNot()
            .HaveDependencyOn("TaskList.Api")
            .GetResult();

        AssertSuccess(result);
    }

    private static void AssertSuccess(NetArchTest.Rules.TestResult result)
    {
        var failing = result.FailingTypeNames is null
            ? string.Empty
            : string.Join(", ", result.FailingTypeNames);
        result.IsSuccessful.ShouldBeTrue(failing);
    }
}
