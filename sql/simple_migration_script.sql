SET XACT_ABORT ON;
BEGIN TRAN;

DECLARE @Snap INT = 0;
DECLARE @SubjectName NVARCHAR(200) = N'Math';
DECLARE @TestName    NVARCHAR(200) = N'Catalog Test - Math';
DECLARE @IsATest     BIT           = 1;

DECLARE @Z1Name NVARCHAR(200) = N'Arithmetic';
DECLARE @Z2Name NVARCHAR(200) = N'Geometry';
DECLARE @Z3Name NVARCHAR(200) = N'Algebra';

DECLARE @Q1Text NVARCHAR(400) = N'1 + 1 = ?';
DECLARE @Q2Text NVARCHAR(400) = N'2 + 3 = ?';
DECLARE @Q3Text NVARCHAR(400) = N'Solve: 2x + 3 = 9';
DECLARE @Q4Text NVARCHAR(400) = N'Area of a circle?';

DECLARE @Q1ZoneName NVARCHAR(200) = @Z1Name;
DECLARE @Q2ZoneName NVARCHAR(200) = @Z1Name;
DECLARE @Q3ZoneName NVARCHAR(200) = @Z3Name;
DECLARE @Q4ZoneName NVARCHAR(200) = @Z2Name;

DECLARE @SubjectId INT =
(
  SELECT TOP 1 s.SubjectId
  FROM Subjects s
  WHERE s.SnapshotId = @Snap AND s.SubjectName = @SubjectName
);

-- Subject
IF @SubjectId IS NULL
BEGIN
  SELECT @SubjectId = ISNULL(MAX(SubjectId), 0) + 1 FROM Subjects;
  INSERT INTO Subjects (SnapshotId, SubjectId, SubjectName)
  VALUES (@Snap, @SubjectId, @SubjectName);
END

-- Zones
DECLARE @Z1Id INT =
(
  SELECT TOP 1 ZoneId FROM Zones
  WHERE SnapshotId=@Snap AND ZoneName=@Z1Name
);
IF @Z1Id IS NULL
BEGIN
  SELECT @Z1Id = ISNULL(MAX(ZoneId), 0) + 1 FROM Zones;
  INSERT INTO Zones (SnapshotId, ZoneId, ZoneName, IsRelevant)
  VALUES (@Snap, @Z1Id, @Z1Name, 1);
END

IF NOT EXISTS (
  SELECT 1 FROM SubjectZones
  WHERE SnapshotId=@Snap AND SubjectId=@SubjectId AND ZoneId=@Z1Id
)
  INSERT INTO SubjectZones (SnapshotId, SubjectId, ZoneId)
  VALUES (@Snap, @SubjectId, @Z1Id);

DECLARE @Z2Id INT =
(
  SELECT TOP 1 ZoneId FROM Zones
  WHERE SnapshotId=@Snap AND ZoneName=@Z2Name
);
IF @Z2Id IS NULL
BEGIN
  SELECT @Z2Id = ISNULL(MAX(ZoneId), 0) + 1 FROM Zones;
  INSERT INTO Zones (SnapshotId, ZoneId, ZoneName, IsRelevant)
  VALUES (@Snap, @Z2Id, @Z2Name, 1);
END
IF NOT EXISTS (
  SELECT 1 FROM SubjectZones
  WHERE SnapshotId=@Snap AND SubjectId=@SubjectId AND ZoneId=@Z2Id
)
  INSERT INTO SubjectZones (SnapshotId, SubjectId, ZoneId)
  VALUES (@Snap, @SubjectId, @Z2Id);

DECLARE @Z3Id INT =
(
  SELECT TOP 1 ZoneId FROM Zones
  WHERE SnapshotId=@Snap AND ZoneName=@Z3Name
);
IF @Z3Id IS NULL
BEGIN
  SELECT @Z3Id = ISNULL(MAX(ZoneId), 0) + 1 FROM Zones;
  INSERT INTO Zones (SnapshotId, ZoneId, ZoneName, IsRelevant)
  VALUES (@Snap, @Z3Id, @Z3Name, 1);
END
IF NOT EXISTS (
  SELECT 1 FROM SubjectZones
  WHERE SnapshotId=@Snap AND SubjectId=@SubjectId AND ZoneId=@Z3Id
)
  INSERT INTO SubjectZones (SnapshotId, SubjectId, ZoneId)
  VALUES (@Snap, @SubjectId, @Z3Id);

-- Test
DECLARE @TestId INT =
(
  SELECT TOP 1 t.TestId FROM Tests t
  WHERE t.TestName = @TestName
);
IF @TestId IS NULL
BEGIN
  SELECT @TestId = ISNULL(MAX(TestId), 0) + 1 FROM Tests;
  INSERT INTO Tests (TestId, TestName, IsATest)
  VALUES (@TestId, @TestName, @IsATest);
END

-- Questions

DECLARE @Q1Id INT =
(
  SELECT TOP 1 q.QuestionId
  FROM Questions q
  WHERE q.SnapshotId=@Snap AND q.TestId=@TestId AND q.QuestionText=@Q1Text
);

-- Q1
IF @Q1Id IS NULL
BEGIN
  SELECT @Q1Id = ISNULL(MAX(QuestionId), 0) + 1 FROM Questions;
  INSERT INTO Questions (SnapshotId, QuestionId, QuestionText, Score, IsRelevant, TestId)
  VALUES (@Snap, @Q1Id, @Q1Text, NULL, 1, @TestId);
END
IF NOT EXISTS (
  SELECT 1 FROM ZonesQuestions
  WHERE SnapshotId=@Snap AND ZoneId=@Z1Id AND QuestionId=@Q1Id
)
  INSERT INTO ZonesQuestions (SnapshotId, ZoneId, QuestionId)
  VALUES (@Snap, @Z1Id, @Q1Id);

-- Q2
DECLARE @Q2Id INT =
(
  SELECT TOP 1 q.QuestionId
  FROM Questions q
  WHERE q.SnapshotId=@Snap AND q.TestId=@TestId AND q.QuestionText=@Q2Text
);
IF @Q2Id IS NULL
BEGIN
  SELECT @Q2Id = ISNULL(MAX(QuestionId), 0) + 1 FROM Questions;
  INSERT INTO Questions (SnapshotId, QuestionId, QuestionText, Score, IsRelevant, TestId)
  VALUES (@Snap, @Q2Id, @Q2Text, NULL, 1, @TestId);
END
IF NOT EXISTS (
  SELECT 1 FROM ZonesQuestions
  WHERE SnapshotId=@Snap AND ZoneId=@Z1Id AND QuestionId=@Q2Id
)
  INSERT INTO ZonesQuestions (SnapshotId, ZoneId, QuestionId)
  VALUES (@Snap, @Z1Id, @Q2Id);

-- Q3
DECLARE @Q3Id INT =
(
  SELECT TOP 1 q.QuestionId
  FROM Questions q
  WHERE q.SnapshotId=@Snap AND q.TestId=@TestId AND q.QuestionText=@Q3Text
);
IF @Q3Id IS NULL
BEGIN
  SELECT @Q3Id = ISNULL(MAX(QuestionId), 0) + 1 FROM Questions;
  INSERT INTO Questions (SnapshotId, QuestionId, QuestionText, Score, IsRelevant, TestId)
  VALUES (@Snap, @Q3Id, @Q3Text, NULL, 1, @TestId);
END
IF NOT EXISTS (
  SELECT 1 FROM ZonesQuestions
  WHERE SnapshotId=@Snap AND ZoneId=@Z3Id AND QuestionId=@Q3Id
)
  INSERT INTO ZonesQuestions (SnapshotId, ZoneId, QuestionId)
  VALUES (@Snap, @Z3Id, @Q3Id);

-- Q4
DECLARE @Q4Id INT =
(
  SELECT TOP 1 q.QuestionId
  FROM Questions q
  WHERE q.SnapshotId=@Snap AND q.TestId=@TestId AND q.QuestionText=@Q4Text
);
IF @Q4Id IS NULL
BEGIN
  SELECT @Q4Id = ISNULL(MAX(QuestionId), 0) + 1 FROM Questions;
  INSERT INTO Questions (SnapshotId, QuestionId, QuestionText, Score, IsRelevant, TestId)
  VALUES (@Snap, @Q4Id, @Q4Text, NULL, 1, @TestId);
END

-- ZonesQuestions
IF NOT EXISTS (
  SELECT 1 FROM ZonesQuestions
  WHERE SnapshotId=@Snap AND ZoneId=@Z2Id AND QuestionId=@Q4Id
)
  INSERT INTO ZonesQuestions (SnapshotId, ZoneId, QuestionId)
  VALUES (@Snap, @Z2Id, @Q4Id);

COMMIT;
