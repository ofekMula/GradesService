namespace Grades.Application.DTOs;

using System.ComponentModel.DataAnnotations;

public record QuestionCreateDto(
    [Required, StringLength(1000)] string QuestionText,
    [Range(typeof(int), "0", "100")] int? Score,
    bool IsRelevant,
    [Range(1, int.MaxValue)] int TestId,
    [Range(1, int.MaxValue)] int ZoneId

);

public record QuestionUpdateDto(
    [StringLength(1000)] string? QuestionText,
    [Range(typeof(int), "0", "100")] int? Score,
    bool? IsRelevant
);

public record QuestionDto(
    int QuestionId,
    string QuestionText,
    int? Score,
    bool IsRelevant,
    int TestId
);
