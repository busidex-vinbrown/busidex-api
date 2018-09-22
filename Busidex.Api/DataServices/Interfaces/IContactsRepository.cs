using System.Collections.Generic;
using System.Web;
using Busidex.Api.DataAccess.DTO;

namespace Busidex.Api.DataServices.Interfaces
{
    public interface IContactsRepository
    {
        List<ContactData> ReadContacts(HttpPostedFile data);
    }
}