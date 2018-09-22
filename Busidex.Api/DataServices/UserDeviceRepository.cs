using Busidex.Api.DataAccess;
using Busidex.Api.DataAccess.DTO;
using Busidex.Api.DataServices.Interfaces;
using Busidex.Api.Models;

namespace Busidex.Api.DataServices
{
    public class UserDeviceRepository : RepositoryBase, IUserDeviceRepository
    {
        public UserDeviceRepository(IBusidexDataContext busidexDal) : base(busidexDal)
        {
        }

        public UserDevice GetUserDevice(long userId, DeviceType deviceType)
        {
            return _dao.GetUserDevice(userId, deviceType);
        }

        public void UpdateUserDevice(UserDevice device)
        {
            _dao.UpdateUserDevice(device);
        }
    }
}