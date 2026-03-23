using AnswerService.Entities;
using Microsoft.EntityFrameworkCore;

namespace AnswerService.Data;

public class AnswerDbContext : DbContext
{
    public AnswerDbContext(DbContextOptions<AnswerDbContext> options) : base(options)
    {
    }

    public DbSet<Answer> Answers => Set<Answer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Answer>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Content).IsRequired();
            entity.Property(x => x.CreatedAt).IsRequired();
            entity.HasIndex(x => x.QuestionId);
        });
    }
}
