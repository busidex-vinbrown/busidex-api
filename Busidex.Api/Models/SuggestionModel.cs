using Busidex.Api.DataAccess.DTO;

namespace Busidex.Api.Models
{
    public class SuggestionModel
    {
        public Suggestion Suggestion { get; set; }
        public long UserId { get; set; }
    }
}