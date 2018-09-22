

namespace Busidex.DAL
{
    using System;

    public class UserAccount
    {
        public long UserAccountId { get; set; }
        public long UserId { get; set; }
        public int AccountTypeId { get; set; }
        public DateTime Created { get; set; }
        public bool Active { get; set; }
        public string Notes { get; set; }
        public Guid? ActivationToken { get; set; }

        public virtual AccountType AccountType { get; set; }
        public virtual BusidexUser BusidexUser { get; set; }
    }
}
