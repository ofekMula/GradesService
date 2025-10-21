namespace Grades.Application.Exceptions;

public sealed class PrincipalReportValidationException : Exception
{
    public PrincipalReportValidationException(string message)
        : base(message) { }
}
public sealed class StudentReportValidationException : Exception
{
    public StudentReportValidationException(string message)
        : base(message) { }
}



public sealed class PrincipalReportNoZones : Exception
{
    public PrincipalReportNoZones(IReadOnlyCollection<int>  snapshotIds)
        : base($"Snapshots [{string.Join(", ", snapshotIds)}] have no zones with scores.") { }
}

public sealed class StudentReportNoZones : Exception
{
    public StudentReportNoZones(int snapshotId)
        : base($"Snapshot {snapshotId} has no zones with scores.") { }
}
