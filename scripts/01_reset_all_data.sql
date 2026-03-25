-- Run with psql as superuser or owner, for example:
-- psql -U postgres -f scripts/01_reset_all_data.sql

-- ==========================================
-- 1) Reset Answer DB
-- ==========================================
\connect swp_answer_db;
BEGIN;
TRUNCATE TABLE public."Answers" RESTART IDENTITY CASCADE;
COMMIT;

-- ==========================================
-- 2) Reset Question DB
-- ==========================================
\connect swp_question_db;
BEGIN;
TRUNCATE TABLE public."Questions", public."Topics", public."Semesters" RESTART IDENTITY CASCADE;
COMMIT;

-- ==========================================
-- 3) Reset Auth DB
-- ==========================================
\connect swp_auth_db;
BEGIN;
TRUNCATE TABLE public."Users" RESTART IDENTITY CASCADE;
COMMIT;

-- Quick verify (Auth DB)
SELECT COUNT(*) AS "UsersCount" FROM public."Users";
