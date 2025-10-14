namespace Grades.Application.DTOs;

using System.ComponentModel.DataAnnotations;
public record QuestionCreateDto(string QuestionText, int? Score, bool IsRelevant, int TestId);
public record QuestionUpdateDto(
    string? QuestionText,
    [Range(0, 100)] int? Score,
    bool? IsRelevant
);
