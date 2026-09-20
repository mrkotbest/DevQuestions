using DevQuestions.Application.Abstractions;
using DevQuestions.Application.Questions.Features.AddAnswer;
using DevQuestions.Application.Questions.Features.CreateQuestion;
using DevQuestions.Contracts.Questions;
using DevQuestions.Presenters.ResponseExtensions;
using Microsoft.AspNetCore.Mvc;

namespace DevQuestions.Presenters.Questions;

[ApiController]
[Route("[controller]")]
public class QuestionsController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromServices] ICommandHandler<Guid, CreateQuestionCommand> handler,
        [FromBody] CreateQuestionDto createQuestionDto,
        CancellationToken cancellationToken)
    {
        var command = new CreateQuestionCommand(createQuestionDto);

        var result = await handler.Handle(command, cancellationToken);

        return result.IsFailure ? result.Error.ToResponse() : Ok(result.Value);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] GetQuestionsDto getQuestionsDto, CancellationToken cancellationToken)
    {
        return Ok("Returning all questions!");
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById([FromRoute]Guid id, CancellationToken cancellationToken)
    {
        return Ok("Returning question by ID!");
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateQuestionDto updateQuestionDto, CancellationToken cancellationToken)
    {
        return Ok("Question updated successfully!");
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        return Ok("Question deleted successfully!");
    }

    [HttpPut("{id:guid}/solution")]
    public async Task<IActionResult> SelectSolution([FromRoute] Guid id, [FromQuery] Guid answerId, CancellationToken cancellationToken)
    {
        return Ok("Solution selected successfully!");
    }

    [HttpPost("{id:guid}/answers")]
    public async Task<IActionResult> AddAnswer(
        [FromServices] ICommandHandler<Guid, AddAnswerCommand> handler,
        [FromRoute] Guid id,
        [FromBody] AddAnswerDto addAnswerDto,
        CancellationToken cancellationToken)
    {
        var command = new AddAnswerCommand(id, addAnswerDto);

        var result = await handler.Handle(command, cancellationToken);

        return result.IsFailure ? result.Error.ToResponse() : Ok(result.Value);
    }
}
