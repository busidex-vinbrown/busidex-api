using System.Collections.Generic;

namespace Busidex.DomainModels.DTO
{
    public class BusiGroupModel
    {
        public List<GroupCard> BusigroupCards { get; set; }
        public Group Busigroup { get; set; }
    }
}