using System.Collections.Generic;

namespace Busidex.DomainModels
{
    public class AdminCommunication
    {
        public EmailTemplate Template { get; set; }
        public List<string> SendTo { get; set; }
        public long UserId { get; set; }
    }
}