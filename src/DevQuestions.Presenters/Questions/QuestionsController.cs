using CSharpFunctionalExtensions;
using DevQuestions.Application.Questions;
using DevQuestions.Contracts.Questions;
using DevQuestions.Presenters.ResponseExtensions;
using Microsoft.AspNetCore.Mvc;
using Shared;

namespace DevQuestions.Presenters.Questions;

[ApiController]
[Route("[controller]")]
public class QuestionsController : ControllerBase
{
    private readonly IQuestionsService _questionsService;

    public QuestionsController(IQuestionsService questionsService)
    {
        _questionsService = questionsService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateQuestionDto createQuestionDto, CancellationToken cancellationToken)
    {
        Result<Guid, Failure> result = await _questionsService.Create(createQuestionDto, cancellationToken);

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
    public async Task<IActionResult> AddAnswer([FromRoute] Guid id, [FromBody] AddAnswerDto addAnswerDto, CancellationToken cancellationToken)
    {
        return Ok("Answer added successfully!");
    }
}
