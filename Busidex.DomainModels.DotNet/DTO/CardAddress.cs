using System;
using System.Data;

namespace Busidex.DomainModels.DotNet.DTO
{
    [Serializable]
    public class CardAddress : ModelBase
    {
        public long CardAddressId { get; set; }
        public long CardId { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public StateCode State { get; set; }
        public int StateCodeId { get; set; }
        public string ZipCode { get; set; }
        public string Region { get; set; }
        public string Country { get; set; }
        public bool Deleted { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public override string ToString()
        {
            return $"{Address1} {Address2} {Environment.NewLine} {City}, {State?.Code} {ZipCode} {Region} {Country}";
        }

        public CardAddress() { }
        public CardAddress(IDataReader reader)
        {
            CardAddressId = ConvertValue<long>(reader["CardAddressId"]);
            CardId = ConvertValue<long>(reader["CardId"]);
            Address1 = ConvertValue<string>(reader["Address1"]);
            Address2 = ConvertValue<string> (reader["Address2"]);
            City = ConvertValue<string>(reader["City"]);
            State = new StateCode
            {
                StateCodeId = ConvertValue<int>(reader["StateCodeId"]),
                Code = ConvertValue<string>(reader["StateCode"]),
                Name = ConvertValue<string>(reader["StateName"])
            };
            StateCodeId = ConvertValue<int>(reader["StateCodeId"]);
            ZipCode = ConvertValue<string>(reader["ZipCode"]);
            Region = ConvertValue<string>(reader["Region"]);
            Country = ConvertValue<string>(reader["Country"]);
            Deleted = ConvertValue<bool>(reader["Deleted"]);
        }
    }
}
