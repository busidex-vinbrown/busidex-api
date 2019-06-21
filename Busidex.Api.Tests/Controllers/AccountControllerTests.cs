using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using Busidex.Api.DataAccess.DTO;
using Busidex.Api.Models;
using NUnit.Framework;

namespace Busidex.Api.Tests.Controllers
{
    public class AccountControllerTests : BaseControllerTest
    {
        private string _username;
        private string _password;
        private long _userId;
        private string _email;

        public override void Setup()
        {
            base.Setup();
            _username = ConfigurationManager.AppSettings["username"];
            _password = ConfigurationManager.AppSettings["password"];
            _userId = long.Parse(ConfigurationManager.AppSettings["userid"]);
            _email = ConfigurationManager.AppSettings["email"];
        }

        [Test]
        public void TestAccountLoginWithValidKnownUserId()
        {
            var account = ApiClient.ExecuteRequest<UserAccount>(HttpMethod.Get, "/account",
                new Dictionary<string, string>
                {
                    {"id", "65"}
                });
            Assert.AreEqual(account.UserId, 65);
        }

        [Test]
        public void TestAccountLoginWithInValidId()
        {
            var account = ApiClient.ExecuteRequest<UserAccount>(HttpMethod.Get, "/account/", 
                new Dictionary<string, string>
                {
                    {"id", "-1"}
                });
            Assert.IsNull(account);
        }

        [Test]
        public void TestAccountLoginWithKnownUsernameAndPassword()
        {
            var account = ApiClient.ExecuteRequest<UserAccount>(HttpMethod.Post, "/account/login",
                new Dictionary<string, string>
                {
                    {"UserName", _username},
                    {"Password", _password}
                });
            Assert.AreEqual(_userId, account.UserId);
        }

        [Test]
        public void TestAccountLoginWithKnownLoginParamsObject()
        {
            var param = new LoginParams
            {
                UserName = _username,
                Password = _password,
                RememberMe = false
            };

            var account = ApiClient.ExecuteRequest<UserAccount>(HttpMethod.Post, "/account/login", null, param);
            Assert.AreEqual(_userId, account.UserId);
        }

        [Test]
        public void TestAccountLoginWithBadLoginParamsObject()
        {
            var param = new LoginParams
            {
                UserName = string.Empty,
                Password = string.Empty,
                RememberMe = false
            };

            ApiClient.ExecuteRequest<UserAccount>(HttpMethod.Post, "/account/login", null, param);
           
        }

        [Test]
        public void TestUpdateUserAccount()
        {
            var user = new BusidexUser
            {
                UserId = _userId, 
                Email = _email, 
                UserName = _username,
                Address = null
            };

            ApiClient.ExecuteRequest<BusidexUser>(HttpMethod.Put, "/account/UpdateUserAccount", null, user);

        }
    }
}
