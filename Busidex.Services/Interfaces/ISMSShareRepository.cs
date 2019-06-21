using Busidex.DomainModels;

namespace Busidex.Services.Interfaces
{
    public interface ISMSShareRepository
    {
        void SaveSmsShare(SMSShare data);
    }
}