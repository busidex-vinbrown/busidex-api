using Busidex.Api.DataAccess.DTO;

namespace Busidex.Api.DataServices.Interfaces
{
    public interface ISMSShareRepository
    {
        void SaveSmsShare(SMSShare data);
    }
}