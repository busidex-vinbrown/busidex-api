﻿using System;

namespace Busidex.DomainModels.DotNet.DTO
{
    [Serializable]
    public class ApplicationError
    {
        public long ApplicationErrorId { get; set; }
        public string Message { get; set; }
        public string InnerException { get; set; }
        public string StackTrace { get; set; }
        public DateTime ErrorDate { get; set; }
        public long UserId { get; set; }
    }
}