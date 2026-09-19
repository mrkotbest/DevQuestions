using DevQuestions.Application.Exceptions;

namespace DevQuestions.Application.Questions.Failures.Exceptions;

public class TooManyQuestionsException : BadRequestException
{
    public TooManyQuestionsException()
        : base([Failures.Errors.Questions.TooManyQuestions()])
    {
    }
}
