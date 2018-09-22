namespace Busidex.Api.Models
{
    public class EventSource
    {
        public int EventSourceId { get; set; }
        public string EventCode { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
    }
}