CREATE OR ALTER PROCEDURE CalculateScorePerSnapshot
  @SnapshotId INT
AS
BEGIN
  SET NOCOUNT ON;

  WITH BaseRawScores AS (
    SELECT
      s.SubjectId,
      s.SubjectName,
      z.ZoneId,
      q.QuestionId,
      q.Score,
      t.IsATest
    FROM Subjects s
    JOIN SubjectZones sz
      ON sz.SnapshotId = s.SnapshotId
     AND sz.SubjectId  = s.SubjectId
    JOIN Zones z
      ON z.SnapshotId = sz.SnapshotId
     AND z.ZoneId = sz.ZoneId
     AND z.IsRelevant = 1
    JOIN ZonesQuestions zq
      ON zq.SnapshotId  = z.SnapshotId
     AND zq.ZoneId      = z.ZoneId
    JOIN Questions q
      ON q.SnapshotId   = zq.SnapshotId
     AND q.QuestionId   = zq.QuestionId
     AND q.IsRelevant   = 1
    JOIN Tests t
      ON t.TestId       = q.TestId
    WHERE s.SnapshotId  = @SnapshotId
      AND q.SnapshotId  = @SnapshotId
  ),
  ZoneAgg AS (
    SELECT
      b.SubjectId,
      b.ZoneId,
      b.IsATest,
      AVG(CAST(b.Score AS FLOAT)) AS ZoneAvg
    FROM BaseRawScores b
    GROUP BY b.SubjectId, b.ZoneId, b.IsATest
  ),
  SubjectScore AS (
    SELECT
      za.SubjectId,
      za.IsATest,
      AVG(za.ZoneAvg) AS SubjectAvg
    FROM ZoneAgg za
    GROUP BY za.SubjectId, za.IsATest
  ),
  SubjectQCounts AS (
    SELECT
      b.SubjectId,
      b.IsATest,
      COUNT(DISTINCT b.QuestionId) AS NumQuestions,
      COUNT(DISTINCT CASE WHEN b.Score IS NOT NULL THEN b.QuestionId END) AS NumAnswered
    FROM BaseRawScores b
    GROUP BY b.SubjectId, b.IsATest
  )
  SELECT
    @SnapshotId  AS SnapshotId,
    s.SubjectId,
    s.SubjectName,
    ISNULL(qn.NumQuestions, 0) AS NumNationalQuestions,
    ISNULL(qn.NumAnswered, 0) AS NumNationalAnsweredQuestions,
    CAST(ISNULL(sn.SubjectAvg, 0) AS DECIMAL(5,2)) AS NationalTestScores,
    ISNULL(qnn.NumQuestions, 0) AS NumNonNationalQuestions,
    ISNULL(qnn.NumAnswered, 0) AS NumNonNationalAnsweredQuestions,
    CAST(ISNULL(snn.SubjectAvg, 0) AS DECIMAL(5,2)) AS NonNationalTestScores
  FROM Subjects s
  LEFT JOIN SubjectQCounts qn
    ON qn.SubjectId = s.SubjectId AND qn.IsATest = 1
  LEFT JOIN SubjectQCounts qnn
    ON qnn.SubjectId = s.SubjectId AND qnn.IsATest = 0
  LEFT JOIN SubjectScore sn
    ON sn.SubjectId = s.SubjectId AND sn.IsATest = 1
  LEFT JOIN SubjectScore snn
    ON snn.SubjectId = s.SubjectId AND snn.IsATest = 0
  WHERE s.SnapshotId = @SnapshotId
  ORDER BY s.SubjectName;
END
