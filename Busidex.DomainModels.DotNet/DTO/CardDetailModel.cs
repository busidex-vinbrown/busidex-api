using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Busidex.DomainModels.DotNet.DTO
{
    public class CardDetailModel
    {

        public long CardId { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public int? BusinessId { get; set; }
        public bool Searchable { get; set; }
        public string Email { get; set; }
        public string Url { get; set; }

        [Display(Name = @"Company Name", Description = @"Company Name", Prompt = @"Company Name", ShortName = @"Company Name")]
        public string CompanyName { get; set; }
        public List<PhoneNumber> PhoneNumbers { get; set; }
        public Dictionary<long, long> CardRelations { get; set; }
        public long? OwnerId { get; set; }
        public bool IsMyCard { get; set; }
        //public List < CategoryGroup > Categories { get; set; }
        public bool HasBackImage { get; set; }
        public bool IsUserLoggedIn { get; set; }
        public AccountType UserAccountType { get; set; }
        public Guid? FrontFileId { get; set; }
        public string FrontFileType { get; set; }
        public string FrontType { get; set; }
        public Guid? BackFileId { get; set; }
        public string BackFileType { get; set; }
        public string BackType { get; set; }
        public string FrontOrientation { get; set; }
        public string BackOrientation { get; set; }
        public bool ExistsInMyBusidex { get; set; }
        public string BasicRelationsJSON { get; set; }
        public Guid OwnerToken { get; set; }
        public string TagList { get; set; }
        public byte Visibility { get; set; }
        public List<Tag> Tags { get; set; }
        public DisplayType Display { get; set; }
        public string Markup { get; set; }
        public long CreatedBy { get; set; }
        public CardType CardType { get; set; }
        public string SEO_Name { get; set; }
        public string CustomContent { get; set; }
        public List<CardAddress> Addresses { get; set; }

        public List<ExternalLink> ExternalLinks { get; set; } = new List<ExternalLink>();

        public CardDetailModel()
        {

        }

        public CardDetailModel(Card card)
        {
            if (card == null) return;

            CardId = card.CardId;
            BusinessId = card.BusinessId;
            Email = card.Email;
            Name = card.Name;
            Title = card.Title;
            Url = card.Url;
            CompanyName = card.CompanyName;
            HasBackImage = card.BackImage != null && card.BackFileId != Guid.Empty;
            OwnerId = card.OwnerId;
            Searchable = card.Searchable;
            PhoneNumbers = card.PhoneNumbers;
            FrontFileId = card.FrontFileId;
            FrontFileType = card.FrontType;
            FrontType = card.FrontType;
            FrontOrientation = card.FrontOrientation;
            BackFileId = card.BackFileId;
            BackFileType = card.BackType;
            BackType = card.BackType;
            BackOrientation = card.BackOrientation;
            OwnerToken = card.OwnerToken.GetValueOrDefault();
            TagList = card.TagList;
            Tags = card.Tags;
            Display = card.Display;
            Markup = card.Markup;
            Addresses = card.Addresses;
            CardType = card.CardType;
            ExistsInMyBusidex = card.ExistsInMyBusidex;
            SEO_Name = card.SEO_Name;
            CustomContent = card.CustomContent;
            ExternalLinks.AddRange(card.ExternalLinks);

            if (PhoneNumbers != null && PhoneNumbers.Count == 0)
            {
                PhoneNumbers.Add(new PhoneNumber
                {
                    Number = string.Empty,
                    PhoneNumberTypeId = 1,
                    Extension = string.Empty
                });
            }
        }
    }

}
