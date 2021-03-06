

using System.Security.Policy;

namespace Busidex.DAL
{
    using System;
    [Serializable]
    public class SharedCard
    {
        public long SharedCardId { get; set; }
        public long CardId { get; set; }
        public long SendFrom { get; set; }
        public string SendFromEmail { get; set; }
        public string Email { get; set; }
        public long? ShareWith { get; set; }
        public DateTime SharedDate { get; set; }
        public bool? Accepted { get; set; }
        public bool? Declined { get; set; }

        public virtual Card Card { get; set; }
    }
}
