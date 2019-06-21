using System.Collections.Generic;

namespace Busidex.Services {
    public class CardComparer : IEqualityComparer<DomainModels.Card> {
        public bool Equals(DomainModels.Card card_a, DomainModels.Card card_b) {
            lock (this)
            {
                if ( card_a != null && card_b != null )
                {

                    if ( card_a.CardId == card_b.CardId )
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public int GetHashCode(DomainModels.Card obj) {
            return obj.CardId.GetHashCode();
        }
    }
}
