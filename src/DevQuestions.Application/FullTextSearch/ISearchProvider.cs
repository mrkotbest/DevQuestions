using DevQuestions.Domain.Questions;

namespace DevQuestions.Application.FullTextSearch;

public interface ISearchProvider
{
    Task<List<Guid>> SearchAsync(string query);

    Task IndexQuestionAsync(Question question);
}
