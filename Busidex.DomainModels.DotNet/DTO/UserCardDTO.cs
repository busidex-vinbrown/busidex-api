using System;
using System.Data;

namespace Busidex.DomainModels.DotNet.DTO
{

    [Serializable]
    public class UserCardDTO
    {

        public long UserCardId { get; set; }
        public long CardId { get; set; }
        public long UserId { get; set; }
        public long? OwnerId { get; set; }
        public DateTime Created { get; set; }
        public bool Deleted { get; set; }
        public string? Notes { get; set; }
        public long? SharedById { get; set; }

        public UserCardDTO()
        {

        }

        public UserCardDTO(IDataReader reader){
            UserId = (long)reader["UserId"];
            OwnerId = (long)reader["OwnerId"];
            DateTime.TryParse(reader["Created"].ToString(), out DateTime dt);
            Created = dt;
            Deleted = (bool)reader["Deleted"];
            Notes = (string)reader["Notes"];
            SharedById = (long)reader["SharedById"];
        }


        public UserCardDTO(long cardId, long userId)
        {
            CardId = cardId;
            UserId = userId;
        }
    }
}