using CSharpFunctionalExtensions;
using DevQuestions.Application.Communication;
using DevQuestions.Application.Database;
using DevQuestions.Application.Extensions;
using DevQuestions.Application.Questions.Failures;
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
    private readonly IValidator<AddAnswerDto> _addAnswerValidator;
    private readonly ITransactionManager _transactionManager;
    private readonly IUsersCommunicationService _usersCommunicationService;
    private readonly ILogger<QuestionsService> _logger;

    public QuestionsService(
        IQuestionsRepository questionsRepository,
        IValidator<CreateQuestionDto> createQuestionValidator,
        IValidator<AddAnswerDto> addAnswerValidator,
        ITransactionManager transactionManager,
        IUsersCommunicationService usersCommunicationService,
        ILogger<QuestionsService> logger)
    {
        _questionsRepository = questionsRepository;
        _createQuestionValidator = createQuestionValidator;
        _addAnswerValidator = addAnswerValidator;
        _transactionManager = transactionManager;
        _usersCommunicationService = usersCommunicationService;
        _logger = logger;
    }

    public async Task<Result<Guid, Failure>> Create(CreateQuestionDto questionDto, CancellationToken cancellationToken)
    {
        var validationResult = await _createQuestionValidator.ValidateAsync(questionDto, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToErrors();
        }

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

        _logger.LogInformation("Question created with ID: {QuestionId}", questionId);

        return questionId;
    }

    /*public async Task Update(Guid id, UpdateQuestionDto updateQuestionDto, CancellationToken cancellationToken)
    {
    }

    public async Task Delete(Guid id, CancellationToken cancellationToken)
    {
    }

    public async Task SelectSolution(Guid id, Guid answerId, CancellationToken cancellationToken)
    {
    }*/

    public async Task<Result<Guid, Failure>> AddAnswer(Guid questionId, AddAnswerDto addAnswerDto, CancellationToken cancellationToken)
    {
        var validationResult = await _addAnswerValidator.ValidateAsync(addAnswerDto, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToErrors();
        }

        var userRatingResult = await _usersCommunicationService.GetUserRatingAsync(addAnswerDto.UserId, cancellationToken);
        if (userRatingResult.IsFailure)
        {
            return userRatingResult.Error;
        }

        if (userRatingResult.Value <= 0)
        {
            _logger.LogError("User with ID {UserId} does not have enough rating to answer the question.", addAnswerDto.UserId);
            return Errors.Questions.NotEnoughRating();
        }

        var transaction = await _transactionManager.BeginTransactionAsync(cancellationToken);

        var questionResult = await _questionsRepository.GetByIdAsync(questionId, cancellationToken);
        if (questionResult.IsFailure)
        {
            return questionResult.Error;
        }

        var answer = new Answer(Guid.NewGuid(), addAnswerDto.UserId, addAnswerDto.Text, questionId);

        questionResult.Value.Answers.Add(answer);

        await _questionsRepository.SaveAsync(questionResult.Value, cancellationToken);

        transaction.Commit();

        _logger.LogInformation("Answer added with ID: {AnswerId} to Question ID: {QuestionId}", answer.Id, questionId);

        return answer.Id;
    }
}

/*public class QuestionCalculator
{
    public UnitResult<Failure> Calculate()
    {
        // Perform some calculations or validations here
        return Error.Conflict(null, string.Empty).ToFailure();
    }
}*/
