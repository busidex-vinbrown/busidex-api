

using System.Data;

namespace Busidex.DomainModels.DotNet.DTO
{
    [Serializable]
    public class Card : CardBase, IComparable
    {

        public long CardId { get; set; }
        public string Name { get; set; }
        public string FrontOrientation { get; set; }
        public string BackOrientation { get; set; }
        public int? BusinessId { get; set; }
        public bool Searchable { get; set; }
        public string Email { get; set; }
        public string Url { get; set; }
        public string PhoneNumber1 { get; set; }
        public DateTime Created { get; set; }
        public long? CreatedBy { get; set; }
        public long? OwnerId { get; set; }
        public DateTime Updated { get; set; }
        public bool Deleted { get; set; }
        public string CompanyName { get; set; }
        public Guid? OwnerToken { get; set; }
        public Guid? FrontFileId { get; set; }
        public Guid? BackFileId { get; set; }
        public string Title { get; set; }
        public string Markup { get; set; }
        public byte Visibility { get; set; }
        public DisplayType Display { get; set; }
        public byte[] FrontImage { get; set; }
        public byte[] BackImage { get; set; }
        public string FrontType { get; set; }
        public string BackType { get; set; }
        public bool IsMyCard { get; set; }
        public CardType CardType { get; set; }
        public bool ExistsInMyBusidex { get; set; }
        public string SEO_Name { get; set; }
        public string CustomContent  { get; set; }
        public Card()
        {
            OnCreated();
        }

        public Card(IDataReader reader)
        {
            if (reader == null) return;

            BackOrientation = ConvertValue<string>(reader["BackOrientation"]);
            BackType = ConvertValue<string>(reader["BackType"]);
            Email = ConvertValue<string>(reader["Email"]);
            Name = ConvertValue<string>(reader["Name"]);
            Title = ConvertValue<string>(reader["Title"]);
            Url = ConvertValue<string>(reader["Url"]);
            CompanyName = ConvertValue<string>(reader["CompanyName"]);
            CardId = (long)reader["CardId"];
            Created = (DateTime)reader["Created"];
            Updated = (DateTime)reader["Updated"];
            FrontOrientation = ConvertValue<string>(reader["FrontOrientation"]);
            FrontType = ConvertValue<string>(reader["FrontType"]);
            CreatedBy = ConvertValue<long?>(reader["CreatedBy"]);
            OwnerId = ConvertValue<long?>(reader["OwnerId"]);
            Searchable = (bool)reader["Searchable"];
            FrontFileId = ConvertValue<Guid>(reader["FrontFileId"]);
            BackFileId = ConvertValue<Guid?>(reader["BackFileId"]);
            Enum.TryParse(typeof(DisplayType), ConvertValue<string>(reader["DisplayType"]), out object dt);
            Display = (DisplayType)dt;
            Visibility = (byte)reader["Visibility"];

            Tags = new List<Tag>();
            Addresses = new List<CardAddress>();
            PhoneNumbers = new List<PhoneNumber>();
            ExternalLinks = new List<ExternalLink>(); 
        }

        public static Card Clone(AddOrEditCardModel copy)
        {
            var card = new Card
            {
                BackImage = copy.BackImage,
                BackOrientation = copy.BackOrientation,
                BackType = copy.BackType,
                BusinessId = copy.BusinessId,
                Email = copy.Email,
                Name = copy.Name,
                Title = copy.Title,
                Url = copy.Url,
                CompanyName = copy.CompanyName,
                CardId = 0,
                Created = copy.Created,
                Updated = copy.Updated,
                FrontImage = copy.FrontImage,
                FrontOrientation = copy.FrontOrientation,
                FrontType = copy.FrontType,
                CreatedBy = copy.CreatedBy,
                OwnerId = copy.OwnerId,
                Searchable = copy.Searchable,
                FrontFileId = copy.FrontFileId,
                BackFileId = copy.BackFileId,
                PhoneNumbers = new List<PhoneNumber>(),
                Tags = new List<Tag>(),
                Addresses = new List<CardAddress>(),
                Markup = copy.Markup,
                Display = copy.Display,
                Visibility = copy.Visibility
            };

            card.PhoneNumbers.AddRange(copy.PhoneNumbers.Where(p=> !string.IsNullOrEmpty(p.Number)));
            card.Addresses.AddRange(copy.Addresses);
            card.Tags.AddRange(copy.Tags != null ? copy.Tags.Where(t => !t.Deleted) : new List<Tag>());
            card.ExternalLinks.AddRange(copy.ExternalLinks);

            return card;
        }

        public static void Update(AddOrEditCardModel copy, ref Card source)
        {

            source.BackImage = copy.BackImage;
            source.BackOrientation = copy.BackOrientation;
            source.BackType = copy.BackType;
            source.BusinessId = copy.BusinessId;
            source.Email = copy.Email;
            source.Name = copy.Name;
            source.Title = copy.Title;
            source.Url = copy.Url;
            source.CompanyName = copy.CompanyName;
            source.CardId = copy.CardId;
            source.Updated = copy.Updated;
            source.FrontImage = copy.FrontImage;
            source.FrontOrientation = copy.FrontOrientation;
            source.FrontType = copy.FrontType;
            source.Searchable = copy.Searchable;
            source.FrontFileId = copy.FrontFileId;
            source.BackFileId = copy.BackFileId;
            source.Markup = copy.Markup;
            source.Display = copy.Display;
            source.Visibility = copy.Visibility;
        }

        public void OnCreated()
        {
            if (Tags == null)
            {
                Tags = new List<Tag>();
            }
            if (Addresses == null)
            {
                Addresses = new List<CardAddress>();
            }
            if (PhoneNumbers == null)
            {
                PhoneNumbers = new List<PhoneNumber>();
            }
            if (FrontOrientation == null)
            {
                FrontOrientation = "H";
            }
            if (BackOrientation == null)
            {
                BackOrientation = "H";
            }
            if(ExternalLinks == null)
            {
                ExternalLinks = new List<ExternalLink>();
            }
        }

        public List<PhoneNumber> PhoneNumbers { get; set; }
        public List<Tag> Tags { get; set; }
        public List<CardAddress> Addresses { get; set; }
        public List<ExternalLink> ExternalLinks { get; set; }

        public string BackImageString
        {
            get { return BackImage != null && BackImage.Length > 0 ? Convert.ToBase64String(BackImage.ToArray()) : string.Empty; }
        }

        public string FrontImageString
        {
            get { return FrontImage != null ? Convert.ToBase64String(FrontImage.ToArray()) : string.Empty; }
        }

        public string TagList
        {
            get
            {
                string[] list = (from t in Tags
                                 where !t.Deleted
                                 select t.Text).ToArray();
                return string.Join(",", list);
            }
        }

        #region IComparable Members

        public int CompareTo(object obj)
        {
            var otherCard = obj as Card;
            if (otherCard != null)
            {
                if (otherCard.Name == Name && otherCard.FrontImage == FrontImage && otherCard.BackImage == BackImage && otherCard.Display == Display && otherCard.Markup == Markup)
                {
                    return 0;
                }
                if (otherCard.CardId == CardId)
                {
                    return 0;
                }
            }
            return -1;
        }

        #endregion
    }
}