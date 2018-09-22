using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Busidex.Api.DataAccess.DTO
{
    public class AdminCommunication
    {
        public EmailTemplate Template { get; set; }
        public List<string> SendTo { get; set; }
        public long UserId { get; set; }
    }
}