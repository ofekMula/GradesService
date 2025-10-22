namespace Grades.Domain.Models;

public class Zone
{
    public int SnapshotId { get; set; }
    public int ZoneId { get; set; }
    public string ZoneName { get; set; } = default!;
    public bool IsRelevant { get; set; }
}
