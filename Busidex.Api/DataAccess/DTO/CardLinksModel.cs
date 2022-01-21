using System.Collections.Generic;

namespace Busidex.Api.DataAccess.DTO
{
    public class CardLinksModel
    {
        public int CardId { get; set; }
        public List<ExternalLink> Links { get; set; }
    }
}