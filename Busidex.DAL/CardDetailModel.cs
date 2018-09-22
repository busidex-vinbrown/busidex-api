using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Busidex.DAL
{
    public class CardDetailModel : ViewModelBase
    {

        public long CardId { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public int? BusinessId { get; set; }
        public bool Searchable { get; set; }
        public string Email { get; set; }
        public string Url { get; set; }

        [Display(Name = "Company Name", Description = "Company Name", Prompt = "Company Name", ShortName = "Company Name")]
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
        public Guid? BackFileId { get; set; }
        public string BackFileType { get; set; }
        public string FrontOrientation { get; set; }
        public string BackOrientation { get; set; }
        public bool ExistsInMyBusidex { get; set; }
        public string BasicRelationsJSON { get; set; }
        public Guid OwnerToken { get; set; }
        public string TagList { get; set; }
        public List<Tag> Tags { get; set; }
        public DisplayType Display { get; set; }
        public string Markup { get; set; }

        public CardDetailModel(Card card)
        {

            CardId = card.CardId;
            //if ( card.CardRelations != null )
            //{
            //    CardRelations = ( from cr in card.CardRelations
            //                      where !cr.Deleted
            //                      select cr ).ToDictionary ( cr => cr.CardId, cr => cr.RelatedCardId );
            //}
            BusinessId = card.BusinessId;
            Email = card.Email;
            Name = card.Name;
            Title = card.Title;
            Url = card.Url;
            CompanyName = card.CompanyName;
            HasBackImage = card.BackImage != null;
            OwnerId = card.OwnerId;
            PhoneNumbers = card.PhoneNumbers;
            FrontFileId = card.FrontFileId;
            FrontFileType = card.FrontType;
            FrontOrientation = card.FrontOrientation;
            BackFileId = card.BackFileId;
            BackFileType = card.BackType;
            BackOrientation = card.BackOrientation;
            OwnerToken = card.OwnerToken.GetValueOrDefault();
            TagList = card.TagList;
            Tags = card.Tags;
            Display = card.Display;
            Markup = card.Markup;

            //BasicRelationsJSON = card.BasicRelationsJSON;

            if (PhoneNumbers != null && PhoneNumbers.Count == 0)
            {
                PhoneNumbers.Add(new PhoneNumber
                    {
                        Number = string.Empty,
                        PhoneNumberTypeId = 1,
                        Extension = string.Empty
                    });
            }

            //if (Categories == null) Categories = new List<CategoryGroup>();

            //foreach ( var category in card.CardCategories )
            //{
            //    Categories.Add (
            //        new CategoryGroup
            //            {
            //                Category = category,
            //                Cards = ( from c in card.CardCategories
            //                          where c.CardCategoryId == category.CardCategoryId
            //                          select c.Card ).ToList()
            //            }
            //        );
            //}
        }


    }

}
