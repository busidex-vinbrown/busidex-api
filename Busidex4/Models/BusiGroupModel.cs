using System.Collections.Generic;
using Busidex.DAL;

namespace Busidex4.Models
{
    public class BusiGroupModel
    {
        public List<UserGroupCard> BusigroupCards { get; set; }
        public Group Busigroup { get; set; }
    }
}