# GradesService

This service runs a snapshot-based grading system. It models subjects, their zones (sub-topics), the questions that assess them, and the tests those questions belong to. Each student assessment creates a new snapshot (snapshot 0 is the catalog/template), duplicating the links so results can be tracked over time.

## What this service contains?
### SQL Assets

- simple_migration_script.sql: static data migration.

- data_generator.sql: data generator procedure.

- calculate_score_per_snapshot.sql: scoring procedure (per subject, national vs non-national).

### Backend (ASP.NET Core + EF Core)

- Questions CRUD (snapshot-scoped) with pagination and validation.

- Student Report endpoint (single snapshot).

- Principal Report endpoint (multi-snapshot).

### Schema layout
![Grades DB schema](./docs/db_schemas.png)

## Getting Started


## Implementation Notes

### Tech Stack

### SQL Scripts
1. static migration:
2. procedure
3. procdure

### BackEnd
