using Microsoft.EntityFrameworkCore;
using QuestionService.Entities;

namespace QuestionService.Data;

public static class QuestionSeed
{
    public static async Task SeedAsync(QuestionDbContext dbContext)
    {
        await dbContext.Database.EnsureCreatedAsync();

        if (await dbContext.Questions.AnyAsync())
        {
            return;
        }

        var sampleQuestion = new Question
        {
            Title = "SWP topic clarification",
            Content = "Cho em hỏi scope của topic kỳ này cần triển khai đến mức nào?",
            Topic = "SWP-Sprint-1",
            StudentId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Status = QuestionStatus.PENDING,
            CreatedAt = DateTime.UtcNow
        };

        await dbContext.Questions.AddAsync(sampleQuestion);
        await dbContext.SaveChangesAsync();
    }
}
