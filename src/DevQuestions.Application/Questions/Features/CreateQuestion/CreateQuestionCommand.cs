using DevQuestions.Application.Abstractions;
using DevQuestions.Contracts.Questions;

namespace DevQuestions.Application.Questions.Features.CreateQuestion;

public record CreateQuestionCommand(CreateQuestionDto CreateQuestionDto): ICommand;
