using Busidex.DataAccess;
using Busidex.DomainModels;
using Busidex.DomainModels.DTO;
using Busidex.Services.Interfaces;

namespace Busidex.Services
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