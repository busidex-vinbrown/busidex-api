using System.Collections.Generic;

namespace Busidex.DomainModels.DTO
{
    public class SettingsModel
    {
        public IEnumerable<Page> SitePages { get; set; }
        public Setting CurrentSetting { get; set; }
        public long UserId { get; set; }
    }
}