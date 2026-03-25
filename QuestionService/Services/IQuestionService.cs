using QuestionService.Entities;
using QuestionService.Models;

namespace QuestionService.Services;

public interface IQuestionService
{
    Task<QuestionResponse> CreateAsync(Guid askedBy, CreateQuestionRequest request, CancellationToken cancellationToken = default);
    Task<List<QuestionResponse>> GetAllAsync(Guid? topicId, Guid? semesterId, Guid? assignedTo, QuestionVisibility? visibility, int? year, int? month, CancellationToken cancellationToken = default);
    Task<TopicQuestionThreadResponse?> GetTopicThreadAsync(Guid topicId, Guid semesterId, string bearerToken, CancellationToken cancellationToken = default);
    Task<QuestionResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<QuestionResponse?> ApproveAsync(Guid questionId, Guid gvhdId, CancellationToken cancellationToken = default);
    Task<QuestionResponse?> AssignAsync(Guid questionId, Guid teacherId, CancellationToken cancellationToken = default);
    Task<QuestionResponse?> MarkAnsweredAsync(Guid questionId, Guid teacherId, CancellationToken cancellationToken = default);
}
