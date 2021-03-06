﻿using System.Collections.Generic;

namespace Busidex.Api.DataAccess.DTO
{
    public class MobileSearchResultModel
    {
        public long? UserId { get; set; }
        public string Criteria { get; set; }
        //public string Category { get; set; }
        public string SearchText { get; set; }
        public string SearchAddress { get; set; }
        public int SearchLocation { get; set; }
        public List<CardDetailModel> Results { get; set; }
        public bool IsLoggedIn { get; set; }
        public bool HasResults { get; set; }
        public ViewType Display { get; set; }
        public int Distance { get; set; }
        public Dictionary<string, int> TagCloud { get; set; }
    }
}
