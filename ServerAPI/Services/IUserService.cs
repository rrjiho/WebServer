using Google.Apis.Auth;
using ServerAPI.Entities;
using ServerAPI.Models;

namespace ServerAPI.Services
{
    public interface IUserService
    {
        Task<User> FindOrCreateUserAsync(GoogleJsonWebSignature.Payload payload);
        Task<User> GetProfileAsync(string userId);
        Task AddExperienceAsync(string userId, int exp);
        Task<Resources> GetResourcesAsync(string userId);
        Task AddResourcesAsync(string userId, int gold, int gems);
        Task UseResourcesAsync(string userId, int gold, int gems);
    }
}
