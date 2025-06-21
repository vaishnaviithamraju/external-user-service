
namespace UserApiIntegration.Core
{
    public interface IApiClient
    {
        Task<User?> GetUserByIdAsync(int userId);
        Task<UserListResponse?> GetUsersAsync(int page);
    }
}