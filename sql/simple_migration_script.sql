SET XACT_ABORT ON;
BEGIN TRAN;

DECLARE @SnapshotId INT = 0;

DECLARE @SubjectName NVARCHAR(500) = N'Computer Science';
DECLARE @TestName NVARCHAR(1000) = N'Catalog Test - Computer Science';

DECLARE @IsATest BIT = 1;
DECLARE @IsRelevant BIT = 1;

DECLARE @Zone1 NVARCHAR(500) = N'Data Structures';
DECLARE @Zone2 NVARCHAR(500) = N'Algorithms';
DECLARE @Zone3 NVARCHAR(500) = N'Databases';

DECLARE @Q1Text NVARCHAR(1000) = N'What is the time complexity of accessing an array element by index?';

DECLARE @Q2Text NVARCHAR(1000) = N'Which data structure uses FIFO semantics?';

DECLARE @Q3Text NVARCHAR(1000) = N'In SQL, what is a primary key?';

DECLARE @Q4Text NVARCHAR(1000) = N'What is the time complexity of binary search on a sorted array?';

DECLARE @SubjectId  INT = (SELECT ISNULL(MAX(SubjectId), 0) + 1 FROM Subjects);
DECLARE @TestId     INT = (SELECT ISNULL(MAX(TestId), 0) + 1 FROM Tests);
DECLARE @ZoneBase   INT = (SELECT ISNULL(MAX(ZoneId), 0) FROM Zones);
DECLARE @QBase      INT = (SELECT ISNULL(MAX(QuestionId), 0) FROM Questions);

DECLARE @Z1Id INT = @ZoneBase + 1;
DECLARE @Z2Id INT = @ZoneBase + 2;
DECLARE @Z3Id INT = @ZoneBase + 3;

DECLARE @Q1Id INT = @QBase + 1;
DECLARE @Q2Id INT = @QBase + 2;
DECLARE @Q3Id INT = @QBase + 3;
DECLARE @Q4Id INT = @QBase + 4;

INSERT INTO Subjects (SnapshotId, SubjectId, SubjectName)
VALUES (@SnapshotId, @SubjectId, @SubjectName);

INSERT INTO Zones (SnapshotId, ZoneId, ZoneName, IsRelevant)
VALUES
  (@SnapshotId, @Z1Id, @Zone1, @IsRelevant),
  (@SnapshotId, @Z2Id, @Zone2, @IsRelevant),
  (@SnapshotId, @Z3Id, @Zone3, @IsRelevant);

INSERT INTO SubjectZones (SnapshotId, SubjectId, ZoneId)
VALUES
  (@SnapshotId, @SubjectId, @Z1Id),
  (@SnapshotId, @SubjectId, @Z2Id),
  (@SnapshotId, @SubjectId, @Z3Id);

INSERT INTO Tests (TestId, TestName, IsATest)
VALUES (@TestId, @TestName, @IsATest);

INSERT INTO Questions (SnapshotId, QuestionId, QuestionText, Score, IsRelevant, TestId)
VALUES
  (@SnapshotId, @Q1Id, @Q1Text, NULL, @IsRelevant, @TestId),
  (@SnapshotId, @Q2Id, @Q2Text, NULL, @IsRelevant, @TestId),
  (@SnapshotId, @Q3Id, @Q3Text, NULL, @IsRelevant, @TestId),
  (@SnapshotId, @Q4Id, @Q4Text, NULL, @IsRelevant, @TestId);

INSERT INTO ZonesQuestions (SnapshotId, ZoneId, QuestionId)
VALUES
  (@SnapshotId, @Z1Id, @Q1Id),
  (@SnapshotId, @Z1Id, @Q2Id),
  (@SnapshotId, @Z3Id, @Q3Id),
  (@SnapshotId, @Z2Id, @Q4Id);

COMMIT;
