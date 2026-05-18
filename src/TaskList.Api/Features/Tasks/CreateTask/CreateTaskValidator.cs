using FluentValidation;
using TaskList.Domain.Tasks;

namespace TaskList.Api.Features.Tasks.CreateTask;

public sealed class CreateTaskValidator : AbstractValidator<CreateTaskCommand>
{
    public CreateTaskValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(TaskItem.TitleMaxLength).WithMessage($"Title must be {TaskItem.TitleMaxLength} characters or fewer.");
    }
}
