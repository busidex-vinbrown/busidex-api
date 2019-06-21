using System;

namespace Busidex.DomainModels.DTO
{
    public class EventActivity
    {
        public long EventActivityId { get; set; }
        public int EventSourceId { get; set; }
        public string EventCode { get; set; }
        public string Description { get; set; }
        public long CardId { get; set; }
        public long? UserId { get; set; }
        public DateTime? ActivityDate { get; set; }
    }
}