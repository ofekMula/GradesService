using Microsoft.EntityFrameworkCore;
using Grades.Domain.Models;

namespace Grades.Infrastructure.Persistence;

public class GradesDbContext(DbContextOptions<GradesDbContext> options) : DbContext(options)
{
    public DbSet<Question> Questions => Set<Question>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Question>(b =>
        {
            b.ToTable("Questions");
            b.HasKey(q => new { q.SnapshotId, q.QuestionId });   // composite PK in your DB
            b.Property(q => q.QuestionText).HasMaxLength(400).IsRequired();
            // other columns follow defaults
        });
    }
}
