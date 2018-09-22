using System.Collections;
using System.Collections.Generic;
using Busidex.DAL;

namespace Busidex.BL {
    public class CardComparer : IEqualityComparer<Card> {
        public bool Equals(Card card_a, Card card_b) {
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

        public int GetHashCode(Card obj) {
            return obj.CardId.GetHashCode();
        }
    }
}
