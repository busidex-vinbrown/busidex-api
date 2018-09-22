using System;
using System.Collections.Generic;
using Busidex.Api.DataAccess.DTO;

namespace Busidex.Api.Models
{
    [Serializable]
    public class SharedCardDTO
    {
        public SharedCard[] SharedCards { get; set; } 
    }
}