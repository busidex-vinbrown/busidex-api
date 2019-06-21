using System;

namespace Busidex.DomainModels
{
    [Serializable]
    public class UserGroupCard
    {

        public long UserGroupCardId { get; set; }
        public long? CardId { get; set; }
        public long GroupId { get; set; }
        public long UserId { get; set; }
        public long? PersonId { get; set; }
        public DateTime Created { get; set; }
        public bool Deleted { get; set; }
        public string Notes { get; set; }
        public long? SharedById { get; set; }
        public Card Card { get; set; }

        public UserGroupCard()
        {
        }

        public UserGroupCard(long cardId, long userId)
        {
            CardId = cardId;
            UserId = userId;
        }

        public UserGroupCard(Card card, long userId)
        {
            Card = card;
            UserId = userId;
        }
    }
}
