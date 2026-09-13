using DevQuestions.Application.Exceptions;
using Shared;

namespace DevQuestions.Application.Questions.Failures.Exceptions;

public class QuestionNotFoundException : NotFoundException
{
    protected QuestionNotFoundException(IEnumerable<Error> errors)
        : base(errors)
    {
    }
}
