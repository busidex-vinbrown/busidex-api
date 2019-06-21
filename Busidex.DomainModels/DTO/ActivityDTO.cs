namespace Busidex.DomainModels.DTO
{
    public class ActivityDTO
    {
        public int EventSourceId { get; set; }
        public long CardId { get; set; }
        public long? UserId { get; set; }
    }
}