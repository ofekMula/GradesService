using System.ComponentModel.DataAnnotations;
namespace Grades.Application.DTOs.Reports;

public record PrincipalReportRequest(
    [Required, MinLength(1)]
    IReadOnlyList<int> SnapshotIds
);
public record PrincipalReportDto(
    string Title,
    DateTime CreatedAtUtc,
    ZoneScoreDto LowestZone
);
