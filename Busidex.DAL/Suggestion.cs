using System;

namespace Busidex.DAL
{
    public class Suggestion
    {
        public int SuggestionId { get; set; }
        public string Summary { get; set; }
        public string Details { get; set; }
        public int Votes { get; set; }
        public long CreatedBy { get; set; }
        public DateTime Created { get; set; }
        public bool Deleted { get; set; }
    }
}
