using Grades.Domain.Models;
using Grades.Application.DTOs;

namespace Grades.Application.Interfaces;

public interface IQuestionService
{
    Task<IReadOnlyList<QuestionDto>> GetAllAsync(int snapshotId, CancellationToken ct = default);
    Task<QuestionDto?> CreateAsync(int snapshotId, QuestionCreateDto dto, CancellationToken ct = default);
    Task<QuestionDto?> UpdateAsync(int snapshotId, int questionId, QuestionUpdateDto dto, CancellationToken ct = default);
    Task DeleteAsync(int snapshotId, int questionId, CancellationToken ct = default);
}
