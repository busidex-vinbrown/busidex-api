using System;
using System.Data;

namespace Busidex.DomainModels.DotNet.DTO
{
    public class EventActivity : ModelBase
    {
        public long EventActivityId { get; set; }
        public int EventSourceId { get; set; }
        public string EventCode { get; set; }
        public string Description { get; set; }
        public long CardId { get; set; }
        public long? UserId { get; set; }
        public DateTime? ActivityDate { get; set; }

        public EventActivity() { }
        public EventActivity(IDataReader reader)
        {
            EventActivityId = ConvertValue<long>(reader["EventActivityId"]); 
            EventSourceId = ConvertValue<int>(reader["EventSourceId"]);
            EventCode = ConvertValue<string>(reader["EventCode"]);
            Description = ConvertValue<string>(reader["Description"]);
            CardId = ConvertValue<long>(reader["CardId"]);
            UserId = ConvertValue<long?>(reader["UserId"]);
            ActivityDate = ConvertValue<DateTime?>(reader["ActivityDate"]);
        }
    }
}