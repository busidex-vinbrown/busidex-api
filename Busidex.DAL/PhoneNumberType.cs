

namespace Busidex.DAL
{
    public class PhoneNumberType
    {
        //public PhoneNumberType()
        //{
        //    this.PhoneNumbers = new HashSet<PhoneNumber>();
        //}

        public int PhoneNumberTypeId { get; set; }
        public string Name { get; set; }
        public bool Deleted { get; set; }

        //public virtual ICollection<PhoneNumber> PhoneNumbers { get; set; }
        public override string ToString()
        {
            return Name;
        }
    }
}
