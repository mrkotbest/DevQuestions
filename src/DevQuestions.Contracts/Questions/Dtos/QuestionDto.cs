namespace DevQuestions.Contracts.Questions.Dtos;

public record QuestionDto(Guid Id, string Title, string Text, Guid UserId, string? screenshotUrl, Guid? SolutionId, IEnumerable<string> Tags, string Status);
