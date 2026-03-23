using Microsoft.EntityFrameworkCore;
using QuestionService.Entities;

namespace QuestionService.Data;

public class QuestionDbContext : DbContext
{
    public QuestionDbContext(DbContextOptions<QuestionDbContext> options) : base(options)
    {
    }

    public DbSet<Question> Questions => Set<Question>();

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
    }
}
