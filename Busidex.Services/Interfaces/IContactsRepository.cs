using System.Collections.Generic;
using System.Web;
using Busidex.DomainModels;

namespace Busidex.Services.Interfaces
{
    public interface IContactsRepository
    {
        List<ContactData> ReadContacts(HttpPostedFile data);
    }
}