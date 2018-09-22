

namespace Busidex.DAL
{

    public class EmailTemplate
    {
        public int EmailTemplateId { get; set; }
        public string Code { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

        //public virtual ICollection<Communication> Communications { get; set; }
    }
}
