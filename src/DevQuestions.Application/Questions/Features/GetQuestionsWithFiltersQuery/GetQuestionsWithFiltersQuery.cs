using DevQuestions.Application.Abstractions;
using DevQuestions.Contracts.Questions.Dtos;

namespace DevQuestions.Application.Questions.Features.GetQuestionsWithFiltersQuery;

public record GetQuestionsWithFiltersQuery(GetQuestionsDto GetQuestionsDto): IQuery;
