using System.Collections.Generic;
using Busidex.Api.DataAccess.DTO;

namespace Busidex.Api.Models
{
    public class SettingsModel
    {
        public IEnumerable<Page> SitePages { get; set; }
        public Setting CurrentSetting { get; set; }
        public long UserId { get; set; }
    }
}