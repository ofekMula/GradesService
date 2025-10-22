using Microsoft.AspNetCore.Mvc;
using Grades.Application.DTOs.Reports;
using Grades.Application.Exceptions;
using Grades.Application.Interfaces;

namespace Grades.Api.Controllers;

[ApiController]
[Route("api/snapshots")]
public class ReportsController : ControllerBase
{
    private readonly IReportService _service;
    private readonly ILogger<ReportsController> _log;

    public ReportsController(IReportService service , ILogger<ReportsController> log)
    {
        _service = service;
        _log = log;
    }

    [HttpGet("{snapshotId:int}/reports/student")]
    public async Task<ActionResult<StudentReportDto>> GetStudentReport(int snapshotId, CancellationToken ct)
    {
        _log.LogInformation("Student report requested for snapshot {SnapshotId}", snapshotId);

        try
        {
            var report = await _service.GenerateStudentReportAsync(snapshotId, ct);
            _log.LogInformation("Student report generated for snapshot {SnapshotId}", snapshotId);

            return Ok(report);
        }
        catch (SnapshotNotExistException ex)
        {
            _log.LogWarning(ex, "Snapshot not found for student report: {SnapshotId}", snapshotId);
            return NotFound(new { error = ex.Message });
        }

        catch (StudentReportNoZones ex)
        {
            _log.LogWarning(ex, "No zones available for student report: {SnapshotId}", snapshotId);
            return UnprocessableEntity(new
            {
                error = ex.Message,
                snapshots = (ex as dynamic)?.SnapshotId ?? null
            });
        }

        catch (StudentReportValidationException ex)
        {
            _log.LogWarning(ex, "Validation error generating student report: {SnapshotId}", snapshotId);
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        catch (Exception ex)
        {
            _log.LogError(ex, "Unexpected error generating student report for snapshot {SnapshotId}", snapshotId);
            return Problem(ex.Message);
        }
    }

    [HttpPost("reports/principal")]
    public async Task<ActionResult<PrincipalReportDto>> GetPrincipalReport(
        [FromBody] PrincipalReportRequest request,
        CancellationToken ct)
    {
        _log.LogInformation(
        "Principal report requested for {Count} snapshot(s): {SnapshotIds}",
        request?.SnapshotIds?.Count ?? 0,
        request?.SnapshotIds
    );
        try
        {
            var report = await _service.GeneratePrincipalReportAsync(request.SnapshotIds, ct);
            _log.LogInformation(
            "Principal report generated for {Count} snapshot(s)",
            request.SnapshotIds?.Count ?? 0);
            return Ok(report);
        }
        catch (SnapshotNotExistException ex)
        {
            _log.LogWarning(ex, "One or more snapshots not found for principal report: {SnapshotIds}", request?.SnapshotIds);
            return NotFound(new { error = ex.Message });
        }

        catch (PrincipalReportValidationException ex)
        {
            _log.LogWarning(ex, "Validation error generating principal report: {SnapshotIds}", request?.SnapshotIds);
            return BadRequest(new { error = ex.Message });
        }

        catch (PrincipalReportNoZones ex)
        {
            _log.LogWarning(ex, "No zones available for principal report: {SnapshotIds}", request?.SnapshotIds);
            return UnprocessableEntity(new
            {
                error = ex.Message,
                snapshots = (ex as dynamic)?.SnapshotIds ?? null
            });
        }
        catch (Exception ex)
        {
            _log.LogError(ex, "Unexpected error generating principal report for snapshots: {SnapshotIds}", request?.SnapshotIds);
            return Problem(ex.Message);
        }
    }
}
