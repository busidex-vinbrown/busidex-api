using System;

namespace Busidex.Profile.DataAccess
{
    public class PhoneNumber
    {
        public long PhoneNumberId { get; set; }
        public int PhoneNumberTypeId { get; set; }
        public long CardId { get; set; }
        public string Number { get; set; }
        public string Extension { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public bool Deleted { get; set; }
        public PhoneNumberType PhoneNumberType { get; set; }

        public override string ToString()
        {
            return this.Number + " " + this.Extension;
        }
    }
}
