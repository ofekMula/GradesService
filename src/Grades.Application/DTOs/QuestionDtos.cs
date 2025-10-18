namespace Grades.Application.DTOs;

using System.ComponentModel.DataAnnotations;

public record QuestionCreateDto(
    [Required, StringLength(300)] string QuestionText,
    [Range(typeof(int), "0", "100")] int? Score,
    bool IsRelevant,
    [Range(1, int.MaxValue)] int TestId
);

public record QuestionUpdateDto(
    [StringLength(300)] string? QuestionText,
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
