﻿namespace Busidex.DomainModels.DotNet.DTO
{
    public class OAuthTokenRequest
    {
        public string username { get; set; }

        public string password { get; set; }

        public string grant_type { get; set; }

        public string scope { get; set; }

        public string refresh_token { get; set; }
    }
}