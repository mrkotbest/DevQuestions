namespace DevQuestions.Contracts.Questions.Dtos;

public record GetQuestionsDto(string Search, Guid[] TagIds, int PageSize, int Limit);
