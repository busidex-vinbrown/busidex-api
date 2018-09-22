using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Busidex.Api.DataAccess.DTO
{
    [Serializable]
    [MetadataType(typeof(Card_Validation))]
    public class AddOrEditCardModel : ViewModelBase
    {
        public enum CardAction
        {
            Add,
            Edit,
            ImageOnly
        }

        public CardAction Action { get; set; }
        public long CardId { get; set; }
        public long CreatedBy { get; set; }
        public long UserId { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public byte[] FrontImage { get; set; }
        public string FrontType { get; set; }
        public string FrontOrientation { get; set; }
        public byte[] BackImage { get; set; }
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
        public DisplayType Display { get; set; }
        public string Markup { get; set; }
        public byte Visibility { get; set; }
        public CardType CardType { get; set; }
        public bool UpdateFrontImage { get; set; }
        public bool UpdateBackImage { get; set; }
        public bool ResetBackImage { get; set; }
        public long? SelectExistingCardId { get; set; }
        public string CustomContent { get; set; }

        public AddOrEditCardModel()
        {
            ModelErrors = new AddOrUpdateCardErrors();
            Display = DisplayType.IMG;
            PhoneNumbers = new List<PhoneNumber>();
        }

        public AddOrEditCardModel(CardDetailModel model)
        {
            CardId = model.CardId;
            CreatedBy = model.CreatedBy;
            UserId = model.OwnerId.GetValueOrDefault();
            Name = model.Name;
            Title = model.Title;
            FrontType = model.FrontType;
            FrontOrientation = model.FrontOrientation;
            BackType = model.BackType;
            BackOrientation = model.BackOrientation;
            BusinessId = model.BusinessId;
            Searchable = model.Searchable;
            Email = model.Email;
            Url = model.Url;
            CompanyName = model.CompanyName;
            PhoneNumbers = new List<PhoneNumber>(model.PhoneNumbers);
            IsMyCard = true;
            FrontFileId = model.FrontFileId;
            BackFileId = model.BackFileId;
            OwnerId = model.OwnerId;
            OwnerToken = model.OwnerToken;
            Tags = new List<Tag>(model.Tags);
            Addresses = new List<CardAddress>(model.Addresses);
            Display = model.Display;
            Markup = model.Markup;
            Visibility = model.Visibility;
            CardType = DTO.CardType.Professional;
            CustomContent = model.CustomContent;
        }

        public AddOrEditCardModel(OrgCardDetailModel model)
        {
            CardId = model.CardId;
            CreatedBy = model.CreatedBy;
            UserId = model.OwnerId.GetValueOrDefault();
            Name = model.Name;
            Title = model.Title;
            FrontType = model.FrontType;
            FrontOrientation = model.FrontOrientation;
            BackType = model.BackType;
            BackOrientation = model.BackOrientation;
            BusinessId = model.BusinessId;
            Searchable = model.Searchable;
            Email = model.Email;
            Url = model.Url;
            CompanyName = model.CompanyName;
            PhoneNumbers = new List<PhoneNumber>(); // these cards have no phone numbers
            IsMyCard = true;
            FrontFileId = model.FrontFileId;
            BackFileId = model.BackFileId;
            OwnerId = model.OwnerId;
            OwnerToken = model.OwnerToken;
            Tags = new List<Tag>();
            Addresses = new List<CardAddress>(); // these cards have no addresses
            Display = model.Display;
            Markup = model.Markup;

            byte visibility = 1;
            switch (model.Visibility)
            {
                case 1:
                    visibility = (byte)1;
                    break;
                case 2:
                    visibility = (byte)2;
                    break;
                case 3:
                    visibility = (byte)3;
                    break;
            }
            Visibility = visibility;

            CardType = DTO.CardType.Professional;
            CustomContent = model.CustomContent;
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