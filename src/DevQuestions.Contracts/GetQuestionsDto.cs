namespace DevQuestions.Contracts;

public record GetQuestionsDto(string Search, Guid[] TagIds, int PageSize, int Limit);
