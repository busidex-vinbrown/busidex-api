using System;

namespace Busidex.Api.DataAccess.DTO
{
    public class Notification<T>
    {
        public long NotificationId { get; set;  }
        public long SendTo { get; set; }
        public DateTime NotificationDate { get; set; }
        public T Payload { get; set; }
    }
}