
using System;

namespace Busidex.DataServices.DTO
{
    public class CardRelation
    {
        public long CardRelationId { get; set; }
        public long CardId { get; set; }
        public long RelatedCardId { get; set; }
        public Guid RelatedCardImageId { get; set; }
        public bool Deleted { get; set; }

        //public virtual Card Card { get; set; }
        //public virtual Card Card1 { get; set; }
    }
}
