using LoggingWithCorrelationIdDemo.Model;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace LoggingWithCorrelationIdDemo.Infrastructure;

/// <summary>
/// https://learn.microsoft.com/en-us/dotnet/fundamentals/networking/http/httpclient-guidelines
/// </summary>
public sealed class TodoSystemApiClient(
    HttpClient httpClient,
    ILogger<TodoSystemApiClient> logger) : IDisposable
{
    public async Task<Todo[]> GetUserTodosAsync(int userId)
    {
        try
        {
            // Make HTTP GET request
            // Parse JSON response deserialize into Todo type
            Todo[]? todos = await httpClient.GetFromJsonAsync<Todo[]>(
                    $"todos?userId={userId}",
                    new JsonSerializerOptions(JsonSerializerDefaults.Web));

            return todos ?? [];
        }
        catch (Exception ex)
        {
            logger.LogError("Error getting something fun to say: {Error}", ex);
        }

        return [];
    }

    public void Dispose() => httpClient?.Dispose();
}