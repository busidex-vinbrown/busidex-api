using Busidex.DataAccess;
using Busidex.DomainModels;
using Busidex.Services.Interfaces;

namespace Busidex.Services
{
    public class SmsShareRepository : RepositoryBase, ISMSShareRepository
    {
        public SmsShareRepository(IBusidexDataContext busidexDal) : base(busidexDal)
        {
        }

        public void SaveSmsShare(SMSShare data)
        {
            _dao.SaveSmsShare(data);
        }
    }
}