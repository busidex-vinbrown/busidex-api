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
    public class ActivityController : BaseApiController
    {
        public ActivityController(ICardRepository cardRepository)
        {
            _cardRepository = cardRepository;
        }

        [System.Web.Http.HttpGet]
        public HttpResponseMessage GetEventSources()
        {
            var sources = _cardRepository.GetAllEventSources();

            return new HttpResponseMessage
            {
                Content = new JsonContent(new
                {
                    Success = true,
                    EventSources = sources.ToDictionary(s=>s.EventCode, s=>s.EventSourceId)
                }),
                StatusCode = HttpStatusCode.OK
            };
        }

        public HttpResponseMessage Get(long cardId, byte month)
        {
            var activities = _cardRepository.GetEventActivities(cardId, month);
            var aggr = (from item in activities
                group item by item.ActivityDate.GetValueOrDefault().Month
                into mact
                select new
                {
                    Month = mact.Key, 
                    Data = (from detail in mact
                           group detail by detail.EventCode
                           into details
                           select details).ToDictionary(d=>d.Key, d=>d.Count())                           
                }).OrderByDescending(g=>g.Month);

            var sources = _cardRepository.GetAllEventSources();

            return new HttpResponseMessage
            {
                Content = new JsonContent(new
                {
                    Success = true,
                    Sources = sources.Select(s =>new {s.EventCode, s.Description}),
                    Activities = aggr
                }),
                StatusCode = HttpStatusCode.OK
            };  
        }

        public HttpResponseMessage Post(ActivityDTO activity)
        {
            if (activity != null)
            {

                if (!activity.UserId.HasValue)
                {
                    long userId = ValidateUser();
                    if (userId > 0)
                    {
                        activity.UserId = userId;
                    }
                }

                _cardRepository.AddEventActivity(new EventActivity
                {
                    CardId = activity.CardId,
                    UserId = activity.UserId,
                    EventSourceId = activity.EventSourceId,
                    ActivityDate = DateTime.UtcNow
                });

                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = true
                    }),
                    StatusCode = HttpStatusCode.OK
                };
            }

            return new HttpResponseMessage
            {
                Content = null,
                StatusCode = HttpStatusCode.BadRequest
            };
        }
        
    }
}
