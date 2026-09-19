using CSharpFunctionalExtensions;
using DevQuestions.Domain.Questions;
using Shared;

namespace DevQuestions.Application.FullTextSearch;

public interface ISearchProvider
{
    Task<List<Guid>> SearchAsync(string query);

    Task<UnitResult<Failure>> IndexQuestionAsync(Question question);
}
