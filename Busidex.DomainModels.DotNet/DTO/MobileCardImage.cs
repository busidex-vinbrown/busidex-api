using System;


namespace Busidex.DomainModels.DotNet.DTO
{
    public class MobileCardImage
    {
        public enum SideIndex
        {
            Front = 0,
            Back = 1
        }

        public Guid? FrontFileId { get; set; }
        public Guid? BackFileId { get; set; }
        public string Orientation { get; set; }

        
        public string EncodedCardImage { get; set; }

        public SideIndex Side { get; set; }
    }
}