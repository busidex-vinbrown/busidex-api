using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Busidex.Api.DataAccess.DTO
{
    [Serializable]
    public class ExternalLink
    {
        public int ExternalLinkId { get; set; }
        public long CardId { get; set; }
        public string Link { get; set; }
        public int ExternalLinkTypeId { get; set; }
        public virtual ExternalLinkType ExternalLinkType { get; set; }
    }
}