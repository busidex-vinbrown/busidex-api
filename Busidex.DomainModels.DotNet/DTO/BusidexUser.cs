using System;

namespace Busidex.DomainModels.DotNet.DTO
{
    public class BusidexUser
    {
        public UserAddress Address { get; set; }
        public Setting Settings { get; set; }
        public string Email { get; set; }

        public Guid ApplicationId { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; }
        public string LoweredUserName { get; set; }
        public string MobileAlias { get; set; }
        public bool IsAnonymous { get; set; }
        public DateTime LastActivityDate { get; set; }

        public Guid CardFileId { get; set; }
        public string CardFileType { get; set; }
        //public virtual ICollection<Card> Cards { get; set; }
        public UserAccount UserAccount { get; set; }
        //public virtual ICollection<UserCard> UserCards { get; set; }
        //public virtual ICollection<UserCard> UserCards1 { get; set; }

        public void OnCreated()
        {
            Settings = new Setting();
        }
    }
}
