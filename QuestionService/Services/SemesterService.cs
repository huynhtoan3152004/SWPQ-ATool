using QuestionService.Entities;
using QuestionService.Models;
using QuestionService.Repositories;

namespace QuestionService.Services;

public class SemesterService : ISemesterService
{
    private readonly ISemesterRepository _semesterRepository;

    public SemesterService(ISemesterRepository semesterRepository)
    {
        _semesterRepository = semesterRepository;
    }

    public async Task<SemesterResponse> CreateAsync(CreateSemesterRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            throw new InvalidOperationException("Semester name is required.");
        }

        if (request.Year < 2000 || request.Year > 2100)
        {
            throw new InvalidOperationException("Year must be in range 2000-2100.");
        }

        if (request.Month < 1 || request.Month > 12)
        {
            throw new InvalidOperationException("Month must be in range 1-12.");
        }

        var normalizedName = request.Name.Trim();
        var isDuplicate = await _semesterRepository.ExistsAsync(normalizedName, request.Year, request.Month, cancellationToken);
        if (isDuplicate)
        {
            throw new InvalidOperationException("Semester already exists.");
        }

        var semester = new Semester
        {
            Name = normalizedName,
            Year = request.Year,
            Month = request.Month,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _semesterRepository.AddAsync(semester, cancellationToken);
        return ToResponse(created);
    }

    public async Task<List<SemesterResponse>> GetAllAsync(string? name, int? year, int? month, CancellationToken cancellationToken = default)
    {
        var semesters = await _semesterRepository.GetAllAsync(name, year, month, cancellationToken);
        return semesters.Select(ToResponse).ToList();
    }

    private static SemesterResponse ToResponse(Semester semester)
    {
        return new SemesterResponse(semester.Id, semester.Name, semester.Year, semester.Month, semester.CreatedAt);
    }
}
