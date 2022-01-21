using Busidex.Api.DataServices.Interfaces;
using Busidex.Api.Models;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http.Cors;
using System.Web.Mvc;

namespace Busidex.Api.Controllers
{
    [RequireHttps]
    [EnableCors("*", "*", "*")]
    public class UserNameController : BaseApiController
    {
        private readonly IAccountRepository _accountRepository;

        public UserNameController(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        [System.Web.Http.HttpGet]
        public async Task<bool> IsEmailAvailabile(string email)
        {
            var userAccount = _accountRepository.GetUserAccountByEmail(email);
   
            var result = new HttpResponseMessage
            {
                Content = new JsonContent(new
                {
                    Success = userAccount == null
                }),
                StatusCode = userAccount == null 
                    ? HttpStatusCode.OK 
                    : HttpStatusCode.Conflict
            };

            return await Task.FromResult(userAccount == null);
        }
    }
}
