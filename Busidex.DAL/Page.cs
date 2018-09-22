

namespace Busidex.DAL
{

    public class Page
    {

        public int PageId { get; set; }
        public string Action { get; set; }
        public string ControllerName { get; set; }
        public string Title { get; set; }
        public bool Deleted { get; set; }

        //public virtual ICollection<Setting> Settings { get; set; }
    }
}
