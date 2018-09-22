using Newtonsoft.Json;

namespace Busidex.Api.Models
{
    public class OAuthRefreshTokenResponse : OAuthTokenResponse
    {
        [JsonIgnore]
        public new string refresh_token { get; set; }
    }
}