using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using Busidex.BL;
using Busidex.BL.Interfaces;
using Busidex.DAL;

namespace Busidex4.Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service1 : IService1
    {
        readonly BusidexDataContext _bdc = new BusidexDataContext();

        public void DoWork()
        {
        }

        public List<Card> GetMyBusidex(long userId)
        {

            ICardRepository repository = new CardRepository(_bdc);
            IEnumerable<Card> cardList = from cards in repository.GetMyBusidex(userId)
                                         select cards.Card;

            return cardList.ToList();
        }
    }
}
