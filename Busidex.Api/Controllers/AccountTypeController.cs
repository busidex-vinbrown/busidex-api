using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Cors;
using Busidex.Api.DataServices.Interfaces;
using Busidex.Api.HttpPipeline;
using Busidex.Api.Models;

namespace Busidex.Api.Controllers
{
    [ApiExceptionFilter]
    [EnableCors("*", "*", "*")]
    public class AccountTypeController : BaseApiController
    {
        private readonly IAccountRepository _accountRepository;
        
        public AccountTypeController(IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;        
        }

        public HttpResponseMessage Get(long id)
        {
            if (id <= 0)
            {
                id = ValidateUser();
            }
            try
            {
                var plans = _accountRepository.GetActivePlans().OrderBy(p => p.DisplayOrder).ToList();
                var userPlan = _accountRepository.GetUserAccountByUserId(id);

                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Plans = plans,
                        SelectedPlanId = userPlan.AccountTypeId
                    })
                };
            }
            catch (Exception ex)
            {
                _cardRepository.SaveApplicationError(ex, 0);
                return new HttpResponseMessage
                {
                    Content = new JsonContent(new { Message = "error" })
                };
            }
        }

        public HttpResponseMessage Put(long userAccountId, int accountTypeId)
        {
            var userId = ValidateUser();
            if (userId <= 0)
            {
                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = false
                    }),
                    StatusCode = HttpStatusCode.Unauthorized
                };
            }

            try
            {

                _accountRepository.UpdateUserAccount(userAccountId, accountTypeId);

                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = true
                    })
                };
            }
            catch (Exception ex)
            {
                _cardRepository.SaveApplicationError(ex, 0);
                return new HttpResponseMessage
                {
                    Content = new JsonContent(new { Message = "error" })
                };
            }
        }

    }
}
