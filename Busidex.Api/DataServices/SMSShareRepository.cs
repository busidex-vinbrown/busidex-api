using Busidex.Api.DataAccess;
using Busidex.Api.DataAccess.DTO;
using Busidex.Api.DataServices.Interfaces;

namespace Busidex.Api.DataServices
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