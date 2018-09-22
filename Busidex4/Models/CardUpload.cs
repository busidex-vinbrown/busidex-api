using System.Runtime.Serialization;

namespace Busidex4.Models
{
    public class CardUpload
    {
        public int Id { get; set; }

        [DataMember(Name = "text")]
        public string Text { get; set; }

        [DataMember(Name = "complete")]
        public bool Complete { get; set; }

        //Added below for blob sas generation in Mobile Services

        [DataMember(Name = "containerName")]
        public string ContainerName { get; set; }

        [DataMember(Name = "resourceName")]
        public string ResourceName { get; set; }

        public string SAS { get; set; }

    }
}
