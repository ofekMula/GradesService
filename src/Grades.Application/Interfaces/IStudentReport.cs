using System.Threading;
using System.Threading.Tasks;
using Grades.Application.DTOs.Reports;

namespace Grades.Application.Interfaces;
public interface IReportService
{
    Task<StudentReportDto> GenerateStudentReportAsync(int snapshotId, CancellationToken ct = default);
    Task<PrincipalReportDto> GeneratePrincipalReportAsync(
        IReadOnlyCollection<int> snapshotIds,
        CancellationToken ct = default);
}
