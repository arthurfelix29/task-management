using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace TaskList.IntegrationTests.Endpoints;

public sealed class TasksEndpointsTests(TaskApiFactory factory) : IClassFixture<TaskApiFactory>, IAsyncLifetime
{
    private readonly HttpClient _client = factory.CreateClient();

    public ValueTask InitializeAsync()
        => new(factory.WithDbAsync(db => db.Tasks.ExecuteDeleteAsync(TestContext.Current.CancellationToken)));

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    [Fact]
    public async Task When_PostingValidTask_Should_Return201WithLocationHeader()
    {
        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/v1/tasks",
            new { title = "Read RFC 7807" },
            TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        response.Headers.Location.ShouldNotBeNull();
        response.Headers.Location!.ToString().ShouldContain("/api/v1/tasks/");
    }

    [Fact]
    public async Task When_PostingDuplicateTitle_Should_Return409WithProblemDetails()
    {
        // Arrange
        var first = await _client.PostAsJsonAsync(
            "/api/v1/tasks",
            new { title = "Read RFC 7807" },
            TestContext.Current.CancellationToken);
        first.StatusCode.ShouldBe(HttpStatusCode.Created);

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/v1/tasks",
            new { title = "  read RFC 7807  " },
            TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>(TestContext.Current.CancellationToken);
        body.GetProperty("status").GetInt32().ShouldBe(409);
        body.GetProperty("detail").GetString().ShouldNotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task When_PostingEmptyTitle_Should_Return422WithProblemDetails()
    {
        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/v1/tasks",
            new { title = string.Empty },
            TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>(TestContext.Current.CancellationToken);
        body.GetProperty("status").GetInt32().ShouldBe(422);
        body.GetProperty("errors").GetProperty("Title").GetArrayLength().ShouldBeGreaterThan(0);
    }

    [Fact]
    public async Task When_GettingTasks_Should_Return200WithCollection()
    {
        // Arrange
        await factory.WithDbAsync(async db =>
        {
            db.Tasks.AddRange(
                TaskFaker.ATask(TimeProvider.System),
                TaskFaker.ATask(TimeProvider.System));
            await db.SaveChangesAsync(TestContext.Current.CancellationToken);
        });

        // Act
        var response = await _client.GetAsync(new Uri("/api/v1/tasks", UriKind.Relative), TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>(TestContext.Current.CancellationToken);
        body.GetProperty("count").GetInt32().ShouldBe(2);
        body.GetProperty("data").GetArrayLength().ShouldBe(2);
    }

    [Fact]
    public async Task When_TogglingExistingTask_Should_Return200WithFlippedState()
    {
        // Arrange
        var taskId = await factory.WithDbAsync(async db =>
        {
            var task = TaskFaker.ATask(TimeProvider.System);
            db.Tasks.Add(task);
            await db.SaveChangesAsync(TestContext.Current.CancellationToken);
            return task.Id;
        });

        // Act
        var response = await _client.PostAsync(
            new Uri($"/api/v1/tasks/{taskId.Value}/toggle", UriKind.Relative),
            content: null,
            TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>(TestContext.Current.CancellationToken);
        body.GetProperty("isCompleted").GetBoolean().ShouldBeTrue();
    }

    [Fact]
    public async Task When_DeletingExistingTask_Should_Return204()
    {
        // Arrange
        var taskId = await factory.WithDbAsync(async db =>
        {
            var task = TaskFaker.ATask(TimeProvider.System);
            db.Tasks.Add(task);
            await db.SaveChangesAsync(TestContext.Current.CancellationToken);
            return task.Id;
        });

        // Act
        var response = await _client.DeleteAsync(
            new Uri($"/api/v1/tasks/{taskId.Value}", UriKind.Relative),
            TestContext.Current.CancellationToken);

        // Assert
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

        // Arrange
        var url = new Uri($"/api/v1/tasks/{Guid.NewGuid()}{urlSuffix}", UriKind.Relative);
        using var request = new HttpRequestMessage(new HttpMethod(method), url);

        // Act
        var response = await _client.SendAsync(request, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>(TestContext.Current.CancellationToken);
        body.GetProperty("status").GetInt32().ShouldBe(404);
    }
}
