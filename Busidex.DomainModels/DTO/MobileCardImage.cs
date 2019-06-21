using System;
using Microsoft.Build.Framework;

namespace Busidex.DomainModels.DTO
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
        [Required]
        public string Orientation { get; set; }

        
        public string EncodedCardImage { get; set; }

        [Required]
        public SideIndex Side { get; set; }
    }
}