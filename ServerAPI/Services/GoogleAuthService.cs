using Google.Apis.Auth;

namespace ServerAPI.Services
{
    public class GoogleAuthService : IGoogleAuthService
    {
        private readonly IConfiguration _config;

        public GoogleAuthService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<GoogleJsonWebSignature.Payload> VerifyIdTokenAsync(string idToken)
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { _config["Google:ClientId"] }
            };
            return await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
        }
    }
}
