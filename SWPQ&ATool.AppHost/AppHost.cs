var builder = DistributedApplication.CreateBuilder(args);

var fePath = @"D:\WorkSpace\Ki8\ASSPRN\swp-q&a-system";

var postgres = builder.AddPostgres("postgres")
	.WithDataVolume()
	.WithPgAdmin();

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

var answerService = builder.AddProject<Projects.AnswerService>("answerservice")
	.WithReference(answerDb)
	.WithReference(questionService)
	.WithReference(authService)
	.WaitFor(answerDb)
	.WaitFor(questionService)
	.WaitFor(authService);

builder.AddExecutable(
	"fe",
	"npm",
	workingDirectory: fePath,
	args: new[] { "run", "dev", "--", "--host", "0.0.0.0", "--port", "3000" })
	.WithEnvironment("VITE_AUTH_API", "/proxy/auth")
	.WithEnvironment("VITE_QUESTION_API", "/proxy/question")
	.WithEnvironment("VITE_ANSWER_API", "/proxy/answer")
	.WithHttpEndpoint(name: "http", port: 3000, targetPort: 3000, isProxied: false)
	.WithExternalHttpEndpoints()
	.WaitFor(authService)
	.WaitFor(questionService)
	.WaitFor(answerService);

builder.Build().Run();
