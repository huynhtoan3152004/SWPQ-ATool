using Microsoft.EntityFrameworkCore;
using QuestionService.Entities;

namespace QuestionService.Data;

public static class QuestionSeed
{
    public static async Task SeedAsync(QuestionDbContext dbContext)
    {
        await dbContext.Database.EnsureCreatedAsync();

        await dbContext.Database.ExecuteSqlRawAsync(@"
            CREATE TABLE IF NOT EXISTS ""Semesters"" (
                ""Id"" uuid NOT NULL,
                ""Name"" character varying(100) NOT NULL,
                ""Year"" integer NOT NULL,
                ""Month"" integer NOT NULL,
                ""CreatedAt"" timestamp with time zone NOT NULL,
                CONSTRAINT ""PK_Semesters"" PRIMARY KEY (""Id"")
            );
        ");

        await dbContext.Database.ExecuteSqlRawAsync(@"
            CREATE UNIQUE INDEX IF NOT EXISTS ""IX_Semesters_Year_Month_Name""
            ON ""Semesters"" (""Year"", ""Month"", ""Name"");
        ");

        await dbContext.Database.ExecuteSqlRawAsync(@"
            CREATE TABLE IF NOT EXISTS ""Topics"" (
                ""Id"" uuid NOT NULL,
                ""Name"" character varying(200) NOT NULL,
                ""SemesterId"" uuid NOT NULL,
                ""LecturerId"" uuid NOT NULL,
                ""CreatedAt"" timestamp with time zone NOT NULL,
                CONSTRAINT ""PK_Topics"" PRIMARY KEY (""Id""),
                CONSTRAINT ""FK_Topics_Semesters_SemesterId"" FOREIGN KEY (""SemesterId"") REFERENCES ""Semesters"" (""Id"") ON DELETE RESTRICT
            );
        ");

        var defaultLecturerId = Guid.Parse("77777777-7777-7777-7777-777777777777");

        await dbContext.Database.ExecuteSqlRawAsync(@"
            ALTER TABLE ""Topics"" ADD COLUMN IF NOT EXISTS ""LecturerId"" uuid;
        ");

        await dbContext.Database.ExecuteSqlInterpolatedAsync($@"
            UPDATE ""Topics""
            SET ""LecturerId"" = {defaultLecturerId}
            WHERE ""LecturerId"" IS NULL;
        ");

        await dbContext.Database.ExecuteSqlRawAsync(@"
            ALTER TABLE ""Topics"" ALTER COLUMN ""LecturerId"" SET NOT NULL;
        ");

        await dbContext.Database.ExecuteSqlRawAsync(@"
            CREATE UNIQUE INDEX IF NOT EXISTS ""IX_Topics_SemesterId_Name""
            ON ""Topics"" (""SemesterId"", ""Name"");
        ");

        await dbContext.Database.ExecuteSqlRawAsync(@"
            CREATE INDEX IF NOT EXISTS ""IX_Topics_LecturerId""
            ON ""Topics"" (""LecturerId"");
        ");

        var semesterSpring2026Id = Guid.Parse("33333333-3333-3333-3333-333333333333");
        var semesterFall2026Id = Guid.Parse("44444444-4444-4444-4444-444444444444");
        var topicSwpArchitectureId = Guid.Parse("22222222-2222-2222-2222-222222222222");
        var topicCloudDeploymentId = Guid.Parse("55555555-5555-5555-5555-555555555555");

        if (!await dbContext.Semesters.AnyAsync())
        {
            await dbContext.Semesters.AddRangeAsync(
                new Semester
                {
                    Id = semesterSpring2026Id,
                    Name = "SPRING",
                    Year = 2026,
                    Month = 3,
                    CreatedAt = DateTime.UtcNow
                },
                new Semester
                {
                    Id = semesterFall2026Id,
                    Name = "FALL",
                    Year = 2026,
                    Month = 9,
                    CreatedAt = DateTime.UtcNow
                });

            await dbContext.SaveChangesAsync();
        }

        if (!await dbContext.Topics.AnyAsync())
        {
            await dbContext.Topics.AddRangeAsync(
                new Topic
                {
                    Id = topicSwpArchitectureId,
                    Name = "SWP Architecture",
                    SemesterId = semesterSpring2026Id,
                    LecturerId = defaultLecturerId,
                    CreatedAt = DateTime.UtcNow
                },
                new Topic
                {
                    Id = topicCloudDeploymentId,
                    Name = "Cloud Deployment",
                    SemesterId = semesterFall2026Id,
                    LecturerId = defaultLecturerId,
                    CreatedAt = DateTime.UtcNow
                });

            await dbContext.SaveChangesAsync();
        }

        if (await dbContext.Questions.AnyAsync())
        {
            return;
        }

        var sampleQuestion = new Question
        {
            Title = "SWP topic clarification",
            Content = "Cho em hỏi scope của topic kỳ này cần triển khai đến mức nào?",
            TopicId = topicSwpArchitectureId,
            SemesterId = semesterSpring2026Id,
            AskedBy = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Visibility = QuestionVisibility.PUBLIC,
            Status = QuestionStatus.PENDING,
            CreatedAt = DateTime.UtcNow
        };

        await dbContext.Questions.AddAsync(sampleQuestion);
        await dbContext.SaveChangesAsync();
    }
}
