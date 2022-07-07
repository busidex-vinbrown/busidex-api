﻿using System;

namespace Busidex.DomainModels.DotNet.DTO
{
    [Serializable]
    public class EventTag
    {
        public long EventTagId { get; set; }
        public long TagId { get; set; }
        public string Text { get; set; }
        public string Description { get; set; }
    }
}
