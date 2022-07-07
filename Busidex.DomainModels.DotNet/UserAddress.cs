
namespace Busidex.DomainModels.DotNet
{
    
    public class UserAddress
    {
        public long UserAddressId { get; set; }
        public long UserId { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Region { get; set; }
        public string Country { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(Address1) && string.IsNullOrEmpty(Address2) && string.IsNullOrEmpty(City) &&
                string.IsNullOrEmpty(State)
                && string.IsNullOrEmpty(ZipCode) && string.IsNullOrEmpty(Region) && string.IsNullOrEmpty(Country))
            {
                return string.Empty;
            }
            return string.Join(" ", new string[] { Address1, Address2, City, State, ZipCode, Region, Country });
        }
    }
}
