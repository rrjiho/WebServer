using Google.Apis.Auth;

namespace ServerAPI.Services
{
    public interface IGoogleAuthService
    {
        Task<GoogleJsonWebSignature.Payload> VerifyIdTokenAsync(string idToken);
    }
}
