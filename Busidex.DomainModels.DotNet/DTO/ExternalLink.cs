using System;
using System.Data;

namespace Busidex.DomainModels.DotNet.DTO
{
    [Serializable]
    public class ExternalLink : ModelBase
    {
        public int ExternalLinkId { get; set; }
        public long CardId { get; set; }
        public string Link { get; set; }
        public int ExternalLinkTypeId { get; set; }
        public virtual ExternalLinkType ExternalLinkType { get; set; }

        public ExternalLink() { }
        public ExternalLink(IDataReader reader)
        {
            ExternalLinkId = ConvertValue<int>(reader["ExternalLinkId"]);
            CardId = ConvertValue<long>(reader["CardId"]); ;
            Link = ConvertValue<string>(reader["Link"]);
            ExternalLinkTypeId = ConvertValue<int>(reader["ExternalLinkTypeId"]);
            ExternalLinkType = new ExternalLinkType
            {
                ExternalLinkTypeId = ConvertValue<int>(reader["ExternalLinkTypeId"]),
                LinkType = ConvertValue<string>(reader["LinkType"])
            };
        }
    }
}