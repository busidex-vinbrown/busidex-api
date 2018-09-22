using System.Collections.Generic;
using System.Web.Management;
using Busidex.Profile.DataAccess;

namespace Busidex.Profile.Services
{
    public static class CardService
    {
        static readonly BusidexDao _dao = new BusidexDao();

        public static CardDetailModel GetCardBySEOName(string name)
        {
            return _dao.GetCardBySEOName(name);
        }

        public static List<CardDetailModel> GetAllCards()
        {
            return _dao.GetAllCards();
        } 
    }
}
