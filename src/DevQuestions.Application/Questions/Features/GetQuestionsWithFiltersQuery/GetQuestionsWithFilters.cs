using CSharpFunctionalExtensions;
using DevQuestions.Application.Abstractions;
using DevQuestions.Application.FilesStorage;
using DevQuestions.Application.Tags;
using DevQuestions.Contracts.Questions.Dtos;
using DevQuestions.Contracts.Questions.Responses;
using DevQuestions.Domain.Questions;
using Microsoft.EntityFrameworkCore;

namespace DevQuestions.Application.Questions.Features.GetQuestionsWithFiltersQuery;

public class GetQuestionsWithFilters : IQueryHandler<QuestionResponse, GetQuestionsWithFiltersQuery>
{
    private readonly IFileProvider _fileProvider;
    private readonly ITagsReadDbContext _tagsReadDbContext;
    private readonly IQuestionsReadDbContext _questionsReadDbContext;

    public GetQuestionsWithFilters(
        IFileProvider fileProvider,
        ITagsReadDbContext tagsReadDbContext,
        IQuestionsReadDbContext questionsReadDbContext)
    {
        _fileProvider = fileProvider;
        _tagsReadDbContext = tagsReadDbContext;
        _questionsReadDbContext = questionsReadDbContext;
    }

    public async Task<QuestionResponse> Handle(GetQuestionsWithFiltersQuery command, CancellationToken cancellationToken)
    {
        var questions = await _questionsReadDbContext.ReadQuestions
            .Include(q => q.Solution)
            .Take(command.GetQuestionsDto.PageSize)
            .ToListAsync(cancellationToken);

        long count = await _questionsReadDbContext.ReadQuestions.LongCountAsync(cancellationToken);

        var screenshotIds = questions
            .Where(q => q.ScreenshotId is not null)
            .Select(q => q.ScreenshotId!.Value);

        var filesDict = await _fileProvider.GetUrlsByIdsAsync(screenshotIds, cancellationToken);

        var questionTags = questions.SelectMany(q => q.Tags);

        var tags = await _tagsReadDbContext.TagsRead
            .Where(t => questionTags.Contains(t.Id))
            .Select(t => t.Name)
            .ToListAsync(cancellationToken);

        var questionsDto = questions.Select(q => new QuestionDto(
            q.Id,
            q.Title,
            q.Text,
            q.UserId,
            q.ScreenshotId is not null ? filesDict[key: q.ScreenshotId.Value] : string.Empty,
            q.Solution?.Id,
            tags,
            q.Status.ToRuString()));

        return new QuestionResponse(questionsDto, count);
    }
}
