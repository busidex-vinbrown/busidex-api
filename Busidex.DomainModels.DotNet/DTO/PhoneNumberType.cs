using System;
using System.Data;

namespace Busidex.DomainModels.DotNet.DTO
{
    [Serializable]
    public class PhoneNumberType
    {
        public int PhoneNumberTypeId { get; set; }
        public string Name { get; set; }
        public bool Deleted { get; set; }
        public override string ToString()
        {
            return Name;
        }

        public PhoneNumberType() { }
        public PhoneNumberType(IDataReader reader)
        {
            PhoneNumberTypeId = (int)reader["PhoneNumberTypeId"];
            Name = (string)reader["Name"];
            Deleted = (bool)reader["Deleted"];
        }
    }
}
