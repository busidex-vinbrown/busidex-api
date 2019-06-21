using System;

namespace Busidex.DomainModels
{
    public class UserAuth
    {
        public long UserId { get; set; }
        public Guid Token { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
    }
}