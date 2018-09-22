using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Busidex.Api.DataServices;
using Busidex.Api.Models;

namespace Busidex.Api.Controllers
{
    public class OAuth2Controller : BaseApiController
    {
        private static readonly OAuth2Service s_service = new OAuth2Service();

        /// <summary>
        /// Two use cases:
        /// 1) Obtains an access token and refresh token based on a user’s credentials, or
        /// 2) Obtains a new access token from an existing refresh token.  Any other access tokens associated with the refresh token are invalidated.
        /// </summary>
        [HttpPost]
        public OAuthTokenResponse token([FromBody] OAuthTokenRequest tokenRequest)
        {
            if (tokenRequest == null)
            {
                throw new ApiException(HttpStatusCode.BadRequest, "The request is not properly formatted");
            }

            if (string.IsNullOrEmpty(tokenRequest.grant_type))
            {
                tokenRequest.grant_type = OAuthConstants.ACCESS_TOKEN;
            }

            switch (tokenRequest.grant_type)
            {
                case OAuthConstants.ACCESS_TOKEN:
                    return OAuth2Service.ExecuteTokenFlow(tokenRequest);

                case OAuthConstants.REFRESH_TOKEN:
                    return OAuth2Service.ExecuteRefreshTokenFlow(tokenRequest);

                default:
                    throw new ApiException(HttpStatusCode.BadRequest, "The grant_type is invalid");
            }
        }

        /// <summary>
        /// Revokes a refresh token and all access tokens associated with it
        /// </summary>
        [HttpPost]
        public void Revoke([FromBody] RevokeRequest revokeRequest)
        {
            if (revokeRequest == null || string.IsNullOrEmpty(revokeRequest.refresh_token))
            {
                throw new ApiException(HttpStatusCode.BadRequest, "Missing required parameter:  refresh_token");
            }

            OAuth2Service.DeleteClientAuthorization(revokeRequest.refresh_token);
        }

        protected OAuth2Service OAuth2Service
        {
            get { return s_service; }
        }
    }
}
