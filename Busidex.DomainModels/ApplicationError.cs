
using System;

namespace Busidex.DomainModels
{
    public class ApplicationError
    {
        public string Message { get; set; }
        public string InnerException { get; set; }
        public string StackTrace { get; set; }
        public DateTime? ErrorDate { get; set; }
        public long? UserId { get; set; }
    }
}