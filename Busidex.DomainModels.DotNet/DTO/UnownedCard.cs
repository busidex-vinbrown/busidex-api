﻿using System;

namespace Busidex.DomainModels.DotNet.DTO
{
    [Serializable]
    public class UnownedCard : Card
    {
        public DateTime? LastContactDate { get; set; }
        public string EmailSentTo { get; set; }
    }
}