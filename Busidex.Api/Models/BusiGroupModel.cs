using System.Collections.Generic;
using Busidex.Api.DataAccess.DTO;

namespace Busidex.Api.Models
{
    public class BusiGroupModel
    {
        public List<GroupCard> BusigroupCards { get; set; }
        public Group Busigroup { get; set; }
    }
}