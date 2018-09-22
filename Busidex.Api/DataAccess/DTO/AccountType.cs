
using System;

namespace Busidex.Api.DataAccess.DTO
{
    [Serializable]
    public class AccountType
    {
        public int AccountTypeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        public int DisplayOrder { get; set; }
    }
}
