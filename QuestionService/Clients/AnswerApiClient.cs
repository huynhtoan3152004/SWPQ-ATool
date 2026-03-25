using System.Net.Http.Json;
using QuestionService.Models;

namespace QuestionService.Clients;

public class AnswerApiClient : IAnswerApiClient
{
    private readonly HttpClient _httpClient;

    public AnswerApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Dictionary<Guid, List<TopicAnswerResponse>>> GetAnswersByQuestionIdsAsync(
        IReadOnlyCollection<Guid> questionIds,
        string bearerToken,
        CancellationToken cancellationToken = default)
    {
        if (questionIds.Count == 0)
        {
            return new Dictionary<Guid, List<TopicAnswerResponse>>();
        }

        var request = new HttpRequestMessage(HttpMethod.Post, "answers/by-questions")
        {
            Content = JsonContent.Create(new
            {
                questionIds
            })
        };

        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);

        var response = await _httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException($"Failed to get answers from AnswerService. Status={(int)response.StatusCode}. Body={errorContent}");
        }

        var payload = await response.Content.ReadFromJsonAsync<List<AnswersByQuestionApiResponse>>(cancellationToken: cancellationToken)
            ?? new List<AnswersByQuestionApiResponse>();

        return payload.ToDictionary(
            item => item.QuestionId,
            item => item.Answers
                .Select(answer => new TopicAnswerResponse(
                    answer.Id,
                    answer.QuestionId,
                    answer.TeacherId,
                    answer.Content,
                    answer.CreatedAt))
                .ToList());
    }

    private sealed record AnswerApiResponse(
        Guid Id,
        Guid QuestionId,
        Guid TeacherId,
        string Content,
        DateTime CreatedAt);

    private sealed record AnswersByQuestionApiResponse(
        Guid QuestionId,
        List<AnswerApiResponse> Answers);
}
