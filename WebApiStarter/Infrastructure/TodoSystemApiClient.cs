using KafkaFlow.Producers;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using WebApiStarter.Domain.Events;
using WebApiStarter.Domain.Model;
using WebApiStarter.Services;

namespace WebApiStarter.Infrastructure;

public interface ITodoSystemApiClient
{
    Task<int> CreateAsync(int userId, Todo item);
    Task<Todo[]> GetUserTodosAsync(int userId);
    void Dispose();
}

/// <summary>
/// https://learn.microsoft.com/en-us/dotnet/fundamentals/networking/http/httpclient-guidelines
/// </summary>
public sealed class TodoSystemApiClient(
    HttpClient httpClient,
    ILogger<TodoSystemApiClient> logger, EventProducer eventProducer) : IDisposable, ITodoSystemApiClient
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

    public async Task<int> CreateAsync(int userId, Todo item)
    {
        try
        {
            HttpResponseMessage response = await httpClient.PostAsJsonAsync<Todo>(
                    $"todos", item, new JsonSerializerOptions(JsonSerializerDefaults.Web));

            response.EnsureSuccessStatusCode();
            var insertedItem = await response.Content.ReadFromJsonAsync<Todo>();

            await eventProducer.ProduceAsync(new TodoItemCreatedNotification() {Item = insertedItem });

            return insertedItem.Id;

        }
        catch (Exception ex)
        {
            logger.LogError("Error getting something fun to say: {Error}", ex);
            throw;
        }
    }

    public void Dispose() => httpClient?.Dispose();
}