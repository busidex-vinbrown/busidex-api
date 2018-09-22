

namespace Busidex.DAL
{
    using System;
    using System.Collections.Generic;

    public class Business
    {
        public Business()
        {
            Init();
        }

        public int BusinessId { get; set; }
        public string Name { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public bool Deleted { get; set; }

        public virtual ICollection<Card> Cards { get; set; }
        public virtual ICollection<EmailAddress> EmailAddresses { get; set; }

        private void Init()
        {
            this.Cards = new HashSet<Card>();
            this.EmailAddresses = new HashSet<EmailAddress>();
        }
    }
}
