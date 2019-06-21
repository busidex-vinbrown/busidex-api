using System;
using System.Collections.Generic;
using System.Data.Linq;

namespace Busidex.DomainModels
{
    [Serializable]
    public class MobileCard
    {
        public Binary FrontImage { get; set; }
        public string FrontFileId { get; set; }
        public string FrontImageType { get; set; }
        public string BackFileId { get; set; }
        public string BackImageType { get; set; }
        public long CardId { get; set; }
        public string Name { get; set; }
        public List<string> PhoneNumbers { get; set; }
        public string Email { get; set; }
        public string TagList { get; set; }
        public string Company { get; set; }
        public bool ExistsInMyBusidex { get; set; }
        public string CustomContent { get; set; }
    }
}
