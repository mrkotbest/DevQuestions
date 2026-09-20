using DevQuestions.Contracts.Questions;
using FluentValidation;

namespace DevQuestions.Application.Questions.Validators;

public class AddAnswerValidator : AbstractValidator<AddAnswerDto>
{
    public AddAnswerValidator()
    {
        RuleFor(x => x.Text)
            .NotEmpty()
            .WithMessage("Text is required.")
            .WithErrorCode("value.invalid")
            .MaximumLength(5000)
            .WithMessage("Text must be at most 5000 characters long.")
            .WithErrorCode("value.invalid");
    }
}
