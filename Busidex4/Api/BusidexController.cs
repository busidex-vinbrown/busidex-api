using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Busidex.BL.Interfaces;
using Busidex.DAL;
using Busidex.BL;

namespace Busidex4.Api
{
    [AllowAnonymous]
    public class BusidexController : ApiController
    {
        // GET /api/<controller>
        //public IEnumerable<string> Get() {
        //    return new string[] { "value1", "value2" };
        //}

        // GET /api/<controller>/5
        //public List<MobileCard> GetMyBusidex(long id)
        //{
        //    ICardRepository repository = new CardRepository(new BusidexDataContext());
        //    var cardList = (from cards in repository.GetMyBusidex(id, true)
        //                    select new MobileCard
        //                              {
        //                                  CardId = cards.Card.CardId,
        //                                  FrontImage = cards.Card.FrontImage,
        //                                  FrontFileId = cards.Card.FrontFileId.ToString(),
        //                                  FrontImageType = cards.Card.FrontType,
        //                                  BackImage = cards.Card.BackImage,
        //                                  //BackFileId = cards.Card.BackFileId.ToString(),
        //                                  //BackImageType = cards.Card.BackType
        //                              }).ToList();

        //    return cardList;
        //}


        // POST /api/<controller>
        public void Post(int c, int t)
        {
            var lr = new LizzidexRepository(new BusidexDataContext());
            //lr.UpdateLizzidex(c, t);
        }

        // PUT /api/<controller>/5
        public void Put(int id, string value)
        {
        }

        // DELETE /api/<controller>/5
        public void Delete(int id)
        {
        }

        //public List<Card> GetMyBusidex(long userId) {

        //    ICardRepository repository = new CardRepository(new Busidex.DAL.BusidexDataContext());
        //    IEnumerable<Card> cardList = from cards in repository.GetMyBusidex(HttpContentExtensions userId)
        //                                 select cards;


        //    return cardList.ToList();
        //}


        //public object Get() {

        //    var lr = new LizzidexRepository(new Busidex.DAL.BusidexDataContext());
        //    Lizzidex lizzy = lr.GetLizzidex();

        //    var l = new LizzidexCount { CoffeeCount = lizzy.CoffeeCount, ThingCount = lizzy.ThingCount };

        //    var ss = new System.Web.Script.Serialization.JavaScriptSerializer();
        //    return ss.Serialize(l);
        //}

        public void UpdateLizzidex(int Coffee, int Thing)
        {

            //var lr = new LizzidexRepository(new Busidex.DAL.BusidexDataContext());
            //lr.UpdateLizzidex(Coffee, Thing);
        }

        // [Serializable]
        public class LizzidexCount
        {
            public int CoffeeCount { get; set; }
            public int ThingCount { get; set; }
        }
    }
}