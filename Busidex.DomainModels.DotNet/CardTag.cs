namespace Busidex.DomainModels.DotNet
{
    public class CardTag
    {
        public long CardId { get; set; }
        public long TagId { get; set; }
        public string Text { get; set; }
        public int TagTypeId { get; set; }
    }
}