using System;

namespace Busidex.DomainModels
{
    public class PhoneNumberDTO
    {

        public long PhoneNumberId { get; set; }

        public int PhoneNumberTypeId { get; set; }

        public string Name { get; set; }

        public long CardId { get; set; }

        public string Number { get; set; }

        public string Extension { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }
    }
}