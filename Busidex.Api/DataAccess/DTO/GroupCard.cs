using System;

namespace Busidex.Api.DataAccess.DTO
{
    [Serializable]
    public class GroupCard
    {

        public long GroupCardId { get; set; }
        public long? CardId { get; set; }
        public long GroupId { get; set; }
        public DateTime Created { get; set; }
        public bool Deleted { get; set; }
        public string Notes { get; set; }
        public long? SharedById { get; set; }
        public Card Card { get; set; }

        public GroupCard()
        {
        }

        public GroupCard(long cardId)
        {
            CardId = cardId;
        }

        public GroupCard(Card card)
        {
            Card = card;
        }
    }
}
