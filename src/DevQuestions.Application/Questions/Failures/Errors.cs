using Shared;

namespace DevQuestions.Application.Questions.Failures;

public partial class Errors
{
    public static class Questions
    {
        public static Error TooManyQuestions()
        {
            return Error.Forbidden(
                "questions.too.many",
                "A user cannot have more than 3 open questions.");
        }
    }
}
