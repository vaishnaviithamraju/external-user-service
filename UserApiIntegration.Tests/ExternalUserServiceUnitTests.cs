using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using UserApiIntegration.Core;
using Xunit;

namespace UserApiIntegration.Tests
{
    public class ExternalUserServiceTests
    {
        private readonly Mock<IApiClient> _apiClientMock;
        private readonly IMemoryCache _memoryCache;
        private readonly Mock<ILogger<ExternalUserService>> _loggerMock;
        private readonly ExternalUserService _service;

        public ExternalUserServiceTests()
        {
            _apiClientMock = new Mock<IApiClient>(); 
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            _loggerMock = new Mock<ILogger<ExternalUserService>>();
            _service = new ExternalUserService(_apiClientMock.Object, _memoryCache, _loggerMock.Object);
        }

        [Fact]
        public async Task GetUserByIdAsync_ReturnsUser_FromCache()
        {
            var user = new User { Id = 1, Email = "test@example.com", FirstName = "Test", LastName = "User" };
            _memoryCache.Set("user_1", user);

            var result = await _service.GetUserByIdAsync(1);

            Assert.Equal(user, result);
            _apiClientMock.Verify(x => x.GetUserByIdAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task GetUserByIdAsync_ReturnsUser_FromApi_AndCaches()
        {
            var user = new User { Id = 2, Email = "api@example.com", FirstName = "Api", LastName = "User" };
            _apiClientMock.Setup(x => x.GetUserByIdAsync(2)).ReturnsAsync(user);

            var result = await _service.GetUserByIdAsync(2);

            Assert.Equal(user, result);
            Assert.True(_memoryCache.TryGetValue("user_2", out User cachedUser));
            Assert.Equal(user, cachedUser);
        }

        [Fact]
        public async Task GetAllUsersAsync_ReturnsCachedUsers_IfPresent()
        {
            // Arrange
            var mockApiClient = new Mock<IApiClient>();
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var cachedUsers = new List<User> { new User { Id = 1, Email = "test@example.com" } };
            memoryCache.Set("all_users", cachedUsers);
            var service = new ExternalUserService(mockApiClient.Object, memoryCache);

            // Act
            var result = await service.GetAllUsersAsync();

            // Assert
            Assert.Equal(cachedUsers, result);
            mockApiClient.Verify(x => x.GetUsersAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task GetAllUsersAsync_FetchesAndCachesUsers_IfNotCached()
        {
            // Arrange
            var mockApiClient = new Mock<IApiClient>();
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            var usersPage1 = new List<User> { new User { Id = 1, Email = "a@b.com" } };
            var usersPage2 = new List<User> { new User { Id = 2, Email = "b@c.com" } };
            mockApiClient.Setup(x => x.GetUsersAsync(1)).ReturnsAsync(new UserListResponse { Data = usersPage1, TotalPages = 2 });
            mockApiClient.Setup(x => x.GetUsersAsync(2)).ReturnsAsync(new UserListResponse { Data = usersPage2, TotalPages = 2 });
            var service = new ExternalUserService(mockApiClient.Object, memoryCache);

            // Act
            var result = await service.GetAllUsersAsync();

            // Assert
            var resultList = Assert.IsAssignableFrom<IEnumerable<User>>(result);
            Assert.Contains(resultList, u => u.Id == 1);
            Assert.Contains(resultList, u => u.Id == 2);
            Assert.True(memoryCache.TryGetValue("all_users", out List<User> cached));
            Assert.Equal(2, cached.Count);
            mockApiClient.Verify(x => x.GetUsersAsync(It.IsAny<int>()), Times.Exactly(2));
        }

        [Fact]
        public async Task GetAllUsersAsync_ReturnsEmptyList_IfApiReturnsNoUsers()
        {
            // Arrange
            var mockApiClient = new Mock<IApiClient>();
            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            mockApiClient.Setup(x => x.GetUsersAsync(1)).ReturnsAsync(new UserListResponse { Data = new List<User>(), TotalPages = 1 });
            var service = new ExternalUserService(mockApiClient.Object, memoryCache);

            // Act
            var result = await service.GetAllUsersAsync();

            // Assert
            Assert.Empty(result);
            Assert.True(memoryCache.TryGetValue("all_users", out List<User> cached));
            Assert.Empty(cached);
        }
    }
}