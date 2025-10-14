using Grades.Application.Abstractions;
using Grades.Application.DTOs;
using Grades.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Infrastructure (EF + services)
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(); // /swagger
}

app.UseHttpsRedirection();

// Group: Questions
var group = app.MapGroup("/api/snapshots/{snapshotId:int}/questions").WithTags("Questions");

// GET all
group.MapGet("/", async (int snapshotId, IQuestionService svc, CancellationToken ct) =>
{
    var items = await svc.GetAllAsync(snapshotId, ct);
    return Results.Ok(items.Take(5));
})
.WithName("GetQuestionsBySnapshot")
.WithSummary("Get all questions for a snapshot");

// POST create
group.MapPost("/", async (int snapshotId, QuestionCreateDto dto, IQuestionService svc, CancellationToken ct) =>
{
    var created = await svc.CreateAsync(snapshotId, dto, ct);
    return created is null
        ? Results.BadRequest("Score cannot be negative.")
        : Results.Created($"/api/snapshots/{snapshotId}/questions/{created.QuestionId}", created);
})
.WithName("CreateQuestion")
.WithSummary("Create a new question for a snapshot");

// PUT update
group.MapPut("/{questionId:int}", async (int snapshotId, int questionId, QuestionUpdateDto dto, IQuestionService svc, CancellationToken ct) =>
{
    var ok = await svc.UpdateAsync(snapshotId, questionId, dto, ct);
    return ok ? Results.NoContent() : Results.NotFound();
})
.WithName("UpdateQuestion")
.WithSummary("Update question text, relevance, and score");

// DELETE remove
group.MapDelete("/{questionId:int}", async (int snapshotId, int questionId, IQuestionService svc, CancellationToken ct) =>
{
    var ok = await svc.DeleteAsync(snapshotId, questionId, ct);
    return ok ? Results.NoContent() : Results.NotFound();
})
.WithName("DeleteQuestion")
.WithSummary("Delete a question by snapshot");

app.Run();
