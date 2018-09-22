using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Mvc;
using Busidex.Api.DataAccess.DTO;
using Busidex.Api.DataServices.Interfaces;
using Busidex.Api.Models;
using Google.GData.Contacts;
using Google.GData.Client;
using Google.GData.Extensions;
using Google.Contacts;

namespace Busidex.Api.Controllers
{
    [RequireHttps]
    [EnableCors("*", "*", "*")]
    public class ContactsController : BaseApiController
    {

        private readonly IContactsRepository _contactsRepository;

        public ContactsController(ICardRepository cardRepository, IContactsRepository contactsRepository)

        {
            _cardRepository = cardRepository;
            _contactsRepository = contactsRepository;
        }

        [System.Web.Http.HttpPost]
        public HttpResponseMessage Post()
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

            if (HttpContext.Current.Request.Files.Count > 0)
            {
                var contactData = HttpContext.Current.Request.Files[0];
                if (contactData != null && contactData.ContentLength > 0)
                {
                    var contacts = ReadContacts(contactData);

                    return new HttpResponseMessage
                    {
                        Content = new JsonContent(new
                        {
                            Success = true,
                            Contacts = contacts
                        }),
                        StatusCode = HttpStatusCode.OK
                    };
                }
            }

            return new HttpResponseMessage
            {
                Content = new JsonContent(new
                {
                    Success = false
                }),
                StatusCode = HttpStatusCode.BadRequest
            };
        }

        private List<ContactData> ReadContacts(HttpPostedFile file)
        {

            return _contactsRepository.ReadContacts(file);

        }
    }
}
