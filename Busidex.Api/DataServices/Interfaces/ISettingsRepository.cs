using System.Collections.Generic;
using System.Threading.Tasks;
using Busidex.Api.DataAccess.DTO;

namespace Busidex.Api.DataServices.Interfaces
{
    public interface ISettingsRepository
    {

        BusidexUser GetBusidexUserById(long userId);
        IEnumerable<Page> GetAllSitePages();
        Setting AddDefaultUserSetting(BusidexUser user);
        Setting GetUserSetting(BusidexUser user);
        Setting SaveSetting(Setting setting);
        void UpdateSetting(Setting setting);
        Task<List<SystemSettingDto>> GetSystemSettings();
    }
}
