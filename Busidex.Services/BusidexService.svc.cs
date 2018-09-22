using System.Collections.Generic;
using System.Linq;
using Busidex.BL;
using Busidex.BL.Interfaces;
using Busidex.DAL;

namespace Busidex.Services
{
    public class BusidexService : IBusidexService
    {
        readonly BusidexDataContext _bdc = new BusidexDataContext();

        public List<Card> GetMyBusidex(long userId)
        {

            ICardRepository repository = new CardRepository(new BusidexDataContext());
            IEnumerable<Card> cardList = from cards in repository.GetMyBusidex(userId, false)
                                         select cards.Card;

            return cardList.ToList();
        }

        //public string GetLizzidex() {
        //    Lizzidex lizzy = _bdc.GetLizzidex();
        //    var s = new System.Web.Script.Serialization.JavaScriptSerializer();
        //    var l = new LizzidexCount { CoffeeCount = lizzy.CoffeeCount, ThingCount = lizzy.ThingCount };
        //    return s.Serialize(l) ;
        //}

        //public void UpdateLizzidex(int coffee, int thing)
        //{

        //    _bdc.UpdateLizzidex(coffee, thing);
        //}
    }
}
