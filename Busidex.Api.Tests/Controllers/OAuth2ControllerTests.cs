using System;
using System.Configuration;
using Busidex.Api.Models;
using NUnit.Framework;

namespace Busidex.Api.Tests.Controllers
{
    public class OAuth2ControllerTests : BaseControllerTest
    {
        private string _username;
        private string _password;
        private long _userId;

        public override void Setup()
        {
            base.Setup();
            _username = ConfigurationManager.AppSettings["username"];
            _password = ConfigurationManager.AppSettings["password"];
            _userId = long.Parse(ConfigurationManager.AppSettings["userid"]);
        }

        [Test]
        public void TestAcquireAndRevoke()
        {
             OAuthTokenResponse resp = ApiClient.AcquireAuthToken(_username, _password, scope: "api", grantType: "access_token");

             Assert.AreEqual(_userId, resp.MemberID);
            Assert.AreNotEqual(resp.access_token, Guid.Empty);
            Assert.AreNotEqual(resp.refresh_token, Guid.Empty);
            Assert.AreEqual(resp.scope, "api");
            Assert.AreEqual(resp.token_type, "Bearer");

            ApiClient.RevokeToken(resp.refresh_token);
        }

        [Test]
        public void TestAcquireWithBadCredentials()
        {
            var response = ApiClient.AcquireAuthToken(username: "bogus", password: "bogus");
            Assert.AreEqual(response.MemberID, 0);
        }

        [Test]
        [ExpectedException(typeof(ApiException))]
        public void TestAcquireWithBadScope()
        {
            ApiClient.AcquireAuthToken(_username, _password, scope: "bogus");
        }

        [Test]
        [ExpectedException(typeof(ApiException))]
        public void TestAcquireWithBadGrant()
        {
            ApiClient.AcquireAuthToken(_username, _password, grantType: "bogus");
        }

        [Test]
        [ExpectedException(typeof(ApiException))]
        public void TestAcquireWithMissingParams()
        {
            ApiClient.AcquireAuthToken(string.Empty, string.Empty);
        }

        [Test]
        public void TestRefresh()
        {
            OAuthTokenResponse resp1 = ApiClient.AcquireAuthToken(_username, _password);
            OAuthTokenResponse resp2 = ApiClient.RefreshAuthToken();
            Assert.AreNotEqual(resp1.refresh_token, resp2.refresh_token);
            ApiClient.RevokeToken();
        }

        [Test]
        [ExpectedException(typeof(ApiException))]
        public void TestRevokeWithoutToken()
        {
            ApiClient.RevokeToken(string.Empty);
        }

        [Test]
        [ExpectedException(typeof(ApiException))]
        public void TestRefreshWithBadScope()
        {
            var response = ApiClient.RefreshAuthToken(scope: "bogus", grantType: "access_token");
            
        }

        [Test]
        [ExpectedException(typeof(ApiException))]
        public void TestRefreshWithBadGrant()
        {
            ApiClient.RefreshAuthToken(grantType: "bogus");
        }

        [Test]
        [ExpectedException(typeof(ApiException))]
        public void TestRefreshWithoutToken()
        {
            ApiClient.RefreshAuthToken();
        }

        [Test]
        [ExpectedException(typeof(ApiException))]
        public void TestRefreshWithBadToken()
        {
            ApiClient.RefreshAuthToken(refreshToken: "bogus");
        }

        [Test]
        public void TestRefreshWithRandomGuidToken()
        {
            var response = ApiClient.RefreshAuthToken(refreshToken: Guid.NewGuid().ToString());
            Assert.AreEqual(response.MemberID, 0);
        }

        protected override void AutoAuthenticate(ApiClient client)
        {
            // Don't perform auto-authentication so we can test OAuth specifically
        }
    }
}
