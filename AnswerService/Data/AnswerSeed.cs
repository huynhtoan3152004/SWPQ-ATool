namespace AnswerService.Data;

public static class AnswerSeed
{
    public static async Task SeedAsync(AnswerDbContext dbContext)
    {
        await dbContext.Database.EnsureCreatedAsync();
    }
}
