using DevQuestions.Contracts.Questions;
using FluentValidation;

namespace DevQuestions.Application.Questions.Features.CreateQuestion;

public class CreateQuestionValidator : AbstractValidator<CreateQuestionDto>
{
    public CreateQuestionValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Question title is required.")
            .WithErrorCode("value.invalid")
            .MaximumLength(500)
            .WithMessage("Question title must be at most 500 characters long.")
            .WithErrorCode("value.invalid");

        RuleFor(x => x.Text)
            .NotEmpty()
            .WithMessage("Question text is required.")
            .WithErrorCode("value.invalid")
            .MaximumLength(5000)
            .WithMessage("Question text must be at most 5000 characters long.")
            .WithErrorCode("value.invalid");

        RuleFor(x => x.UserId)
            .NotEmpty();
    }
}
