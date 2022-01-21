using System;

namespace Busidex.Api.DataAccess.DTO
{
    [Serializable]
    public class ExternalLinkType
    {
        public int ExternalLinkTypeId { get; set; }
        public string LinkType { get; set; }
    }
}