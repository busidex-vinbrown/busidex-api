using System;

namespace Busidex.DataServices.DTO
{
    [Serializable]
    public class StateCode
    {
        public int StateCodeId { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
