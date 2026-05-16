using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using TaskList.IntegrationTests.Fixtures;

namespace TaskList.IntegrationTests.Endpoints;

public sealed class TasksEndpointsTests : IClassFixture<TaskApiFactory>, IAsyncLifetime
{
    private readonly TaskApiFactory _factory;
    private readonly HttpClient _client;

    public TasksEndpointsTests(TaskApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    public ValueTask InitializeAsync() =>
        new(_factory.WithDbAsync(db => db.Tasks.ExecuteDeleteAsync(TestContext.Current.CancellationToken)));

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    [Fact]
    public async Task When_PostingValidTask_Should_Return201WithLocationHeader()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/v1/tasks",
            new { title = "Read RFC 7807" },
            TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        response.Headers.Location.ShouldNotBeNull();
        response.Headers.Location!.ToString().ShouldContain("/api/v1/tasks/");
    }

    [Fact]
    public async Task When_PostingDuplicateTitle_Should_Return409WithProblemDetails()
    {
        var first = await _client.PostAsJsonAsync(
            "/api/v1/tasks",
            new { title = "Read RFC 7807" },
            TestContext.Current.CancellationToken);
        first.StatusCode.ShouldBe(HttpStatusCode.Created);

        var response = await _client.PostAsJsonAsync(
            "/api/v1/tasks",
            new { title = "  read RFC 7807  " },
            TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>(TestContext.Current.CancellationToken);
        body.GetProperty("status").GetInt32().ShouldBe(409);
        body.GetProperty("detail").GetString().ShouldNotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task When_PostingEmptyTitle_Should_Return422WithProblemDetails()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/v1/tasks",
            new { title = string.Empty },
            TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>(TestContext.Current.CancellationToken);
        body.GetProperty("status").GetInt32().ShouldBe(422);
        body.GetProperty("errors").GetProperty("Title").GetArrayLength().ShouldBeGreaterThan(0);
    }

    [Fact]
    public async Task When_GettingTasks_Should_Return200WithCollection()
    {
        await _factory.WithDbAsync(async db =>
        {
            db.Tasks.AddRange(
                TaskFaker.ATask(TimeProvider.System),
                TaskFaker.ATask(TimeProvider.System));
            await db.SaveChangesAsync(TestContext.Current.CancellationToken);
        });

        var response = await _client.GetAsync(new Uri("/api/v1/tasks", UriKind.Relative), TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>(TestContext.Current.CancellationToken);
        body.GetProperty("count").GetInt32().ShouldBe(2);
        body.GetProperty("data").GetArrayLength().ShouldBe(2);
    }

    [Fact]
    public async Task When_TogglingExistingTask_Should_Return200WithFlippedState()
    {
        var taskId = await _factory.WithDbAsync(async db =>
        {
            var task = TaskFaker.ATask(TimeProvider.System);
            db.Tasks.Add(task);
            await db.SaveChangesAsync(TestContext.Current.CancellationToken);
            return task.Id;
        });

        var response = await _client.PostAsync(
            new Uri($"/api/v1/tasks/{taskId.Value}/toggle", UriKind.Relative),
            content: null,
            TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>(TestContext.Current.CancellationToken);
        body.GetProperty("isCompleted").GetBoolean().ShouldBeTrue();
    }

    [Fact]
    public async Task When_DeletingExistingTask_Should_Return204()
    {
        var taskId = await _factory.WithDbAsync(async db =>
        {
            var task = TaskFaker.ATask(TimeProvider.System);
            db.Tasks.Add(task);
            await db.SaveChangesAsync(TestContext.Current.CancellationToken);
            return task.Id;
        });

        var response = await _client.DeleteAsync(
            new Uri($"/api/v1/tasks/{taskId.Value}", UriKind.Relative),
            TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }

    public static TheoryData<string, string, string> MissingTaskRoutes => new()
    {
        { "GET",    "",         "GET by id returns 404 when missing" },
        { "POST",   "/toggle",  "POST toggle returns 404 when missing" },
        { "DELETE", "",         "DELETE returns 404 when missing" },
    };

    [Theory]
    [MemberData(nameof(MissingTaskRoutes))]
    public async Task When_OperatingOnMissingTask_Should_Return404WithProblemDetails(
        string method, string urlSuffix, string scenario)
    {
        _ = scenario;

        var url = new Uri($"/api/v1/tasks/{Guid.NewGuid()}{urlSuffix}", UriKind.Relative);
        using var request = new HttpRequestMessage(new HttpMethod(method), url);

        var response = await _client.SendAsync(request, TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>(TestContext.Current.CancellationToken);
        body.GetProperty("status").GetInt32().ShouldBe(404);
    }
}
