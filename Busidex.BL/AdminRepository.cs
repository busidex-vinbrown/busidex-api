using System.Collections.Generic;
using Busidex.BL.Interfaces;
using Busidex.DAL;

namespace Busidex.BL
{
    public class AdminRepository : RepositoryBase, IAdminRepository
    {

        public AdminRepository(IBusidexDataContext busidexDal)
            : base(busidexDal)
        {
        }

        public List<BusidexUser> GetAllBusidexUsers()
        {
            return BusidexDAL.GetAllBusidexUsers();
        }

        public List<Card> GetAllUnownedCards()
        {
            return BusidexDAL.GetAllUnownedCards();
        }

        //public List<Category> GetAllCategories(HttpContextBase context)
        //{
        //    var categories = Bcp.GetFromCache(context, BusidexCacheProvider.CachKeys.Categories) as List<Category>;
        //    if (categories == null)
        //    {
        //        Bcp.UpdateCache(context, BusidexCacheProvider.CachKeys.Categories, BusidexDAL.GetAllCategories());
        //    }
        //    categories = Bcp.GetFromCache(context, BusidexCacheProvider.CachKeys.Categories) as List<Category>;

        //    return categories;
        //}

        //public void AddCategory(HttpContextBase context, Category category)
        //{
        //    BusidexDAL.AddCategory( category );
        //    Bcp.UpdateCache(context, BusidexCacheProvider.CachKeys.Categories, BusidexDAL.GetAllCategories());
        //}

        //public void EditCategory(int categoryId, string name)
        //{
        //    var c = BusidexDAL.GetCategoryById(categoryId);
        //    if (c != null)
        //    {
        //        c.Name = name;
        //        BusidexDAL.SubmitChanges();
        //    }
        //}
    }
}
