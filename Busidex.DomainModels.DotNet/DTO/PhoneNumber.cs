using System;
using System.Data;

namespace Busidex.DomainModels.DotNet.DTO
{
    [Serializable]
    public class PhoneNumber : ModelBase
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
            return Number + " " + Extension;
        }

        public PhoneNumber() { }
        public PhoneNumber (IDataReader reader)
        {
            PhoneNumberId = (long)reader["PhoneNumberId"];
            PhoneNumberTypeId = (int)reader["PhoneNumberTypeId"];
            CardId = (long)reader["CardId"];
            Number = (string)reader["Number"];
            Extension = ConvertValue<string>(reader["Extension"]);
            Deleted = (bool)reader["Deleted"];

            
        }
    }
}
