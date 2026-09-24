using CSharpFunctionalExtensions;
using DevQuestions.Application.Abstractions;
using DevQuestions.Application.Extensions;
using DevQuestions.Contracts.Questions.Dtos;
using DevQuestions.Domain.Questions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Shared;

namespace DevQuestions.Application.Questions.Features.AddAnswerCommand;

public class AddAnswerHandler : ICommandHandler<Guid, AddAnswerCommand>
{
    private readonly IQuestionsRepository _repository;
    private readonly IValidator<AddAnswerDto> _validator;
    // private readonly ITransactionManager _transactionManager;
    //private readonly IUsersCommunicationService _usersCommunicationService;
    private readonly ILogger<AddAnswerHandler> _logger;

    public AddAnswerHandler(
        IQuestionsRepository repository,
        IValidator<AddAnswerDto> validator,
        // ITransactionManager transactionManager,
        //IUsersCommunicationService usersCommunicationService,
        ILogger<AddAnswerHandler> logger)
    {
        _repository = repository;
        _validator = validator;
        // _transactionManager = transactionManager;
        //_usersCommunicationService = usersCommunicationService;
        _logger = logger;
    }

    public async Task<Result<Guid, Failure>> Handle(AddAnswerCommand command, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command.AddAnswerDto, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToErrors();
        }

        //var userRatingResult = await _usersCommunicationService.GetUserRatingAsync(command.AddAnswerDto.UserId, cancellationToken);
        //if (userRatingResult.IsFailure)
        //{
        //    return userRatingResult.Error;
        //}

        //if (userRatingResult.Value <= 0)
        //{
        //    _logger.LogError("User with ID {UserId} does not have enough rating to answer the question.", command.AddAnswerDto.UserId);
        //    return Errors.Questions.NotEnoughRating();
        //}

        // var transaction = await _transactionManager.BeginTransactionAsync(cancellationToken);

        var questionResult = await _repository.GetByIdAsync(command.QuestionId, cancellationToken);
        if (questionResult.IsFailure)
        {
            return questionResult.Error;
        }

        var answer = new Answer(Guid.NewGuid(), command.AddAnswerDto.UserId, command.AddAnswerDto.Text, command.QuestionId);

        questionResult.Value.Answers.Add(answer);

        await _repository.SaveAsync(questionResult.Value, cancellationToken);

        // transaction.Commit();

        _logger.LogInformation("Answer added with ID: {AnswerId} to Question ID: {QuestionId}", answer.Id, command.QuestionId);

        return answer.Id;
    }
}
