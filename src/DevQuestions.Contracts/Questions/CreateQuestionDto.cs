namespace DevQuestions.Contracts.Questions;

public record CreateQuestionDto(string Title, string Text, Guid UserId, Guid[] TagIds);
