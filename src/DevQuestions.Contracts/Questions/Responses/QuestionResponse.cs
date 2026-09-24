using DevQuestions.Contracts.Questions.Dtos;

namespace DevQuestions.Contracts.Questions.Responses;

public record QuestionResponse(IEnumerable<QuestionDto> Questions, long TotalCount);
