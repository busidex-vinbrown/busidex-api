using System;

namespace Busidex.DomainModels.DTO
{
    [Serializable]
    public class ExternalLinkType
    {
        public int ExternalLinkTypeId { get; set; }
        public string LinkType { get; set; }
    }
}