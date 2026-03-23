var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddPostgres("postgres");

var authDb = postgres.AddDatabase("authdb");
var questionDb = postgres.AddDatabase("questiondb");
var answerDb = postgres.AddDatabase("answerdb");

var authService = builder.AddProject<Projects.AuthService>("authservice")
	.WithReference(authDb)
	.WaitFor(authDb);

var questionService = builder.AddProject<Projects.QuestionService>("questionservice")
	.WithReference(questionDb)
	.WithReference(authService)
	.WaitFor(questionDb)
	.WaitFor(authService);

builder.AddProject<Projects.AnswerService>("answerservice")
	.WithReference(answerDb)
	.WithReference(questionService)
	.WithReference(authService)
	.WaitFor(answerDb)
	.WaitFor(questionService)
	.WaitFor(authService);

builder.Build().Run();
