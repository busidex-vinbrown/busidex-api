using System.Collections.Generic;
using Busidex.DAL;

namespace Busidex.BL.Interfaces
{
    public interface ISettingsRepository
    {

        BusidexUser GetBusidexUserById(long userId);
        IEnumerable<Page> GetAllSitePages();
        Setting AddDefaultUserSetting(BusidexUser user);
        Setting GetUserSetting(BusidexUser user);
        Setting SaveSetting(Setting setting);
        void UpdateSetting(Setting setting);
    }
}
