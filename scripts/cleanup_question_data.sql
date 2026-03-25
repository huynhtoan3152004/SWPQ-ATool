-- QuestionService data cleanup script
-- Use with PostgreSQL (psql/pgAdmin).
--
-- Recommendation: backup first
--   pg_dump -U postgres -d QuestionDb > questiondb_backup_before_cleanup.sql

BEGIN;

-- ============================================================
-- 0) Pre-check overview (run first)
-- ============================================================
-- Total rows
SELECT 'Semesters' AS "Table", COUNT(*) AS "Rows" FROM public."Semesters"
UNION ALL
SELECT 'Topics' AS "Table", COUNT(*) AS "Rows" FROM public."Topics"
UNION ALL
SELECT 'Questions' AS "Table", COUNT(*) AS "Rows" FROM public."Questions";

-- Questions with empty title/content
SELECT COUNT(*) AS "InvalidQuestions_EmptyTitleOrContent"
FROM public."Questions"
WHERE btrim(COALESCE("Title", '')) = ''
   OR btrim(COALESCE("Content", '')) = '';

-- Orphan questions (missing topic/semester)
SELECT COUNT(*) AS "InvalidQuestions_Orphan"
FROM public."Questions" q
LEFT JOIN public."Topics" t ON t."Id" = q."TopicId"
LEFT JOIN public."Semesters" s ON s."Id" = q."SemesterId"
WHERE t."Id" IS NULL OR s."Id" IS NULL;

-- Duplicate topic code (case-insensitive, trimmed)
SELECT upper(trim("Code")) AS "NormalizedCode", COUNT(*) AS "Count"
FROM public."Topics"
GROUP BY upper(trim("Code"))
HAVING COUNT(*) > 1
ORDER BY COUNT(*) DESC, upper(trim("Code"));

-- ============================================================
-- MODE A: CLEANUP ONLY (delete invalid records, keep valid data)
-- Uncomment this block if you want selective cleanup.
-- ============================================================
/*
-- 1) Remove empty questions
DELETE FROM public."Questions"
WHERE btrim(COALESCE("Title", '')) = ''
   OR btrim(COALESCE("Content", '')) = '';

-- 2) Remove orphan questions
DELETE FROM public."Questions" q
WHERE NOT EXISTS (SELECT 1 FROM public."Topics" t WHERE t."Id" = q."TopicId")
   OR NOT EXISTS (SELECT 1 FROM public."Semesters" s WHERE s."Id" = q."SemesterId");

-- 3) Normalize topic code
UPDATE public."Topics"
SET "Code" = upper(trim("Code"))
WHERE "Code" IS NOT NULL;

-- 4) Remove duplicate topics by Code (keep oldest by CreatedAt, then Id)
--    and remove related questions first to avoid FK conflict.
WITH ranked AS (
    SELECT
        "Id",
        upper(trim("Code")) AS normalized_code,
        row_number() OVER (
            PARTITION BY upper(trim("Code"))
            ORDER BY "CreatedAt", "Id"
        ) AS rn
    FROM public."Topics"
), duplicated AS (
    SELECT "Id"
    FROM ranked
    WHERE rn > 1
)
DELETE FROM public."Questions" q
USING duplicated d
WHERE q."TopicId" = d."Id";

WITH ranked AS (
    SELECT
        "Id",
        upper(trim("Code")) AS normalized_code,
        row_number() OVER (
            PARTITION BY upper(trim("Code"))
            ORDER BY "CreatedAt", "Id"
        ) AS rn
    FROM public."Topics"
)
DELETE FROM public."Topics" t
USING ranked r
WHERE t."Id" = r."Id"
  AND r.rn > 1;
*/

-- ============================================================
-- MODE B: RESET ALL DATA (KEEP SCHEMA) - FASTEST WAY
-- Uncomment this single line if you want clean slate.
-- ============================================================
TRUNCATE TABLE public."Questions", public."Topics", public."Semesters" RESTART IDENTITY CASCADE;

-- ============================================================
-- 9) Post-check
-- ============================================================
SELECT 'Semesters' AS "Table", COUNT(*) AS "Rows" FROM public."Semesters"
UNION ALL
SELECT 'Topics' AS "Table", COUNT(*) AS "Rows" FROM public."Topics"
UNION ALL
SELECT 'Questions' AS "Table", COUNT(*) AS "Rows" FROM public."Questions";

COMMIT;
