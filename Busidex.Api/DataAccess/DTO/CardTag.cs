using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Busidex.Api.DataAccess.DTO
{
    public class CardTag
    {
        public long CardId { get; set; }
        public long TagId { get; set; }
        public string Text { get; set; }
        public int TagTypeId { get; set; }
    }
}