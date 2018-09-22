using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Linq;

namespace Busidex.DAL
{
    [MetadataType(typeof(Card_Validation))]
    public class AddOrEditCardModel : ViewModelBase
    {

        public long CardId { get; set; }
        public long CreatedBy { get; set; }

        public string Name { get; set; }
        public string Title { get; set; }
        public Binary FrontImage { get; set; }
        public string FrontType { get; set; }
        public string FrontOrientation { get; set; }
        public Binary BackImage { get; set; }
        public string BackType { get; set; }
        public string BackOrientation { get; set; }
        public int? BusinessId { get; set; }
        public bool Searchable { get; set; }
        public string Email { get; set; }
        public string Url { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public string CompanyName { get; set; }
        public bool CanHaveRelatedCards { get; set; }
        public string FileSizeLimit { get; set; }
        public string FileSizeInfoContent { get; set; }
        public List<UserCard> MyBusidex { get; set; }
        public List<PhoneNumber> PhoneNumbers { get; set; }
        public List<PhoneNumberType> PhoneNumberTypes { get; set; }
        public Dictionary<long, long> CardRelations { get; set; }
        public string ActionMethod { get; set; }
        public List<Card> ExistingCards { get; set; }
        public bool? IsMyCard { get; set; }
        public bool HasFrontImage { get; set; }
        public bool HasBackImage { get; set; }
        public string Notes { get; set; }
        public Guid? FrontFileId { get; set; }
        public Guid? BackFileId { get; set; }
        public long? OwnerId { get; set; }
        public Guid? OwnerToken { get; set; }
        public string MyEmail { get; set; }
        public List<Tag> Tags { get; set; }
        public List<CardAddress> Addresses { get; set; }
        //public List<StateCode> StateCodes { get; set; }
        public DisplayType Display { get; set; }
        public string Markup { get; set; }

        public long? SelectExistingCardId { get; set; }
        public AddOrEditCardModel()
        {
            ModelErrors = new AddOrUpdateCardErrors();
            Display = DisplayType.IMG;
            PhoneNumbers = new List<PhoneNumber>();
        }

        public void LoadImage(byte[] image, string type, bool isFront)
        {

            if (isFront)
            {
                FrontType = type.Replace(".", "");
                FrontImage = image;
            }
            else
            {
                BackType = type.Replace(".", "");
                BackImage = image;
            }
        }
    }
}