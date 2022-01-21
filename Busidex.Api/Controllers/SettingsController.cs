using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Mvc;
using Busidex.Api.DataServices.Interfaces;
using Busidex.Api.Models;
using Busidex.Api.DataAccess.DTO;
using BusidexUser = Busidex.Api.DataAccess.DTO.BusidexUser;
using System.Web.Http.Results;
using System.Threading.Tasks;

namespace Busidex.Api.Controllers
{

    [RequireHttps]
    [EnableCors("*", "*", "*")]
    public class SettingsController : BaseApiController
    {

        private readonly ISettingsRepository _settingsRepository;

        public SettingsController(ISettingsRepository settingsRepository)
        {
            if (settingsRepository == null) throw new ArgumentNullException("settingsRepository");
            _settingsRepository = settingsRepository;
        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage GetSystemSettings()
        {
            var id = ValidateUser();
            if (id < 0) return null;

            var settings = _settingsRepository.GetSystemSettings();
            return new HttpResponseMessage
            {
                Content = new JsonContent(new
                {
                    Success = true,
                    Model = settings
                }),

                StatusCode = HttpStatusCode.OK
            };
        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage Get(long id)
        {
            if (id > 0)
            {
                BusidexUser user = _settingsRepository.GetBusidexUserById(id);
                Setting s = user.Settings ?? _settingsRepository.AddDefaultUserSetting(user);
                IEnumerable<Page> pages = _settingsRepository.GetAllSitePages();

                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = true,
                        Model = new SettingsModel
                        {
                            SitePages = pages,
                            UserId = id,
                            CurrentSetting = s
                        }
                    }),

                    StatusCode = HttpStatusCode.OK
                };
            }

            return new HttpResponseMessage
            {
                Content = new JsonContent(new
                {
                    Success = false,
                    Model = new ChangePasswordModel()
                }),
                StatusCode = HttpStatusCode.NotFound
            };
        }

        public HttpResponseMessage Put(SettingsModel model)
        {
            if (model == null || model.UserId <= 0)
            {
                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = false
                    }),
                    StatusCode = HttpStatusCode.NotFound
                };
            }

            _settingsRepository.UpdateSetting(model.CurrentSetting);

            return new HttpResponseMessage
            {
                Content = new JsonContent(new
                {
                    Success = false,
                    Model = model
                }),

                StatusCode = HttpStatusCode.OK
            };
        }
    }
}
