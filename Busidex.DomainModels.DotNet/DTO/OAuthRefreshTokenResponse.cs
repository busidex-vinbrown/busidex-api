using Newtonsoft.Json;

namespace Busidex.DomainModels.DotNet.DTO
{
    public class OAuthRefreshTokenResponse : OAuthTokenResponse
    {
        [JsonIgnore]
        public new string refresh_token { get; set; }
    }
}