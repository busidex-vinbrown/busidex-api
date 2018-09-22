

namespace Busidex.DAL
{
    public class AccountType
    {
        //public AccountType()
        //{
        //    this.FeeSchedules = new HashSet<FeeSchedule>();
        //    this.UserAccounts = new HashSet<UserAccount>();
        //}

        public int AccountTypeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        public int DisplayOrder { get; set; }

        //public virtual ICollection<FeeSchedule> FeeSchedules { get; set; }
        //public virtual ICollection<UserAccount> UserAccounts { get; set; }
    }
}
