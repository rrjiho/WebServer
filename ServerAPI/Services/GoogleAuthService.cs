using Google.Apis.Auth;

namespace ServerAPI.Services
{
    public class GoogleAuthService : IGoogleAuthService
    {     
        public async Task<GoogleJsonWebSignature.Payload> VerifyIdTokenAsync(string idToken)
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID") }
            };
            return await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
        }
    }
}
