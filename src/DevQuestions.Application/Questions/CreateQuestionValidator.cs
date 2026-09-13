using DevQuestions.Contracts.Questions;
using FluentValidation;

namespace DevQuestions.Application.Questions;

public class CreateQuestionValidator : AbstractValidator<CreateQuestionDto>
{
    public CreateQuestionValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Question title is required.")
            .MaximumLength(500)
            .WithMessage("Question title must be at most 500 characters long.");

        RuleFor(x => x.Text)
            .NotEmpty()
            .WithMessage("Question text is required.")
            .MaximumLength(5000)
            .WithMessage("Question text must be at most 5000 characters long.");

        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}
