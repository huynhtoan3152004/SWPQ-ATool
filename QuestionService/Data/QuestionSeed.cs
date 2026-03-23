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
            TopicId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            SemesterId = Guid.Parse("33333333-3333-3333-3333-333333333333"),
            AskedBy = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Visibility = QuestionVisibility.PUBLIC,
            Status = QuestionStatus.PENDING,
            CreatedAt = DateTime.UtcNow
        };

        await dbContext.Questions.AddAsync(sampleQuestion);
        await dbContext.SaveChangesAsync();
    }
}
