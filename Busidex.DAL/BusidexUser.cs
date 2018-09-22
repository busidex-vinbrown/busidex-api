using System.Collections.Generic;

namespace Busidex.DAL
{
    public class BusidexUser
    {
        public UserAddress Address { get; set; }
        public Setting Settings { get; set; }
        public string Email { get; set; }

        public System.Guid ApplicationId { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; }
        public string LoweredUserName { get; set; }
        public string MobileAlias { get; set; }
        public bool IsAnonymous { get; set; }
        public System.DateTime LastActivityDate { get; set; }

        public virtual ICollection<Card> Cards { get; set; }
        public virtual ICollection<UserAccount> UserAccounts { get; set; }
        public virtual ICollection<UserCard> UserCards { get; set; }
        public virtual ICollection<UserCard> UserCards1 { get; set; }

        public void OnCreated()
        {
            Settings = new Setting();
        }
    }
}
