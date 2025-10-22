using Grades.Domain.Models;
using Microsoft.EntityFrameworkCore;

public class GradesDbContext(DbContextOptions<GradesDbContext> options) : DbContext(options)
{
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<Test> Tests=> Set<Test>();
    public DbSet<Subject> Subjects => Set<Subject>();
    public DbSet<SubjectZone> SubjectZones => Set<SubjectZone>();
    public DbSet<Zone> Zones=> Set<Zone>();
    public DbSet<ZonesQuestion> ZonesQuestions => Set<ZonesQuestion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Question>(b =>
        {
            b.ToTable("Questions");
            b.HasKey(q => new { q.SnapshotId, q.QuestionId });
            b.Property(q => q.QuestionText).HasMaxLength(400).IsRequired();
                    });

                    modelBuilder.Entity<Test>(b =>
                    {
            b.ToTable("Tests");
            b.HasKey(t => t.TestId);
            b.Property(t => t.TestName).HasMaxLength(200).IsRequired();
                    });

                    modelBuilder.Entity<Subject>(b =>
                    {
            b.ToTable("Subjects");
            b.HasKey(s => new { s.SnapshotId, s.SubjectId });
            b.Property(s => s.SubjectName).HasMaxLength(200).IsRequired();
                    });

                    modelBuilder.Entity<SubjectZone>(b =>
                    {
            b.ToTable("SubjectZones");
            b.HasKey(sz => new { sz.SnapshotId, sz.SubjectId, sz.ZoneId });

            b.HasOne<Subject>()
            .WithMany()
            .HasForeignKey(sz => new { sz.SnapshotId, sz.SubjectId })
            .OnDelete(DeleteBehavior.Cascade);

            b.HasOne<Zone>()
            .WithMany()
            .HasForeignKey(sz => new { sz.SnapshotId, sz.ZoneId })
            .OnDelete(DeleteBehavior.Cascade);
                    });

                    modelBuilder.Entity<Zone>(b =>
                    {
            b.ToTable("Zones");
            b.HasKey(z => new { z.SnapshotId, z.ZoneId });
            b.Property(z => z.ZoneName).HasMaxLength(200).IsRequired();
                    });

                    modelBuilder.Entity<ZonesQuestion>(b =>
                    {
            b.ToTable("ZonesQuestions");
            b.HasKey(zq => new { zq.SnapshotId, zq.ZoneId, zq.QuestionId });

            b.HasOne< Question >()
            .WithMany()
            .HasForeignKey(zq => new { zq.SnapshotId, zq.QuestionId })
            .OnDelete(DeleteBehavior.Cascade);

            b.HasOne< Zone >()
            .WithMany()
            .HasForeignKey(zq => new { zq.SnapshotId, zq.ZoneId })
            .OnDelete(DeleteBehavior.Cascade);
                    });
                }
            }
