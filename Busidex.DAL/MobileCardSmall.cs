﻿using System;
using System.Collections.Generic;
using System.Data.Linq;

namespace Busidex.DAL
{
    [Serializable]
    public class MobileCardSmall
    {
        public string FrontFileId { get; set; }
        public string Name { get; set; }
        public List<string> PhoneNumbers { get; set; }
        public string Email { get; set; }
        public string TagList { get; set; }
        public string Company { get; set; }
        public long? OwnerId { get; set; }
    }
}
