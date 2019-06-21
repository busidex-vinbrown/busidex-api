
using System;

namespace Busidex.DomainModels
{
    public class EmailAddress
    {
        public int EmailAddressId { get; set; }
        public int BusinessId { get; set; }
        public string Email { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public bool Deleted { get; set; }

        //public virtual Business Business { get; set; }
    }
}
