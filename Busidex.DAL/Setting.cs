

namespace Busidex.DAL
{
    using System;

    public class Setting
    {
        public long SettingsId { get; set; }
        public long UserId { get; set; }
        public int? StartPage { get; set; }
        public DateTime Updated { get; set; }
        public bool AllowGoogleSync { get; set; }
        public bool Deleted { get; set; }

        //public virtual busidex_Users busidex_Users { get; set; }
        //public virtual Page Page { get; set; }
    }
}
