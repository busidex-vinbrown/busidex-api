

namespace Busidex.DAL
{
    using System;

    public class FeeSchedule
    {
        public int FeeScheduleId { get; set; }
        public int AccountTypeId { get; set; }
        public decimal Amount { get; set; }
        public DateTime ActiveFrom { get; set; }
        public DateTime? ActiveUntil { get; set; }
        public bool Deteted { get; set; }

        //public virtual AccountType AccountType { get; set; }
    }
}
