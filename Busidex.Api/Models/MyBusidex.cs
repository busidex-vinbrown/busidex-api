using System.Collections.Generic;
using Busidex.Api.DataAccess.DTO;

namespace Busidex.Api.Models
{
    public class MyBusidex
    {
        public List<UserCard> Busidex { get; set; }
        public bool IsLoggedIn { get; set; }
        public int CardCount { get; set; }
        public Dictionary<string, int> TagCloud { get; set; }
        public string ImageCDNPath { get;set; }
    }
}