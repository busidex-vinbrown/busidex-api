using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Busidex.DAL;

namespace Busidex4.Models
{
    public class AddGroupModel
    {
        public Group Busigroup { get; set; }
        public List<UserCard> Busidex { get; set; }
        public Dictionary<string, int> TagCloud { get; set; }
    }
}