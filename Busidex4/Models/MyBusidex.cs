using System.Collections.Generic;
using Busidex.DAL;

namespace Busidex4.Models
{
    public class MyBusidex
    {
        public List<UserCard> Busidex { get; set; }
        public string CurrentView { get; set; }
        public Dictionary<int, string> Categories { get; set; }
        public bool IsLoggedIn { get; set; }
        public int CardCount { get; set; }
        public Dictionary<string, int> TagCloud { get; set; }
    }
}