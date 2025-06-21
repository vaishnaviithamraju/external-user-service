using System.Text.Json.Serialization;

namespace UserApiIntegration.Core;

public class User
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("first_name")]
    public string FirstName { get; set; } = string.Empty;

    [JsonPropertyName("last_name")]
    public string LastName { get; set; } = string.Empty;

    [JsonPropertyName("avatar")]
    public string? Avatar { get; set; }
}

public class UserListResponse
{
    [JsonPropertyName("page")]
    public int Page { get; set; }

    [JsonPropertyName("per_page")]
    public int PerPage { get; set; }

    [JsonPropertyName("total")]
    public int Total { get; set; }

    [JsonPropertyName("total_pages")]
    public int TotalPages { get; set; }

    [JsonPropertyName("data")]
    public List<User> Data { get; set; } = new();

    [JsonPropertyName("support")]
    public object? Support { get; set; }
}

public class SingleUserResponse
{
    [JsonPropertyName("data")]
    public User Data { get; set; } = new();

    [JsonPropertyName("support")]
    public object? Support { get; set; }
}
