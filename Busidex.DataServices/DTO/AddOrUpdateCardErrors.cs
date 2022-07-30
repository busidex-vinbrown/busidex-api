using System.Collections.Generic;

namespace Busidex.DataServices.DTO
{
    public class AddOrUpdateCardErrors : ModelErrorsBase
    {

        public List<Card> ExistingCards { get; set; }
        public Card CardDTO { get; set; }

        public AddOrUpdateCardErrors()
        {

            ExistingCards = new List<Card>();
            ErrorCollection = ErrorCollection ?? new Dictionary<string, string>();
        }
    }
}
