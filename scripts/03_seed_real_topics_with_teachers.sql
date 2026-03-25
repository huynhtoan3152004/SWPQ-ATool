-- Run with defaults (swp_auth_db/swp_question_db):
-- psql -h localhost -p 5432 -U postgres -f scripts/03_seed_real_topics_with_teachers.sql
-- Run with custom db names:
-- psql -h localhost -p 5432 -U postgres -v AUTH_DB=authdb -v QUESTION_DB=questiondb -f scripts/03_seed_real_topics_with_teachers.sql

\if :{?AUTH_DB}
\else
\set AUTH_DB swp_auth_db
\endif

\if :{?QUESTION_DB}
\else
\set QUESTION_DB swp_question_db
\endif

\connect :AUTH_DB;
BEGIN;

ALTER TABLE public."Users" ADD COLUMN IF NOT EXISTS "FullName" character varying(150);
ALTER TABLE public."Users" ADD COLUMN IF NOT EXISTS "RequestedRole" text;

UPDATE public."Users"
SET "FullName" = COALESCE("FullName", split_part("Email", '@', 1));

ALTER TABLE public."Users" ALTER COLUMN "FullName" SET NOT NULL;

-- Default password for all teacher accounts: 123456
-- ASP.NET Identity hash compatible with current AuthService login flow
INSERT INTO public."Users" ("Id", "FullName", "Email", "PasswordHash", "Role", "RequestedRole")
VALUES
('20000000-0000-0000-0000-000000000001'::uuid, 'MinhNT01', 'minhnt01@swp.local', 'AQAAAAIAAYagAAAAEF6YsDoIQat1p541CurzCqeSZliXJd+gt/k0uCjjHf2LCHzXnXbWdUGHmWKFIFsT3Q==', 'TEACHER', NULL),
('20000000-0000-0000-0000-000000000002'::uuid, 'HieuNV02', 'hieunv02@swp.local', 'AQAAAAIAAYagAAAAEF6YsDoIQat1p541CurzCqeSZliXJd+gt/k0uCjjHf2LCHzXnXbWdUGHmWKFIFsT3Q==', 'TEACHER', NULL),
('20000000-0000-0000-0000-000000000003'::uuid, 'LinhTT03', 'linhtt03@swp.local', 'AQAAAAIAAYagAAAAEF6YsDoIQat1p541CurzCqeSZliXJd+gt/k0uCjjHf2LCHzXnXbWdUGHmWKFIFsT3Q==', 'TEACHER', NULL),
('20000000-0000-0000-0000-000000000004'::uuid, 'AnPV04', 'anpv04@swp.local', 'AQAAAAIAAYagAAAAEF6YsDoIQat1p541CurzCqeSZliXJd+gt/k0uCjjHf2LCHzXnXbWdUGHmWKFIFsT3Q==', 'TEACHER', NULL),
('20000000-0000-0000-0000-000000000005'::uuid, 'HaLT05', 'halt05@swp.local', 'AQAAAAIAAYagAAAAEF6YsDoIQat1p541CurzCqeSZliXJd+gt/k0uCjjHf2LCHzXnXbWdUGHmWKFIFsT3Q==', 'TEACHER', NULL),
('20000000-0000-0000-0000-000000000006'::uuid, 'NamDQ06', 'namdq06@swp.local', 'AQAAAAIAAYagAAAAEF6YsDoIQat1p541CurzCqeSZliXJd+gt/k0uCjjHf2LCHzXnXbWdUGHmWKFIFsT3Q==', 'TEACHER', NULL),
('20000000-0000-0000-0000-000000000007'::uuid, 'TrangBM07', 'trangbm07@swp.local', 'AQAAAAIAAYagAAAAEF6YsDoIQat1p541CurzCqeSZliXJd+gt/k0uCjjHf2LCHzXnXbWdUGHmWKFIFsT3Q==', 'TEACHER', NULL),
('20000000-0000-0000-0000-000000000008'::uuid, 'KhoaPD08', 'khoapd08@swp.local', 'AQAAAAIAAYagAAAAEF6YsDoIQat1p541CurzCqeSZliXJd+gt/k0uCjjHf2LCHzXnXbWdUGHmWKFIFsT3Q==', 'TEACHER', NULL),
('20000000-0000-0000-0000-000000000009'::uuid, 'VyNH09', 'vynh09@swp.local', 'AQAAAAIAAYagAAAAEF6YsDoIQat1p541CurzCqeSZliXJd+gt/k0uCjjHf2LCHzXnXbWdUGHmWKFIFsT3Q==', 'TEACHER', NULL),
('20000000-0000-0000-0000-000000000010'::uuid, 'LongTV10', 'longtv10@swp.local', 'AQAAAAIAAYagAAAAEF6YsDoIQat1p541CurzCqeSZliXJd+gt/k0uCjjHf2LCHzXnXbWdUGHmWKFIFsT3Q==', 'TEACHER', NULL)
ON CONFLICT ("Email")
DO UPDATE SET
    "FullName" = EXCLUDED."FullName",
    "PasswordHash" = EXCLUDED."PasswordHash",
    "Role" = 'TEACHER',
    "RequestedRole" = NULL;

COMMIT;

\connect :QUESTION_DB;
BEGIN;

-- Remove all old seeded/synthetic question-domain data
TRUNCATE TABLE public."Questions" RESTART IDENTITY CASCADE;
TRUNCATE TABLE public."Topics" RESTART IDENTITY CASCADE;
TRUNCATE TABLE public."Semesters" RESTART IDENTITY CASCADE;

-- One canonical semester for these topics
INSERT INTO public."Semesters" ("Id", "Name", "Year", "Month", "CreatedAt")
VALUES ('30000000-0000-0000-0000-000000000001'::uuid, 'SP26', 2026, 1, now())
ON CONFLICT ("Id") DO UPDATE
SET "Name" = EXCLUDED."Name",
    "Year" = EXCLUDED."Year",
    "Month" = EXCLUDED."Month";

INSERT INTO public."Topics"
("Id", "Code", "Name", "NameEn", "NameVn", "SubmittedBy", "ResponsibleBy", "Context", "Problems", "Actors", "FunctionalRequirements", "References", "SemesterId", "LecturerId", "CreatedAt")
VALUES
('40000000-0000-0000-0000-000000000008'::uuid, 'SP26SWP08', 'Smart Parking Management System', 'Smart Parking Management System', 'Hệ thống quản lý bãi đỗ xe thông minh', 'MinhNT01', 'MinhNT01', 'Hệ thống giúp quản lý bãi đỗ xe thông minh với khả năng theo dõi chỗ trống theo thời gian thực.', 'Thiếu thông tin chỗ trống, gây ùn tắc và mất thời gian tìm chỗ đậu xe.', 'Driver, Parking Staff, Admin', 'Driver tìm chỗ trống; Đặt chỗ trước; Thanh toán online; Staff quản lý bãi xe', NULL, '30000000-0000-0000-0000-000000000001'::uuid, '20000000-0000-0000-0000-000000000001'::uuid, now()),
('40000000-0000-0000-0000-000000000009'::uuid, 'SP26SWP09', 'Online Food Donation Platform', 'Online Food Donation Platform', 'Nền tảng quyên góp thực phẩm trực tuyến', 'HieuNV02', 'HieuNV02', 'Kết nối người có thực phẩm dư thừa với tổ chức từ thiện.', 'Lãng phí thực phẩm, thiếu kết nối giữa người cho và người nhận.', 'Donor, Charity, Admin', 'Đăng bài quyên góp; Nhận yêu cầu; Theo dõi phân phối', NULL, '30000000-0000-0000-0000-000000000001'::uuid, '20000000-0000-0000-0000-000000000002'::uuid, now()),
('40000000-0000-0000-0000-000000000010'::uuid, 'SP26SWP10', 'University Event Management System', 'University Event Management System', 'Hệ thống quản lý sự kiện đại học', 'LinhTT03', 'LinhTT03', 'Quản lý sự kiện trong trường đại học.', 'Thiếu hệ thống quản lý đăng ký và tổ chức sự kiện.', 'Student, Organizer, Admin', 'Tạo sự kiện; Đăng ký tham gia; Check-in QR', NULL, '30000000-0000-0000-0000-000000000001'::uuid, '20000000-0000-0000-0000-000000000003'::uuid, now()),
('40000000-0000-0000-0000-000000000011'::uuid, 'SP26SWP11', 'Online Tutor Matching Platform', 'Online Tutor Matching Platform', 'Nền tảng kết nối gia sư trực tuyến', 'AnPV04', 'AnPV04', 'Kết nối học sinh với gia sư phù hợp.', 'Khó tìm gia sư uy tín và phù hợp.', 'Student, Tutor, Admin', 'Tìm gia sư; Booking; Rating', NULL, '30000000-0000-0000-0000-000000000001'::uuid, '20000000-0000-0000-0000-000000000004'::uuid, now()),
('40000000-0000-0000-0000-000000000012'::uuid, 'SP26SWP12', 'Healthcare Appointment Booking System', 'Healthcare Appointment Booking System', 'Hệ thống đặt lịch khám bệnh trực tuyến', 'HaLT05', 'HaLT05', 'Đặt lịch khám bệnh online.', 'Quá tải bệnh viện, chờ đợi lâu.', 'Patient, Doctor, Admin', 'Đặt lịch; Quản lý lịch khám; Nhắc lịch', NULL, '30000000-0000-0000-0000-000000000001'::uuid, '20000000-0000-0000-0000-000000000005'::uuid, now()),
('40000000-0000-0000-0000-000000000013'::uuid, 'SP26SWP13', 'Online Learning Management System', 'Online Learning Management System', 'Hệ thống quản lý học tập trực tuyến', 'NamDQ06', 'NamDQ06', 'Hỗ trợ học online.', 'Thiếu nền tảng học tập tập trung.', 'Student, Teacher, Admin', 'Upload bài học; Làm bài tập; Chấm điểm', NULL, '30000000-0000-0000-0000-000000000001'::uuid, '20000000-0000-0000-0000-000000000006'::uuid, now()),
('40000000-0000-0000-0000-000000000014'::uuid, 'SP26SWP14', 'Job Recruitment Platform', 'Job Recruitment Platform', 'Nền tảng tuyển dụng việc làm', 'TrangBM07', 'TrangBM07', 'Kết nối nhà tuyển dụng và ứng viên.', 'Khó tìm việc phù hợp.', 'Candidate, Recruiter, Admin', 'Đăng job; Apply job; Tracking', NULL, '30000000-0000-0000-0000-000000000001'::uuid, '20000000-0000-0000-0000-000000000007'::uuid, now()),
('40000000-0000-0000-0000-000000000015'::uuid, 'SP26SWP15', 'Online Marketplace for Handmade Products', 'Online Marketplace for Handmade Products', 'Sàn thương mại điện tử đồ handmade', 'KhoaPD08', 'KhoaPD08', 'Nền tảng bán đồ handmade.', 'Người bán nhỏ lẻ khó tiếp cận khách hàng.', 'Seller, Buyer, Admin', 'Đăng sản phẩm; Mua hàng; Thanh toán', NULL, '30000000-0000-0000-0000-000000000001'::uuid, '20000000-0000-0000-0000-000000000008'::uuid, now()),
('40000000-0000-0000-0000-000000000016'::uuid, 'SP26SWP16', 'Fitness Tracking Application', 'Fitness Tracking Application', 'Ứng dụng theo dõi luyện tập thể chất', 'VyNH09', 'VyNH09', 'Theo dõi sức khỏe và tập luyện.', 'Người dùng thiếu động lực theo dõi sức khỏe.', 'User, Trainer', 'Track workout; Goal setting; Analytics', NULL, '30000000-0000-0000-0000-000000000001'::uuid, '20000000-0000-0000-0000-000000000009'::uuid, now()),
('40000000-0000-0000-0000-000000000017'::uuid, 'SP26SWP17', 'Online Library Management System', 'Online Library Management System', 'Hệ thống quản lý thư viện trực tuyến', 'LongTV10', 'LongTV10', 'Quản lý thư viện số.', 'Khó quản lý sách và mượn trả.', 'Reader, Librarian, Admin', 'Mượn sách; Trả sách; Tìm kiếm', NULL, '30000000-0000-0000-0000-000000000001'::uuid, '20000000-0000-0000-0000-000000000010'::uuid, now());

COMMIT;

\connect :AUTH_DB;
SELECT "Id", "FullName", "Email", "Role"
FROM public."Users"
WHERE "Email" LIKE '%@swp.local'
ORDER BY "Email";

\connect :QUESTION_DB;
SELECT "Code", "Name", "SubmittedBy", "ResponsibleBy", "LecturerId"
FROM public."Topics"
ORDER BY "Code";
