namespace Busidex.Api.DataAccess.DTO
{
    public class CardAddressDTO
    {
        public long CardAddressId { get; set; }

        public long CardId { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string ZipCode { get; set; }

        public string Region { get; set; }

        public string Country { get; set; }

        public bool Deleted { get; set; }
    }
}
