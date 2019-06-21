using System;

namespace Busidex.DomainModels
{
    public class DeviceDetail
    {
        public long UserDeviceId { get; set; }
        public long? MembershipId { get; set; }
        public long UserId { get; set; }
        public int DeviceTypeId { get; set; }
        public int? Version { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime Created { get; set; }
    }
}
