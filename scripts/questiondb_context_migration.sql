BEGIN;

-- 1) Rename old field to new semantic name
ALTER TABLE public."Questions"
    RENAME COLUMN "StudentId" TO "AskedBy";

-- 2) Add new context fields
ALTER TABLE public."Questions"
    ADD COLUMN IF NOT EXISTS "TopicId" uuid,
    ADD COLUMN IF NOT EXISTS "SemesterId" uuid,
    ADD COLUMN IF NOT EXISTS "Visibility" text;

-- 3) Backfill existing rows (replace with your real ids if needed)
UPDATE public."Questions"
SET
    "TopicId" = COALESCE("TopicId", '22222222-2222-2222-2222-222222222222'::uuid),
    "SemesterId" = COALESCE("SemesterId", '33333333-3333-3333-3333-333333333333'::uuid),
    "Visibility" = COALESCE("Visibility", 'PUBLIC');

-- 4) Enforce not-null and default
ALTER TABLE public."Questions"
    ALTER COLUMN "TopicId" SET NOT NULL,
    ALTER COLUMN "SemesterId" SET NOT NULL,
    ALTER COLUMN "Visibility" SET NOT NULL,
    ALTER COLUMN "Visibility" SET DEFAULT 'PUBLIC';

-- 5) Recreate indexes for new query pattern
DROP INDEX IF EXISTS public."IX_Questions_Topic_Status";
DROP INDEX IF EXISTS public."IX_Questions_TopicId_SemesterId_Status";
DROP INDEX IF EXISTS public."IX_Questions_TopicId_SemesterId_Visibility";

CREATE INDEX "IX_Questions_TopicId_SemesterId_Status"
    ON public."Questions" ("TopicId", "SemesterId", "Status");

CREATE INDEX "IX_Questions_TopicId_SemesterId_Visibility"
    ON public."Questions" ("TopicId", "SemesterId", "Visibility");

COMMIT;
