

using System.Data;

namespace Busidex.DomainModels.DotNet.DTO
{
    public class Tag
    {
        public long TagId { get; set; }
        public string Text { get; set; }
        public bool Deleted { get; set; }
        public TagType TagType { get; set; }
        public int TagTypeId { get; set; }

        public Tag() { }
        public Tag(IDataReader reader)
        {
            TagId = (long)reader["TagId"];
            Text = (string)reader["text"];
            Deleted = (bool)reader["Deleted"];
            int.TryParse(reader["TagTypeId"]?.ToString(), out int tagTypeId);
            if(tagTypeId > 0)
            {
                TagType = (TagType)tagTypeId;
            }
            TagTypeId = tagTypeId;

        }
    }
}
