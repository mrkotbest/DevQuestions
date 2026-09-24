using CSharpFunctionalExtensions;
using DevQuestions.Application.Abstractions;
using DevQuestions.Application.Extensions;
using DevQuestions.Contracts.Questions.Dtos;
using DevQuestions.Domain.Questions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Shared;

namespace DevQuestions.Application.Questions.Features.CreateQuestionCommand;

public class CreateQuestionHandler : ICommandHandler<Guid, CreateQuestionCommand>
{
    private readonly IQuestionsRepository _repository;
    private readonly IValidator<CreateQuestionDto> _validator;
    private readonly ILogger<CreateQuestionHandler> _logger;

    public CreateQuestionHandler(
        IQuestionsRepository repository,
        IValidator<CreateQuestionDto> validator,
        ILogger<CreateQuestionHandler> logger)
    {
        _repository = repository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<Guid, Failure>> Handle(CreateQuestionCommand command, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(command.CreateQuestionDto, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToErrors();
        }

        int openUserQuestionsCount = await _repository.GetOpenUserQuestionsCountAsync(command.CreateQuestionDto.UserId, cancellationToken);
        if (openUserQuestionsCount > 3)
        {
            return Failures.Errors.Questions.TooManyQuestions().ToFailure();
        }

        var questionId = Guid.NewGuid();

        var question = new Question(
            questionId,
            command.CreateQuestionDto.Title,
            command.CreateQuestionDto.Text,
            command.CreateQuestionDto.UserId,
            null,
            command.CreateQuestionDto.TagIds);

        await _repository.AddAsync(question, cancellationToken);

        _logger.LogInformation("Question created with ID: {QuestionId}", questionId);

        return questionId;
    }
}
