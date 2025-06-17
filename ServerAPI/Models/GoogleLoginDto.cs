using System.Text.Json.Serialization;

namespace ServerAPI.Models
{
    public class GoogleLoginDto
    {
        [JsonPropertyName("idToken")]
        public string IdToken { get; set; }
    }
}
