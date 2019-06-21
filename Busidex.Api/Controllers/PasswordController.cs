using System;
using System.Net;
using System.Net.Http;
using System.Web.Http.Cors;
using System.Web.Mvc;
using System.Web.Security;
using Busidex.Api.DataServices.Interfaces;
using Busidex.Api.Models;

namespace Busidex.Api.Controllers
{
    [RequireHttps]
    [EnableCors("*", "*", "*")]
    public class PasswordController : BaseApiController
    {

        private readonly IAccountRepository _accountRepository;

        public PasswordController(ICardRepository cardRepository, IAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
            _cardRepository = cardRepository;
        }

        public HttpResponseMessage Get()
        {
            return new HttpResponseMessage
            {
                Content = new JsonContent(new
                {
                    Success = false,
                    Model = new ChangePasswordModel()
                }),
                StatusCode = HttpStatusCode.OK
            };
        }

        public HttpResponseMessage Put(ChangePasswordModel model)
        {
            if (model == null || model.NewPassword != model.ConfirmPassword ||
                string.IsNullOrEmpty(model.OldPassword)
                || string.IsNullOrEmpty(model.NewPassword) || string.IsNullOrEmpty(model.ConfirmPassword))
            {
                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = false,
                        Message = "Invalid Model"
                    }),
                    StatusCode = HttpStatusCode.BadRequest
                };
            }

            if (model.UserId == 0)
            {
                model.UserId = ValidateUser();
                if (model.UserId <= 0)
                {
                    return new HttpResponseMessage
                    {
                        Content = new JsonContent(new
                        {
                            Message = "Unauthorized",
                            Success = false
                        }),               
                        StatusCode = HttpStatusCode.Unauthorized
                    };
                }
            }

            bool changePasswordSucceeded = false;
            try
            {
                var user = _accountRepository.GetBusidexUserById(model.UserId);

                if (user != null)
                {
                    MembershipUser currentUser = Membership.GetUser(user.UserName);
                    if (currentUser != null)
                    {
                        changePasswordSucceeded = currentUser.ChangePassword(model.OldPassword, model.NewPassword);
                    }
                    else
                    {
                        _cardRepository.SaveApplicationError(new Exception("MembershipUser not found: " + user.UserName),
                            model.UserId);
                    }
                }
                else
                {
                    _cardRepository.SaveApplicationError(new Exception("BusidexUser not found: " + model.UserId),
                            model.UserId);
                }
            }
            catch (Exception ex)
            {
                if (model != null)
                {
                    _cardRepository.SaveApplicationError(ex, model.UserId);
                }
                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = false,
                        Message = "Password not changed",
                    }),
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }

            if (!changePasswordSucceeded)
            {
                _cardRepository.SaveApplicationError(new Exception("ERROR: Password Not Changed."), model.UserId);
                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = false,
                        Message = "ERROR: Password not changed",
                    }),
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }

            return new HttpResponseMessage
            {
                Content = new JsonContent(new
                {
                    Success = true,
                    Message = "Password changed",
                }),
                StatusCode = HttpStatusCode.OK
            };
        }
    }
}