using CSharpFunctionalExtensions;
using DevQuestions.Contracts.Questions;
using Shared;

namespace DevQuestions.Application.Questions;

public interface IQuestionsService
{
    Task<Result<Guid, Failure>> Create(CreateQuestionDto createQuestionDto, CancellationToken cancellationToken);
}
