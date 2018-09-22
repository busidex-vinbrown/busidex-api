using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http.Cors;
using System.Web.Mvc;
using Busidex.Api.DataAccess.DTO;
using Busidex.Api.DataServices.Interfaces;
using Busidex.Api.Models;

namespace Busidex.Api.Controllers
{
    [RequireHttps]
    [EnableCors("*", "*", "*")]
    public class TagsController : BaseApiController
    {
        private readonly IAdminRepository _adminRepository;

        public TagsController(ICardRepository cardRepository, IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
            _cardRepository = cardRepository;
        }

        public HttpResponseMessage GetTags(TagType type, string tag)
        {
            if (!string.IsNullOrEmpty(tag))
            {
                var tags = new List<Tag>();
                tags.Add(_adminRepository.GetTag(type, tag));

                if (tags.FirstOrDefault() == null)
                {
                    return new HttpResponseMessage
                    {
                        Content = new JsonContent(new
                        {
                            Success = false,
                            Tags = tags
                        }),
                        StatusCode = HttpStatusCode.NotFound
                    };  
                }

                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = true,
                        Tags = tags
                    }),
                    StatusCode = HttpStatusCode.OK
                };
            }

            if (type == TagType.System)
            {
                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = true,
                        Tags = _adminRepository.GetSystemTags()
                    }),
                    StatusCode = HttpStatusCode.OK
                };
            }
            if (type == TagType.User)
            {
                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = true,
                        Tags = _adminRepository.GetPopularTags()
                    }),
                    StatusCode = HttpStatusCode.OK
                };
            }
            return new HttpResponseMessage
            {
                Content = new JsonContent(new
                {
                    Success = false,
                    Tags = new List<Tag>()
                }),
                StatusCode = HttpStatusCode.NotFound
            };
        }
    }
}
