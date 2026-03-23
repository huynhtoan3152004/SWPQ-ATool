using QuestionService.Models;

namespace QuestionService.Services;

public interface ISemesterService
{
    Task<SemesterResponse> CreateAsync(CreateSemesterRequest request, CancellationToken cancellationToken = default);
    Task<List<SemesterResponse>> GetAllAsync(string? name, int? year, int? month, CancellationToken cancellationToken = default);
}
