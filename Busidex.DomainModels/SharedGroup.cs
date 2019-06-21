using System;

namespace Busidex.DomainModels
{
    [Serializable]
    public class SharedGroup
    {
        public long SharedGroupId { get; set; }
        public long GroupId { get; set; }
        public long SendFrom { get; set; }
        public string Email { get; set; }
        public long? ShareWith { get; set; }
        public DateTime SharedDate { get; set; }
        public bool Accepted { get; set; }
        public bool Declined { get; set; }

        public virtual Group Group { get; set; }
    }
}
