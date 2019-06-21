using System.Collections.Generic;
using Busidex.DataAccess;
using Busidex.DomainModels;
using Busidex.Services.Interfaces;

namespace Busidex.Services
{
    public class AdminRepository : RepositoryBase, IAdminRepository
    {
        //readonly BusidexDao _dao = new BusidexDao();
        public AdminRepository(IBusidexDataContext busidexDal)
            : base(busidexDal)
        {
        }

        public Tag GetTag(TagType type, string tag)
        {
            return _dao.GetTag(type, tag);
        }

        public List<Tag> GetSystemTags()
        {
            return _dao.GetSystemTags();
        }

        public List<ApplicationError> GetApplicationErrors(int daysBack)
        {
            return _dao.GetApplicationErrors(daysBack);
        }

        public List<BusidexUser> GetAllBusidexUsers()
        {
            
            return _dao.GetAllBusidexUsers();
        }

        public List<BusidexUser> GetCardOwners()
        {
            return _dao.GetCardOwners();
        }

        public List<UnownedCard> GetAllUnownedCards()
        {
            return BusidexDAL.GetAllUnownedCards();
        }

        public Dictionary<string, int> GetPopularTags()
        {
            return _dao.GetPopularTags();
        }

        public void AddTag(string text, int type)
        {
            _dao.AddTag(text, type); 
        }

        public List<DeviceSummary> GetDeviceSummary()
        {
            return _dao.GetDeviceSummary();
        }

        public List<DeviceDetail> GetDeviceDetails()
        {
            return _dao.GetDeviceDetails();
        }
    }
}
