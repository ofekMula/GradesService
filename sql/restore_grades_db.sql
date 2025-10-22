RESTORE FILELISTONLY
FROM DISK = N'/var/opt/mssql/backup/GRADES.bak';

RESTORE DATABASE [GradesDB]
FROM DISK = N'/var/opt/mssql/backup/GRADES.bak'
WITH MOVE N'Grades'     TO N'/var/opt/mssql/restore/GradesDB_data.mdf',
     MOVE N'Grades_log' TO N'/var/opt/mssql/restore/GradesDB_log.ldf',
     REPLACE, RECOVERY, STATS = 10;
