using DevQuestions.Application.Abstractions;
using DevQuestions.Contracts.Questions.Dtos;

namespace DevQuestions.Application.Questions.Features.AddAnswerCommand;

public record AddAnswerCommand(Guid QuestionId, AddAnswerDto AddAnswerDto): ICommand;
