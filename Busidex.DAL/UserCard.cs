using System;
using System.Collections.Generic;

namespace Busidex.DAL
{

    [Serializable]
    public class UserCard
    {

        public long UserCardId { get; set; }
        public long CardId { get; set; }
        public long UserId { get; set; }
        public long? OwnerId { get; set; }
        public DateTime Created { get; set; }
        public bool Deleted { get; set; }
        public string Notes { get; set; }
        public long? SharedById { get; set; }
        public bool Selected { get; set; }
        public bool MobileView { get; set; }

        public Card Card { get; set; }
        public List<CardRelation> RelatedCards { get; set; }

        public UserCard()
        {
            if (RelatedCards == null)
            {
                RelatedCards = new List<CardRelation>();
            }
        }

        public UserCard(long cardId, long userId)
        {
            CardId = cardId;
            UserId = userId;
        }

        public UserCard(Card card, long userId)
        {
            Card = card;
            UserId = userId;
        }
    }
}