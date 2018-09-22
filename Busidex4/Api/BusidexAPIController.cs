using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using Busidex.DAL;
using Busidex.BL;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Busidex4.Api.Controllers {
    public class BusidexAPIController : ApiController {
        // GET /api/<controller>
        //public IEnumerable<string> Get() {
        //    return new string[] { "value1", "value2" };
        //}

        // GET /api/<controller>/5
        public string Get(int id) {
            return "value";
        }

        // POST /api/<controller>
        public void Post(int c, int t) {
            LizzidexRepository lr = new LizzidexRepository(new Busidex.DAL.BusidexDataContext());
            lr.UpdateLizzidex(c, t);
        }

        // PUT /api/<controller>/5
        public void Put(int id, string value) {
        }

        // DELETE /api/<controller>/5
        public void Delete(int id) {
        }

        //public List<Card> GetMyBusidex(long userId) {

        //    ICardRepository repository = new CardRepository(new Busidex.DAL.BusidexDataContext());
        //    IEnumerable<Card> cardList = from cards in repository.GetMyBusidex(HttpContentExtensions userId)
        //                                 select cards;


        //    return cardList.ToList();
        //}


        public object Get() {

            LizzidexRepository lr = new LizzidexRepository(new Busidex.DAL.BusidexDataContext());
            Lizzidex lizzy = lr.GetLizzidex();

            var l = new LizzidexCount { CoffeeCount = lizzy.CoffeeCount, ThingCount = lizzy.ThingCount };

            System.Web.Script.Serialization.JavaScriptSerializer ss = new System.Web.Script.Serialization.JavaScriptSerializer();
            return ss.Serialize(l);
        }

        public void UpdateLizzidex(int Coffee, int Thing) {

            LizzidexRepository lr = new LizzidexRepository(new Busidex.DAL.BusidexDataContext());
            lr.UpdateLizzidex(Coffee, Thing);
        }

       // [Serializable]
        public class LizzidexCount {
            public int CoffeeCount { get; set; }
            public int ThingCount { get; set; }
        }
    }
}