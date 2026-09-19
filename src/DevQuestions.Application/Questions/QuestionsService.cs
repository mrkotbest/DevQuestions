using CSharpFunctionalExtensions;
using DevQuestions.Application.Extensions;
using DevQuestions.Contracts.Questions;
using DevQuestions.Domain.Questions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Shared;

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

    public async Task<Result<Guid, Failure>> Create(CreateQuestionDto questionDto, CancellationToken cancellationToken)
    {
        var validationResult = await _createQuestionValidator.ValidateAsync(questionDto, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToErrors();
        }

        var calculator = new QuestionCalculator();

        var calculationResult = calculator.Calculate();
        if (calculationResult.IsFailure)
        {
            return calculationResult.Error;
        }

        // Check if the question already exists (this is just a placeholder, you might want to implement a proper check)
        var exsitedQuestion = await _questionsRepository.GetByIdAsync(Guid.Empty, cancellationToken);

        int openUserQuestionsCount = await _questionsRepository.GetOpenUserQuestionsCountAsync(questionDto.UserId, cancellationToken);
        if (openUserQuestionsCount > 3)
        {
            return Failures.Errors.Questions.TooManyQuestions().ToFailure();
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

        // TODO: index the question in the search provider once the Elasticsearch adapter is implemented

        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation("Question created with ID: {QuestionId}", questionId);
        }

        // Wrap success value into a Result success
        return Result.Success<Guid, Failure>(questionId);
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

public class QuestionCalculator
{
    public UnitResult<Failure> Calculate()
    {
        // Perform some calculations or validations here
        return Error.Conflict(null, string.Empty).ToFailure();
    }
}
