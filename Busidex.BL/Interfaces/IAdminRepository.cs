using System.Collections.Generic;
using System.Web;
using Busidex.DAL;

namespace Busidex.BL.Interfaces
{
    public interface IAdminRepository
    {

        //List<Category> GetAllCategories(HttpContextBase context);
        //void AddCategory( HttpContextBase ctx, Category c );
        //void EditCategory(int categoryId, string name);
        List<BusidexUser> GetAllBusidexUsers();
        List<Card> GetAllUnownedCards();
    }
}
