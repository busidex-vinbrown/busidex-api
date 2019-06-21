using Busidex.DomainModels;
using Busidex.DomainModels.DTO;

namespace Busidex.Services.Interfaces
{
    public interface IUserDeviceRepository
    {
        UserDevice GetUserDevice(long userId, DeviceType deviceType );
        void UpdateUserDevice(UserDevice device);
    }
}