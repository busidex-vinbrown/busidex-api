﻿namespace Busidex.Profile.DataAccess
{
    public class PhoneNumberType
    {
        public int PhoneNumberTypeId { get; set; }
        public string Name { get; set; }
        public bool Deleted { get; set; }
        public override string ToString()
        {
            return Name;
        }
    }
}
