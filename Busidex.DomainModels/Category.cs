
namespace Busidex.DomainModels
{
    public class Category
    {

        public int CategoryId { get; set; }
        public string Name { get; set; }
        public bool Deleted { get; set; }

        //public virtual ICollection<CardCategory> CardCategories { get; set; }
    }
}
