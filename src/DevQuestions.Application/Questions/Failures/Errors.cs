using Shared;

namespace DevQuestions.Application.Questions.Failures;

public partial class Errors
{
    public static class General
    {
        public static Error NotFound(Guid questionId)
        {
            return Error.Forbidden(
                "general.not.found",
                $"Question not found by ID: {questionId}");
        }
    }

    public static class Questions
    {
        public static Error TooManyQuestions()
        {
            return Error.Forbidden(
                "questions.too.many",
                "A user cannot have more than 3 open questions.");
        }

        public static Failure NotEnoughRating()
        {
            return Error.Forbidden(
                "questions.not.enough.rating",
                "A user does not have enough rating to answer this question.");
        }
    }
}
