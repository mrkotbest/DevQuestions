using CSharpFunctionalExtensions;
using DevQuestions.Contracts.Questions;
using Shared;

namespace DevQuestions.Application.Questions;

//public interface IQuestionsService
//{
//    /// <summary>Creates a new question.</summary>
//    /// <param name="createQuestionDto">The DTO containing the question details.</param>
//    /// <param name="cancellationToken">The cancellation token.</param>
//    /// <returns>A result indicating the outcome of the operation.</returns>
//    Task<Result<Guid, Failure>> Create(CreateQuestionDto createQuestionDto, CancellationToken cancellationToken);

//    /// <summary>
//    /// Adds an answer to a question.
//    /// </summary>
//    /// <param name="id">The ID of the question to which to add an answer.</param>
//    /// <param name="addAnswerDto">The DTO containing the answer details.</param>
//    /// <param name="cancellationToken">The cancellation token.</param>
//    /// <returns>A result indicating the outcome of the operation.</returns>
//    Task<Result<Guid, Failure>> AddAnswer(Guid id, AddAnswerDto addAnswerDto, CancellationToken cancellationToken);
//}
