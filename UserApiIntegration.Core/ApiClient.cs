using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace UserApiIntegration.Core;

public class ApiClient : IApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ApiClient>? _logger;

    public ApiClient(HttpClient httpClient, ILogger<ApiClient>? logger = null)
    {
        _httpClient = httpClient;
        _logger = logger;
        if (!_httpClient.DefaultRequestHeaders.Contains("x-api-key"))
        {
            _httpClient.DefaultRequestHeaders.Add("x-api-key", "reqres-free-v1");
        }
    }

    public async Task<UserListResponse?> GetUsersAsync(int page)
    {
        var response = await _httpClient.GetAsync($"users?page={page}");
        if (!response.IsSuccessStatusCode)
        {
            _logger?.LogError("Failed to fetch users: {StatusCode}", response.StatusCode);
            throw new HttpRequestException($"Failed to fetch users: {response.StatusCode}");
        }

        var stream = await response.Content.ReadAsStreamAsync();
        try
        {
            var result = await JsonSerializer.DeserializeAsync<UserListResponse>(stream);
            if (result == null)
                throw new JsonException("Deserialized UserListResponse is null");
            return result;
        }
        catch (JsonException ex)
        {
            _logger?.LogError(ex, "Deserialization error for user list");
            return null;
        }
    }

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        var response = await _httpClient.GetAsync($"users/{userId}");
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
        if (!response.IsSuccessStatusCode)
        {
            _logger?.LogError("Failed to fetch user {UserId}: {StatusCode}", userId, response.StatusCode);
            throw new HttpRequestException($"Failed to fetch user {userId}: {response.StatusCode}");
        }
        var stream = await response.Content.ReadAsStreamAsync();
        try
        {
            var result = await JsonSerializer.DeserializeAsync<SingleUserResponse>(stream);
            if (result == null)
                throw new JsonException("Deserialized SingleUserResponse is null");
            return result.Data;
        }
        catch (JsonException ex)
        {
            _logger?.LogError(ex, "Deserialization error for user {UserId}", userId);
            return null;
        }
    }
} 