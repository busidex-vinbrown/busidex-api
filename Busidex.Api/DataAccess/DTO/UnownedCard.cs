using System;

namespace Busidex.Api.DataAccess.DTO
{
    [Serializable]
    public class UnownedCard : Card
    {
        public DateTime? LastContactDate { get; set; } 
        public string EmailSentTo { get; set; }
    }
}