using System.Collections.Generic;
using Busidex.DomainModels;

namespace Busidex.Services.Interfaces
{
    public interface IAdminRepository
    {

        //List<Category> GetAllCategories(HttpContextBase context);
        //void AddCategory( HttpContextBase ctx, Category c );
        //void EditCategory(int categoryId, string name);
        Tag GetTag(TagType type, string tag);
        void AddTag(string text, int type);
        List<Tag> GetSystemTags();
        List<BusidexUser> GetAllBusidexUsers();
        List<BusidexUser> GetCardOwners();
        List<UnownedCard> GetAllUnownedCards();
        Dictionary<string, int> GetPopularTags();
        List<ApplicationError> GetApplicationErrors(int daysBack);
        List<DeviceSummary> GetDeviceSummary();
        List<DeviceDetail> GetDeviceDetails();
    }
}
