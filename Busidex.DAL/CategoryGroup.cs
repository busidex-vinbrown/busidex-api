using System.Collections.Generic;

namespace Busidex.DAL {
    public class CategoryGroup {
        public CardCategory Category { get; set; }
        public List<Card> Cards { get; set; }

        public CategoryGroup(CardCategory category, IEnumerable<Card> cards) {
            Initialize();
            Category = category;
            foreach (Card card in cards) {
                Cards.Add(card);
            }
        }

        public CategoryGroup() {
            Initialize();
        }

        private void Initialize() {
            Cards = new List<Card>();
        }
    }
}
