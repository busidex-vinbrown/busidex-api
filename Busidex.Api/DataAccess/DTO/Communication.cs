using System;

namespace Busidex.Api.DataAccess.DTO
{
    public class Communication
    {
        public long CommunicationId { get; set; }
        public int EmailTemplateId { get; set; }
        public long UserId { get; set; }
        public string Email { get; set; }
        public string Body { get; set; }
        public DateTime DateSent { get; set; }
        public bool Failed { get; set; }
        public EmailTemplate EmailTemplate { get; set; }
        public long SentById { get; set; }
        public Guid? OwnerToken { get; set; }
    }
}
