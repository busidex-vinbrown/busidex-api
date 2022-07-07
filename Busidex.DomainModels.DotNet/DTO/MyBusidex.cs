using System.Collections.Generic;

namespace Busidex.DomainModels.DotNet.DTO
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