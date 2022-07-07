
using System.Data;

namespace Busidex.DomainModels.DotNet.DTO
{
    public class StateCode : ModelBase
    {
        public int StateCodeId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public StateCode() { }
        public StateCode(IDataReader reader)
        {
            StateCodeId = ConvertValue<int>(reader["StateCodeId"]); ;
            Code = ConvertValue<string>(reader["Code"]); ;
            Name = ConvertValue<string>(reader["Name"]); ;
        }
    }
}
