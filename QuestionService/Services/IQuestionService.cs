using QuestionService.Entities;
using QuestionService.Models;

namespace QuestionService.Services;

public interface IQuestionService
{
    Task<QuestionResponse> CreateAsync(Guid studentId, CreateQuestionRequest request, CancellationToken cancellationToken = default);
    Task<List<QuestionResponse>> GetAllAsync(Guid? topicId, Guid? semesterId, QuestionVisibility? visibility, CancellationToken cancellationToken = default);
    Task<QuestionResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<QuestionResponse?> ApproveAsync(Guid questionId, Guid gvhdId, CancellationToken cancellationToken = default);
    Task<QuestionResponse?> AssignAsync(Guid questionId, Guid teacherId, CancellationToken cancellationToken = default);
    Task<QuestionResponse?> MarkAnsweredAsync(Guid questionId, CancellationToken cancellationToken = default);
}
