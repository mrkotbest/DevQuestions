namespace DevQuestions.Domain.Questions;

public static class QuestionStatusExtensions
{
    extension(QuestionStatus status)
    {
        public string ToRuString()
        {
            return status switch
            {
                QuestionStatus.Open => "Открыт",
                QuestionStatus.Resolved => "Решён",
                _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
            };
        }
    }
}
