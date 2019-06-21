using System;

namespace Busidex.DomainModels
{
    [Serializable]
    public class GroupCardResult
    {

        public long GroupCardId { get; set; }
        public long? CardId { get; set; }
        public long GroupId { get; set; }
        public bool Deleted { get; set; }
        public string Notes { get; set; }
        
       public string Name{ get; set; }
      public string Title{ get; set; }
      public string FrontType{ get; set; }
      public string FrontOrientation{ get; set; }
      public string BackType{ get; set; }
      public string BackOrientation{ get; set; }
      public bool Searchable{ get; set; }
      public string CompanyName{ get; set; }
      public string Email{ get; set; }
      public string Url{ get; set; }
      public long CreatedBy{ get; set; }
      public long? OwnerId{ get; set; }
      public Guid? FrontFileId { get; set; }
      public Guid? BackFileId { get; set; }

        public GroupCardResult()
        {
        }

       
    }
}
