using Google.Apis.Auth;
using Microsoft.EntityFrameworkCore;
using ServerAPI.DB;
using ServerAPI.Entities;

namespace ServerAPI.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        // 이것도 DI Container가 bulider 해놓은 context 객체와 참조 연결해줌 IConfiguration 마찬가지 원리
        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User> FindOrCreateUserByGoogleIdAsync(string googleId, string? nickname)
        {
            var user = await _context.Users
                                     .FirstOrDefaultAsync(u => u.GoogleId == googleId);

            if (user == null)
            {
                user = new User
                {
                    GoogleId = googleId,
                    Email = $"{googleId}@test.com",        
                    Username = nickname ?? googleId             
                                                                
                };

                user.Resources = new Resources();                 
                _context.Users.Add(user);
                await _context.SaveChangesAsync();                
            }

            return user;
        }

        public async Task<User> FindOrCreateUserAsync(GoogleJsonWebSignature.Payload payload)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.GoogleId == payload.Subject);

            if (user == null)
            {
                user = new User
                {
                    GoogleId = payload.Subject,
                    Email = payload.Email,
                    Username = payload.Email.Split('@')[0],
                };

                var resources = new Resources { UserId = user.Id };
                user.Resources = resources;

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }

            return user;
        }

        public async Task<User> GetProfileAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId)) throw new ArgumentException("User ID cannot be null or empty");

            var user = await _context.Users.FindAsync(int.Parse(userId));
            if (user == null) throw new Exception("User not found");
            return user;
        }

        public async Task AddExperienceAsync(string userId, int exp)
        {
            if (string.IsNullOrEmpty(userId)) throw new ArgumentException("User ID cannot be null or empty");

            var user = await _context.Users.FindAsync(int.Parse(userId));
            if (user == null) throw new Exception("User not found");

            user.Experience += exp;
            if(user.Experience >= 100)
            {
                user.Level++;
                user.Experience -= 100;
                user.Strength += 2;
                user.Agility += 2;
                user.Intelligence += 2;
            }
            await _context.SaveChangesAsync();
        }

        public async Task<Resources> GetResourcesAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId)) throw new ArgumentException("User ID cannot be null or empty");

            var resources = await _context.Resources.FirstOrDefaultAsync(r => r.UserId == int.Parse(userId));
            if (resources == null) throw new Exception("Don't have any resources");
            return resources;
        }

        public async Task AddResourcesAsync(string userId, int gold, int gems)
        {
            if (string.IsNullOrEmpty(userId)) throw new ArgumentException("User ID cannot be null or empty");

            var resources = await _context.Resources.FirstOrDefaultAsync(r => r.UserId == int.Parse(userId));
            if (resources == null) throw new Exception("Don't have any resources");

            resources.Gold += gold;
            resources.Gems += gems;
            await _context.SaveChangesAsync();
        }

        public async Task UseResourcesAsync(string userId, int gold, int gems)
        {
            if (string.IsNullOrEmpty(userId)) throw new ArgumentException("User ID cannot be null or empty");

            var resources = await _context.Resources.FirstOrDefaultAsync(r => r.UserId == int.Parse(userId));
            if (resources == null) throw new Exception("Don't have any resources");
            if (resources.Gold < gold || resources.Gems < gems) throw new Exception("Insufficient resources");

            resources.Gold -= gold;
            resources.Gems -= gems;
            await _context.SaveChangesAsync();
        }
    }
}
