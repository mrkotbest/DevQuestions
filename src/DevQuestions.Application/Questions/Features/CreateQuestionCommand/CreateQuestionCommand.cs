using DevQuestions.Application.Abstractions;
using DevQuestions.Contracts.Questions.Dtos;

namespace DevQuestions.Application.Questions.Features.CreateQuestionCommand;

public record CreateQuestionCommand(CreateQuestionDto CreateQuestionDto): ICommand;
