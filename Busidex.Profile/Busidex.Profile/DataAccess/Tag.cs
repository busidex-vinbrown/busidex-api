﻿namespace Busidex.Profile.DataAccess
{
    public class Tag
    {
        public long TagId { get; set; }
        public string Text { get; set; }
        public bool Deleted { get; set; }
        public TagType TagType { get; set; }
        public int TagTypeId { get; set; }
    }
}
