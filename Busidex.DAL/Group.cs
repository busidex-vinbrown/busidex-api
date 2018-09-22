using System;

namespace Busidex.DAL
{
    [Serializable]
    public class Group
    {
        public long GroupId { get; set; }
        public long UserId { get; set; }
        public string Description { get; set; }
        public string Notes { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public bool Deleted { get; set; }
    }
}
