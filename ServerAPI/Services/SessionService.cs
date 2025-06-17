using Microsoft.Extensions.Caching.Distributed;

namespace ServerAPI.Services
{
    public class SessionService : ISessionService
    {
        private readonly IDistributedCache _cache;

        public SessionService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<string> CreateSessionAsync(int userId)
        {
            var sessionId = Guid.NewGuid().ToString();
            await _cache.SetStringAsync($"session:{sessionId}", userId.ToString(), new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
            });
            return sessionId;
        }

        public async Task<string> GetUserIdFromSessionAsync(string sessionId)
        {
            return await _cache.GetStringAsync($"session:{sessionId}");
        }

        public async Task DeleteSessionAsync(string sessionId)
        {
            await _cache.RemoveAsync($"session:{sessionId}");
        }
    }
}
