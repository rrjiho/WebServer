namespace ServerAPI.Services
{
    public interface ISessionService
    {
        Task<string> CreateSessionAsync(int userId);
        Task<string> GetUserIdFromSessionAsync(string sessionId);
        Task DeleteSessionAsync(string sessionId);
    }
}
