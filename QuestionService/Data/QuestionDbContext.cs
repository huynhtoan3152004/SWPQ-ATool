using Microsoft.EntityFrameworkCore;
using QuestionService.Entities;

namespace QuestionService.Data;

public class QuestionDbContext : DbContext
{
    public QuestionDbContext(DbContextOptions<QuestionDbContext> options) : base(options)
    {
    }

    public DbSet<Question> Questions => Set<Question>();
    public DbSet<Semester> Semesters => Set<Semester>();
    public DbSet<Topic> Topics => Set<Topic>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Title).IsRequired().HasMaxLength(300);
            entity.Property(x => x.Content).IsRequired();
            entity.Property(x => x.AskedBy).IsRequired();
            entity.Property(x => x.TopicId).IsRequired();
            entity.Property(x => x.SemesterId).IsRequired();
            entity.Property(x => x.Visibility).HasConversion<string>().IsRequired();
            entity.Property(x => x.Status).HasConversion<string>().IsRequired();
            entity.Property(x => x.CreatedAt).IsRequired();
            entity.HasIndex(x => new { x.TopicId, x.SemesterId, x.Status });
            entity.HasIndex(x => new { x.TopicId, x.SemesterId, x.Visibility });
        });

        modelBuilder.Entity<Semester>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).IsRequired().HasMaxLength(100);
            entity.Property(x => x.Year).IsRequired();
            entity.Property(x => x.Month).IsRequired();
            entity.Property(x => x.CreatedAt).IsRequired();
            entity.HasIndex(x => new { x.Year, x.Month, x.Name }).IsUnique();
        });

        modelBuilder.Entity<Topic>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Code).IsRequired().HasMaxLength(50);
            entity.Property(x => x.Name).IsRequired().HasMaxLength(200);
            entity.Property(x => x.NameEn).IsRequired().HasMaxLength(300);
            entity.Property(x => x.NameVn).IsRequired().HasMaxLength(500);
            entity.Property(x => x.SubmittedBy).IsRequired().HasMaxLength(100);
            entity.Property(x => x.ResponsibleBy).IsRequired().HasMaxLength(100);
            entity.Property(x => x.Context).IsRequired();
            entity.Property(x => x.Problems).IsRequired();
            entity.Property(x => x.Actors).IsRequired();
            entity.Property(x => x.FunctionalRequirements).IsRequired();
            entity.Property(x => x.References).HasMaxLength(2000);
            entity.Property(x => x.SemesterId).IsRequired();
            entity.Property(x => x.LecturerId).IsRequired();
            entity.Property(x => x.CreatedAt).IsRequired();
            entity.HasIndex(x => x.Code).IsUnique();
            entity.HasIndex(x => new { x.SemesterId, x.Name }).IsUnique();
            entity.HasIndex(x => x.LecturerId);

            entity.HasOne<Semester>()
                .WithMany()
                .HasForeignKey(x => x.SemesterId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
