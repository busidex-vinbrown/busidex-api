using System;
using System.Collections.Generic;
using System.Data;

namespace Busidex.DomainModels.DotNet.DTO
{

    [Serializable]
    public class UserCard : ModelBase
    {

        public long UserCardId { get; set; }
        public long CardId { get; set; }
        public long UserId { get; set; }
        public long? OwnerId { get; set; }
        public DateTime Created { get; set; }
        public bool Deleted { get; set; }
        public string? Notes { get; set; }
        public long? SharedById { get; set; }
        public bool Selected { get; set; }
        public bool MobileView { get; set; }

        public Card Card { get; set; }
        public List<CardRelation> RelatedCards { get; set; }

        public UserCard(IDataReader reader)
        {
            UserCardId = (long)reader["UserCardId"];
            CardId = (long)reader["CardId"];
            UserId = (long)reader["UserId"];
            OwnerId = ConvertValue<long?>(reader["OwnerId"]);
            Notes = ConvertValue<string>(reader["Notes"]);
            Deleted = (bool)reader["Deleted"];
            DateTime.TryParse(reader["Created"].ToString(), out DateTime dt);
            Created = dt;
            SharedById = reader["SharedById"] == DBNull.Value ? null : (long)reader["SharedById"];

        }

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