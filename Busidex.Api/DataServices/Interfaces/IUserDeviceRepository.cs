using Busidex.Api.DataAccess.DTO;
using Busidex.Api.Models;

namespace Busidex.Api.DataServices.Interfaces
{
    public interface IUserDeviceRepository
    {
        UserDevice GetUserDevice(long userId, DeviceType deviceType );
        void UpdateUserDevice(UserDevice device);
    }
}