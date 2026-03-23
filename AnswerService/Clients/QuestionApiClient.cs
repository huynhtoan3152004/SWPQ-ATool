using System.Net;
using System.Net.Http.Headers;

namespace AnswerService.Clients;

public class QuestionApiClient : IQuestionApiClient
{
    private readonly HttpClient _httpClient;

    public QuestionApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task MarkAnsweredAsync(Guid questionId, string bearerToken, CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Patch, $"questions/{questionId}/mark-answered")
        {
            Content = new StringContent("{}", System.Text.Encoding.UTF8, "application/json")
        };

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
        var response = await _httpClient.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            throw new InvalidOperationException("Question not found.");
        }

        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException($"Failed to update question status: {content}");
        }
    }
}
