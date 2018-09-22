using System.Collections.Generic;
using Busidex.BL.Interfaces;
using Busidex.DAL;

namespace Busidex.BL
{
    public class SettingsRepository : RepositoryBase, ISettingsRepository
    {
        public SettingsRepository(IBusidexDataContext busidexDal)
            : base(busidexDal)
        {
        }

        public IEnumerable<Page> GetAllSitePages()
        {
            return BusidexDAL.GetAllSitePages();
        }

        public Setting AddDefaultUserSetting(BusidexUser user)
        {
            return BusidexDAL.AddDefaultUserSetting(user);
        }

        public Setting GetUserSetting(BusidexUser user)
        {
            return BusidexDAL.GetUserSetting(user);
        }

        public Setting SaveSetting(Setting setting)
        {
            return BusidexDAL.SaveSetting(setting);
        }

        public void UpdateSetting(Setting setting)
        {
            BusidexDAL.UpdateSetting(setting);
        }
    }
}
