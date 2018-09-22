
namespace Busidex.Api.DataAccess.DTO
{
    public class OrganizationGuest
    {
        public long UserCardId { get; set; }
        public string Email { get; set; }
        public UserCardAddStatus AddStatus { get; set; }
    }
}