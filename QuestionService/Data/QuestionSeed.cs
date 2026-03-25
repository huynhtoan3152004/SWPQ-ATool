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
                ""Code"" character varying(50) NOT NULL,
                ""Name"" character varying(200) NOT NULL,
                ""NameEn"" character varying(300) NOT NULL,
                ""NameVn"" character varying(500) NOT NULL,
                ""SubmittedBy"" character varying(100) NOT NULL,
                ""ResponsibleBy"" character varying(100) NOT NULL,
                ""Context"" text NOT NULL,
                ""Problems"" text NOT NULL,
                ""Actors"" text NOT NULL,
                ""FunctionalRequirements"" text NOT NULL,
                ""References"" character varying(2000),
                ""SemesterId"" uuid NOT NULL,
                ""LecturerId"" uuid NOT NULL,
                ""CreatedAt"" timestamp with time zone NOT NULL,
                CONSTRAINT ""PK_Topics"" PRIMARY KEY (""Id""),
                CONSTRAINT ""FK_Topics_Semesters_SemesterId"" FOREIGN KEY (""SemesterId"") REFERENCES ""Semesters"" (""Id"") ON DELETE RESTRICT
            );
        ");

        await dbContext.Database.ExecuteSqlRawAsync(@"
            ALTER TABLE ""Topics"" ADD COLUMN IF NOT EXISTS ""LecturerId"" uuid;
        ");

        await dbContext.Database.ExecuteSqlRawAsync(@"
            ALTER TABLE ""Topics"" ADD COLUMN IF NOT EXISTS ""Code"" character varying(50);
        ");

        await dbContext.Database.ExecuteSqlRawAsync(@"
            ALTER TABLE ""Topics"" ADD COLUMN IF NOT EXISTS ""NameEn"" character varying(300);
        ");

        await dbContext.Database.ExecuteSqlRawAsync(@"
            ALTER TABLE ""Topics"" ADD COLUMN IF NOT EXISTS ""NameVn"" character varying(500);
        ");

        await dbContext.Database.ExecuteSqlRawAsync(@"
            ALTER TABLE ""Topics"" ADD COLUMN IF NOT EXISTS ""SubmittedBy"" character varying(100);
        ");

        await dbContext.Database.ExecuteSqlRawAsync(@"
            ALTER TABLE ""Topics"" ADD COLUMN IF NOT EXISTS ""ResponsibleBy"" character varying(100);
        ");

        await dbContext.Database.ExecuteSqlRawAsync(@"
            ALTER TABLE ""Topics"" ADD COLUMN IF NOT EXISTS ""Context"" text;
        ");

        await dbContext.Database.ExecuteSqlRawAsync(@"
            ALTER TABLE ""Topics"" ADD COLUMN IF NOT EXISTS ""Problems"" text;
        ");

        await dbContext.Database.ExecuteSqlRawAsync(@"
            ALTER TABLE ""Topics"" ADD COLUMN IF NOT EXISTS ""Actors"" text;
        ");

        await dbContext.Database.ExecuteSqlRawAsync(@"
            ALTER TABLE ""Topics"" ADD COLUMN IF NOT EXISTS ""FunctionalRequirements"" text;
        ");

        await dbContext.Database.ExecuteSqlRawAsync(@"
            ALTER TABLE ""Topics"" ADD COLUMN IF NOT EXISTS ""References"" character varying(2000);
        ");

        await dbContext.Database.ExecuteSqlRawAsync(@"
            UPDATE ""Topics""
            SET
                ""Code"" = COALESCE(NULLIF(trim(""Code""), ''), 'TOPIC-' || substring(""Id""::text, 1, 8)),
                ""NameEn"" = COALESCE(""NameEn"", ""Name""),
                ""NameVn"" = COALESCE(""NameVn"", ""Name""),
                ""SubmittedBy"" = COALESCE(""SubmittedBy"", 'system'),
                ""ResponsibleBy"" = COALESCE(""ResponsibleBy"", 'system'),
                ""Context"" = COALESCE(""Context"", 'N/A'),
                ""Problems"" = COALESCE(""Problems"", 'N/A'),
                ""Actors"" = COALESCE(""Actors"", 'N/A'),
                ""FunctionalRequirements"" = COALESCE(""FunctionalRequirements"", 'N/A');
        ");

        await dbContext.Database.ExecuteSqlRawAsync(@"
            UPDATE ""Topics""
            SET ""Code"" = upper(trim(""Code""));
        ");

        await dbContext.Database.ExecuteSqlRawAsync(@"
            WITH ranked AS (
                SELECT
                    ""Id"",
                    ""Code"",
                    row_number() OVER (PARTITION BY ""Code"" ORDER BY ""CreatedAt"", ""Id"") AS rn
                FROM ""Topics""
            )
            UPDATE ""Topics"" t
            SET ""Code"" = left(r.""Code"", 41) || '-' || substring(t.""Id""::text, 1, 8)
            FROM ranked r
            WHERE t.""Id"" = r.""Id""
              AND r.rn > 1;
        ");

        await dbContext.Database.ExecuteSqlRawAsync(@"
            ALTER TABLE ""Topics"" ALTER COLUMN ""Code"" SET NOT NULL;
        ");

        await dbContext.Database.ExecuteSqlRawAsync(@"
            ALTER TABLE ""Topics"" ALTER COLUMN ""NameEn"" SET NOT NULL;
        ");

        await dbContext.Database.ExecuteSqlRawAsync(@"
            ALTER TABLE ""Topics"" ALTER COLUMN ""NameVn"" SET NOT NULL;
        ");

        await dbContext.Database.ExecuteSqlRawAsync(@"
            ALTER TABLE ""Topics"" ALTER COLUMN ""SubmittedBy"" SET NOT NULL;
        ");

        await dbContext.Database.ExecuteSqlRawAsync(@"
            ALTER TABLE ""Topics"" ALTER COLUMN ""ResponsibleBy"" SET NOT NULL;
        ");

        await dbContext.Database.ExecuteSqlRawAsync(@"
            ALTER TABLE ""Topics"" ALTER COLUMN ""Context"" SET NOT NULL;
        ");

        await dbContext.Database.ExecuteSqlRawAsync(@"
            ALTER TABLE ""Topics"" ALTER COLUMN ""Problems"" SET NOT NULL;
        ");

        await dbContext.Database.ExecuteSqlRawAsync(@"
            ALTER TABLE ""Topics"" ALTER COLUMN ""Actors"" SET NOT NULL;
        ");

        await dbContext.Database.ExecuteSqlRawAsync(@"
            ALTER TABLE ""Topics"" ALTER COLUMN ""FunctionalRequirements"" SET NOT NULL;
        ");

        await dbContext.Database.ExecuteSqlRawAsync(@"
            DO $$
            BEGIN
                IF NOT EXISTS (SELECT 1 FROM ""Topics"" WHERE ""LecturerId"" IS NULL) THEN
                    ALTER TABLE ""Topics"" ALTER COLUMN ""LecturerId"" SET NOT NULL;
                END IF;
            END $$;
        ");

        await dbContext.Database.ExecuteSqlRawAsync(@"
            DO $$
            BEGIN
                IF EXISTS (
                    SELECT 1
                    FROM ""Topics""
                    GROUP BY ""Code""
                    HAVING COUNT(*) > 1
                ) THEN
                    CREATE INDEX IF NOT EXISTS ""IX_Topics_Code_NonUnique""
                    ON ""Topics"" (""Code"");
                ELSE
                    CREATE UNIQUE INDEX IF NOT EXISTS ""IX_Topics_Code""
                    ON ""Topics"" (""Code"");
                END IF;
            END $$;
        ");

        await dbContext.Database.ExecuteSqlRawAsync(@"
            CREATE UNIQUE INDEX IF NOT EXISTS ""IX_Topics_SemesterId_Name""
            ON ""Topics"" (""SemesterId"", ""Name"");
        ");

        await dbContext.Database.ExecuteSqlRawAsync(@"
            CREATE INDEX IF NOT EXISTS ""IX_Topics_LecturerId""
            ON ""Topics"" (""LecturerId"");
        ");

        await dbContext.Database.ExecuteSqlRawAsync(@"
            ALTER TABLE ""Questions"" ADD COLUMN IF NOT EXISTS ""AskedBy"" uuid;
        ");

        await dbContext.Database.ExecuteSqlRawAsync(@"
            ALTER TABLE ""Questions"" ADD COLUMN IF NOT EXISTS ""TopicId"" uuid;
        ");

        await dbContext.Database.ExecuteSqlRawAsync(@"
            ALTER TABLE ""Questions"" ADD COLUMN IF NOT EXISTS ""SemesterId"" uuid;
        ");

        await dbContext.Database.ExecuteSqlRawAsync(@"
            ALTER TABLE ""Questions"" ADD COLUMN IF NOT EXISTS ""Visibility"" text;
        ");

        await dbContext.Database.ExecuteSqlRawAsync(@"
            UPDATE ""Questions""
            SET ""AskedBy"" = ""StudentId""
            WHERE ""AskedBy"" IS NULL;
        ");

        await dbContext.Database.ExecuteSqlRawAsync(@"
            UPDATE ""Questions"" q
            SET ""TopicId"" = t.""Id""
            FROM ""Topics"" t
            WHERE q.""TopicId"" IS NULL
              AND lower(q.""Topic"") = lower(t.""Name"");
        ");

        await dbContext.Database.ExecuteSqlRawAsync(@"
            UPDATE ""Questions"" q
            SET ""SemesterId"" = t.""SemesterId""
            FROM ""Topics"" t
            WHERE q.""SemesterId"" IS NULL
              AND q.""TopicId"" = t.""Id"";
        ");

        await dbContext.Database.ExecuteSqlRawAsync(@"
            UPDATE ""Questions""
            SET
                ""TopicId"" = COALESCE(""TopicId"", (SELECT ""Id"" FROM ""Topics"" ORDER BY ""CreatedAt"" LIMIT 1)),
                ""SemesterId"" = COALESCE(""SemesterId"", (SELECT ""Id"" FROM ""Semesters"" ORDER BY ""CreatedAt"" LIMIT 1)),
                ""Visibility"" = COALESCE(""Visibility"", 'PUBLIC'),
                ""AskedBy"" = COALESCE(""AskedBy"", ""StudentId"")
            WHERE ""TopicId"" IS NULL
               OR ""SemesterId"" IS NULL
               OR ""Visibility"" IS NULL
               OR ""AskedBy"" IS NULL;
        ");

        await dbContext.Database.ExecuteSqlRawAsync(@"
            DO $$
            BEGIN
                IF NOT EXISTS (SELECT 1 FROM ""Questions"" WHERE ""TopicId"" IS NULL) THEN
                    ALTER TABLE ""Questions"" ALTER COLUMN ""TopicId"" SET NOT NULL;
                END IF;

                IF NOT EXISTS (SELECT 1 FROM ""Questions"" WHERE ""SemesterId"" IS NULL) THEN
                    ALTER TABLE ""Questions"" ALTER COLUMN ""SemesterId"" SET NOT NULL;
                END IF;

                IF NOT EXISTS (SELECT 1 FROM ""Questions"" WHERE ""Visibility"" IS NULL) THEN
                    ALTER TABLE ""Questions"" ALTER COLUMN ""Visibility"" SET NOT NULL;
                END IF;

                IF NOT EXISTS (SELECT 1 FROM ""Questions"" WHERE ""AskedBy"" IS NULL) THEN
                    ALTER TABLE ""Questions"" ALTER COLUMN ""AskedBy"" SET NOT NULL;
                END IF;
            END $$;
        ");

        await dbContext.Database.ExecuteSqlRawAsync(@"
            DROP INDEX IF EXISTS ""IX_Questions_Topic_Status"";
        ");

        await dbContext.Database.ExecuteSqlRawAsync(@"
            CREATE INDEX IF NOT EXISTS ""IX_Questions_TopicId_SemesterId_Status""
            ON ""Questions"" (""TopicId"", ""SemesterId"", ""Status"");
        ");

        await dbContext.Database.ExecuteSqlRawAsync(@"
            CREATE INDEX IF NOT EXISTS ""IX_Questions_TopicId_SemesterId_Visibility""
            ON ""Questions"" (""TopicId"", ""SemesterId"", ""Visibility"");
        ");

        return;
    }
}
