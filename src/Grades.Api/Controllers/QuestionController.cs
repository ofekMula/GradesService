using Microsoft.AspNetCore.Mvc;
using Grades.Application.Interfaces;
using Grades.Application.DTOs;
using Grades.Application.Exceptions;

namespace Grades.Api.Controllers;

[ApiController]
[Route("api/snapshots/{snapshotId:int}/questions")]
public class QuestionsController : ControllerBase
{
    private readonly IQuestionService _service;
    private readonly ILogger<QuestionsController> _log;
      public QuestionsController(IQuestionService service, ILogger<QuestionsController> log)
    {
        _service = service;
        _log = log;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<QuestionDto>>> GetAll(
        int snapshotId,
        [FromQuery] PaginationDto pagination,
        CancellationToken ct = default)
    {
        _log.LogInformation("Handling GetAll questions for snapshot {SnapshotId} with {@Pagination}", snapshotId, pagination);

        var items = await _service.GetAllAsync(snapshotId, pagination, ct);
        return Ok(items);
    }


    [HttpPost]
    public async Task<ActionResult<QuestionDto>> Create(
        int snapshotId, [FromBody] QuestionCreateDto dto, CancellationToken ct)
    {
        _log.LogInformation("Create question requested for snapshot {SnapshotId}, zone {ZoneId}, test {TestId}",
        snapshotId, dto?.ZoneId, dto?.TestId);
        try
        {
            var created = await _service.CreateAsync(snapshotId, dto, ct);

            _log.LogInformation("Question created: {QuestionId} (snapshot {SnapshotId})",
            created.QuestionId, snapshotId);

            var location = $"/api/snapshots/{snapshotId}/questions/{created.QuestionId}";
            return Created(location, created);
        }
        catch (ZoneNotFoundException ex)
        {
            _log.LogWarning(ex, "Zone not found for snapshot {SnapshotId}, zone {ZoneId}",
            snapshotId, dto?.ZoneId);
            return BadRequest(new { error = ex.Message });
        }
        catch (TestNotFoundException ex)
        {
            _log.LogWarning(ex, "Test not found for snapshot {SnapshotId}, test {TestId}",
            snapshotId, dto?.TestId);
            return BadRequest(new { error = ex.Message });
        }

        catch (Exception ex)
        {
            _log.LogError(ex, "Unexpected error creating question for snapshot {SnapshotId}", snapshotId);
            return Problem(ex.Message);
        }
    }

    [HttpPut("{questionId:int}")]
    public async Task<ActionResult<QuestionDto>> Update(
        int snapshotId, int questionId, [FromBody] QuestionUpdateDto dto, CancellationToken ct)
    {
        _log.LogInformation("Update question requested: snapshot {SnapshotId}, question {QuestionId}",
        snapshotId, questionId);
        try
        {
            var updated = await _service.UpdateAsync(snapshotId, questionId, dto, ct);
            _log.LogInformation("Question updated: {QuestionId} (snapshot {SnapshotId})", questionId, snapshotId);

            return Ok(updated);
        }
        catch (QuestionNotFoundException ex)
        {
            _log.LogWarning(ex, "Question not found: snapshot {SnapshotId}, question {QuestionId}",
            snapshotId, questionId);
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Unexpected error updating question: snapshot {SnapshotId}, question {QuestionId}",
            snapshotId, questionId);
            return Problem(ex.Message);
        }
    }

    [HttpDelete("{questionId:int}")]
    public async Task<IActionResult> Delete(
        int snapshotId, int questionId, CancellationToken ct)
    {
        _log.LogInformation("Delete question requested: snapshot {SnapshotId}, question {QuestionId}",
        snapshotId, questionId);
        try
        {
            await _service.DeleteAsync(snapshotId, questionId, ct);
            _log.LogInformation("Question deleted: {QuestionId} (snapshot {SnapshotId})",
            questionId, snapshotId);
            return NoContent();
        }
        catch (QuestionNotFoundException ex)
        {
            _log.LogWarning(ex, "Question not found: snapshot {SnapshotId}, question {QuestionId}",
            snapshotId, questionId);
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Unexpected error deleting question: snapshot {SnapshotId}, question {QuestionId}",
            snapshotId, questionId);
            return Problem(ex.Message);
        }
    }
}
