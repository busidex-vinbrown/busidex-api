using System.ComponentModel.DataAnnotations;

namespace Busidex.DAL
{
    public class Card_Validation
    {
        [Display(Name = "Company Name", Description = "Company Name", Prompt = "Company Name", ShortName = "Company Name")]
        public string CompanyName { get; set; }

        [Email]
        public string Email { get; set; }
    }
}
