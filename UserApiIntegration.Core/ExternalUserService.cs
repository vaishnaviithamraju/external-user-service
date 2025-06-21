using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace UserApiIntegration.Core;

public class ExternalUserService
{
    private readonly IApiClient _apiClient;
    private readonly IMemoryCache _cache;
    private readonly ILogger<ExternalUserService>? _logger;
    private readonly MemoryCacheEntryOptions? _cacheOptions;

    public ExternalUserService(IApiClient apiClient, IMemoryCache? cache = null, ILogger<ExternalUserService>? logger = null, MemoryCacheEntryOptions? cacheOptions = null)
    {
        _apiClient = apiClient;
        _cache = cache;
        _logger = logger;
        _cacheOptions = cacheOptions ?? new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = System.TimeSpan.FromMinutes(5) };
    }

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        if (_cache != null && _cache.TryGetValue<User>($"user_{userId}", out var cachedUserObj))
        {
            return cachedUserObj;
        }
        var user = await _apiClient.GetUserByIdAsync(userId);
        if (user != null && _cache != null)
        {
            _cache.Set($"user_{userId}", user, _cacheOptions);
        }
        return user;
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        const string cacheKey = "all_users";
        var users = new List<User>();
        int page = 1;
        List<User>? cachedUsers = null;

        if (_cache != null)
        {
            _cache.TryGetValue<List<User>>(cacheKey, out cachedUsers);
        }

        if (cachedUsers == null || cachedUsers.Count == 0)
        {
            UserListResponse? response;
            do
            {
                response = await _apiClient.GetUsersAsync(page);
                if (response?.Data != null)
                    users.AddRange(response.Data);
                page++;
            } while (response != null && page <= (response.TotalPages > 0 ? response.TotalPages : 1));

            if (_cache != null)
            {
                _cache.Set(cacheKey, users, _cacheOptions);
            }
        }
        else
        {
            users = cachedUsers;
        }

        return users;
    }
} 