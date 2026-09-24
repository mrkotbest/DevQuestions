using CSharpFunctionalExtensions;
using DevQuestions.Application.Questions;
using DevQuestions.Application.Questions.Failures;
using DevQuestions.Domain.Questions;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace DevQuestions.Infrastructure.PostgreSql.Questions;

public class QuestionsEfCoreRepository : IQuestionsRepository
{
    private readonly QuestionsDbContext _dbContext;

    public QuestionsEfCoreRepository(QuestionsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Guid> AddAsync(Question question, CancellationToken cancellationToken)
    {
        await _dbContext.Questions.AddAsync(question, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return question.Id;
    }

    public async Task<Guid> DeleteAsync(Guid questionId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<Guid> SaveAsync(Question question, CancellationToken cancellationToken)
    {
        _dbContext.Questions.Update(question);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return question.Id;
    }

    public async Task<Result<Question, Failure>> GetByIdAsync(Guid questionId, CancellationToken cancellationToken)
    {
        var question = await _dbContext.Questions
            .Include(q => q.Answers)
            .Include(q => q.Solution)
            .FirstOrDefaultAsync(q => q.Id == questionId, cancellationToken);

        if (question == null)
        {
            return Errors.General.NotFound(questionId).ToFailure();
        }

        return question;
    }

    public Task<int> GetOpenUserQuestionsCountAsync(Guid userId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
