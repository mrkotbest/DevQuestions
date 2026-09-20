using DevQuestions.Application.Abstractions;
using DevQuestions.Contracts.Questions;

namespace DevQuestions.Application.Questions.Features.AddAnswer;

public record AddAnswerCommand(Guid QuestionId, AddAnswerDto AddAnswerDto): ICommand;
