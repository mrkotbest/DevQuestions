using DevQuestions.Application.FullTextSearch;
using DevQuestions.Domain.Questions;

namespace DevQuestions.Infrastructure.ElasticSearch;

public class ElasticSearchProvider : ISearchProvider
{
    public Task IndexQuestionAsync(Question question)
    {
        throw new NotImplementedException();
    }

    public Task<List<Guid>> SearchAsync(string query)
    {
        throw new NotImplementedException();
    }
}
