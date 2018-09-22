using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Mvc;
using Busidex.Api.DataAccess.DTO;
using Busidex.Api.DataServices.Interfaces;
using Busidex.Api.Models;

namespace Busidex.Api.Controllers
{
    [RequireHttps]
    [EnableCors("*", "*", "*")]
    public class UserDeviceController : BaseApiController
    {
        private readonly IUserDeviceRepository _userDeviceRepository;

        public UserDeviceController(ICardRepository cardRepository, IUserDeviceRepository userDeviceRepository)
        {
            _userDeviceRepository = userDeviceRepository;
            _cardRepository = cardRepository;
        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage GetCurrentAppInfo()
        {
            return new HttpResponseMessage
            {
                Content = new JsonContent(new
                {
                    Android = 546,
                    iOS = 547
                })
            };
        }


        [System.Web.Http.HttpGet]
        public HttpResponseMessage GetDeviceDetails(DeviceType deviceType)
        {
            var userId = ValidateUser();

            if (userId <= 0)
            {
                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = false,
                        StatusCode = HttpStatusCode.Unauthorized
                    })
                };
            }

            try
            {
                var device = _userDeviceRepository.GetUserDevice(userId, deviceType);

                return new HttpResponseMessage
                {
                    Content = new JsonContent(device),
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                _cardRepository.SaveApplicationError(ex, userId);
                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = false,
                        StatusCode = HttpStatusCode.InternalServerError
                    })
                };
            }
        }

        [System.Web.Http.HttpPut]
        public HttpResponseMessage UpdateUserDevice([FromBody] UserDevice device)
        {
            var userId = ValidateUser();

            if (userId <= 0)
            {
                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = false,
                        StatusCode = HttpStatusCode.Unauthorized
                    })
                };
            }

            if (device == null)
            {
                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = false,
                        StatusCode = HttpStatusCode.BadRequest
                    })
                };
            }

            try
            {
                device.UserId = userId;
                _userDeviceRepository.UpdateUserDevice(device);

                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = true
                    }),
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                _cardRepository.SaveApplicationError(ex, userId);
                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = false,
                        StatusCode = HttpStatusCode.InternalServerError
                    })
                };
            }
        }
    }
}
