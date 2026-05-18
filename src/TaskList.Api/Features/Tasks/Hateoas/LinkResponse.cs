namespace TaskList.Api.Features.Tasks.Hateoas;

public sealed record LinkResponse(string Href, string Rel, string Method);
