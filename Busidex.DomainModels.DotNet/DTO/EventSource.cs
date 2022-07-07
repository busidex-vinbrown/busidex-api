using System.Data;

namespace Busidex.DomainModels.DotNet.DTO
{
    public class EventSource : ModelBase
    {
        public int EventSourceId { get; set; }
        public string EventCode { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }

        public EventSource() { }
        public EventSource(IDataReader reader)
        {
            EventSourceId = ConvertValue<int>(reader["EventSourceId"]); ;
            EventCode = ConvertValue<string>(reader["EventCode"]); ;
            Description = ConvertValue<string>(reader["Description"]); ;
            Active = ConvertValue<bool>(reader["Active"]); ;
        }
    }
}