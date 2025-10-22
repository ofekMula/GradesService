namespace Grades.Application.DTOs.Reports;

public record ZoneScoreDto(
    int ZoneId,
    string ZoneName,
    double Score
);

public record StudentReportDto(
    string Title,
    DateTime CreatedAtUtc,
    IReadOnlyList<ZoneScoreDto> TopZones,
    IReadOnlyList<ZoneScoreDto> BottomZones,
    IReadOnlyList<ZoneScoreDto> LowZones
);
