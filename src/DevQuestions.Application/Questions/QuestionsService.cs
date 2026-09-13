using DevQuestions.Contracts.Questions;
using DevQuestions.Domain.Questions;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace DevQuestions.Application.Questions;

public class QuestionsService : IQuestionsService
{
    private readonly IQuestionsRepository _questionsRepository;
    private readonly IValidator<CreateQuestionDto> _createQuestionValidator;
    private readonly ILogger<QuestionsService> _logger;

    public QuestionsService(
        IQuestionsRepository questionsRepository,
        IValidator<CreateQuestionDto> createQuestionValidator,
        ILogger<QuestionsService> logger)
    {
        _questionsRepository = questionsRepository;
        _createQuestionValidator = createQuestionValidator;
        _logger = logger;
    }

    public async Task<Guid> Create(CreateQuestionDto questionDto, CancellationToken cancellationToken)
    {
        var validationResult = await _createQuestionValidator.ValidateAsync(questionDto, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        int openUserQuestionsCount = await _questionsRepository.GetOpenUserQuestionsCountAsync(questionDto.UserId, cancellationToken);
        if (openUserQuestionsCount > 3)
        {
            throw new InvalidOperationException("A user cannot have more than 3 open questions.");
        }

        var questionId = Guid.NewGuid();

        var question = new Question(
            questionId,
            questionDto.Title,
            questionDto.Text,
            questionDto.UserId,
            null,
            questionDto.TagIds);

        await _questionsRepository.AddAsync(question, cancellationToken);

        _logger.LogInformation("Question created with ID: {QuestionId}", questionId);

        return questionId;
    }

    public async Task Update(Guid id, UpdateQuestionDto updateQuestionDto, CancellationToken cancellationToken)
    {
    }

    public async Task Delete(Guid id, CancellationToken cancellationToken)
    {
    }

    public async Task SelectSolution(Guid id, Guid answerId, CancellationToken cancellationToken)
    {
    }

    public async Task AddAnswer(Guid id, AddAnswerDto addAnswerDto, CancellationToken cancellationToken)
    {
    }
}
