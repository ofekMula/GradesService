namespace Grades.Application.DTOs;

using Grades.Domain.Models;

public static class QuestionMapper
{
    public static QuestionDto ToDto(this Question q) => new(
        q.QuestionId,
        q.QuestionText,
        q.Score,
        q.IsRelevant,
        q.TestId
    );

    public static Question ToEntity(this QuestionCreateDto dto, int snapshotId, int nextId) => new()
    {
        SnapshotId   = snapshotId,
        QuestionId   = nextId,
        QuestionText = dto.QuestionText,
        Score        = dto.Score,
        IsRelevant   = dto.IsRelevant,
        TestId       = dto.TestId
    };

}
