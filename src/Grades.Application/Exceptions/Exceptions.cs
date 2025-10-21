namespace Grades.Application.Exceptions;


public sealed class QuestionNotFoundException : Exception
{
    public QuestionNotFoundException(int snapshotId, int questionId)
        : base($"Question of Snapshot: {snapshotId} with ID: '{questionId}' was not found.") { }
}

public sealed class ZoneNotFoundException : Exception
{
    public ZoneNotFoundException(int snapshotId, int zoneId)
        : base($"Zone of Snapshot: {snapshotId} with ID: '{zoneId}' was not found.") { }
}

public sealed class TestNotFoundException : Exception
{
    public TestNotFoundException(int testId)
        : base($"Test with ID: '{testId}' was not found.") { }
}

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

public sealed class SnapshotNotExistException : Exception
{
    public SnapshotNotExistException(int snapshotId)
        : base($"Snapshot '{snapshotId}' does not exist or has no questions.") { }

    public SnapshotNotExistException(IEnumerable<int> snapshotIds)
        : base($"Snapshots [{string.Join(", ", snapshotIds)}] do not exist or have no questions.") { }

    public SnapshotNotExistException(params int[] snapshotIds)
        : this((IEnumerable<int>)snapshotIds) { }
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
