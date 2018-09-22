using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Reflection;
using System.Web;
using Busidex.Providers;

namespace Busidex.DAL
{
    using System.Collections.Generic;
    using System.Linq;
    using System;

    partial class BusidexDataContext : IBusidexDataContext
    {

        public void RelateCards(long ownerId, long relatedCardId)
        {
            _RelateCards(ownerId, relatedCardId);
        }

        public void UnRelateCards(long ownerId, long relatedCardId)
        {
            _UnRelateCards(ownerId, relatedCardId);
        }

        public List<CardImage> SyncData(long id)
        {
            var results = _GetMyBusidexImages(id).ToList();
            return results.Select(img => new CardImage { FileImage = img.FrontImage.ToArray(), Name = img.FrontFileId.ToString(), FileType = img.FrontType }).ToList();
        }

        public List<UserCard> GetMyBusidex(long userId, bool includeImages)
        {
            //lock (this)
            //{
            // This may include deleted cards. If someone deletes their card, that should not
            // necessarily remove that card from everyone's busidex collection.
            var mybusidex = _GetMyBusidex(userId, includeImages);
            var relatedCards = mybusidex.GetResult<CardRelation>().ToList();

            var cardResults = mybusidex.GetResult<usp_getMyBusidexResult>().ToList();

            var cardIds = string.Join(",", cardResults.Select(c => c.CardId).ToArray());
            var phoneNumbersDTO = _GetCardPhoneNumbers(cardIds);

            var phoneNumbers = phoneNumbersDTO.Select(phoneNumber => new PhoneNumber
                                                                         {
                                                                             CardId = phoneNumber.CardId,
                                                                             Number = phoneNumber.Number,
                                                                             Extension = phoneNumber.Extension,
                                                                             PhoneNumberType = new PhoneNumberType
                                                                                            {
                                                                                                Name = phoneNumber.Name,
                                                                                                PhoneNumberTypeId = phoneNumber.PhoneNumberTypeId
                                                                                            }
                                                                         }).ToList();


            var cards = cardResults.Select(item => new UserCard
                                                     {
                                                         UserCardId = item.UserCardId,
                                                         Notes = HttpUtility.UrlDecode(item.Notes),
                                                         CardId = item.CardId,
                                                         RelatedCards = relatedCards.Where(r => r.CardId == item.CardId).ToList(),
                                                         MobileView = item.MobileView,
                                                         Card = new Card
                                                                  {
                                                                      BackFileId = item.BackFileId,
                                                                      BackOrientation = item.BackOrientation,
                                                                      BackType = item.BackType,
                                                                      CardId = item.CardId,
                                                                      FrontFileId = item.FrontFileId,
                                                                      FrontType = item.FrontType,
                                                                      FrontOrientation = item.FrontOrientation,
                                                                      Email = item.Email,
                                                                      Url = item.Url,
                                                                      Title = item.Title,
                                                                      Name = item.Name,
                                                                      OwnerId = item.OwnerId,
                                                                      CreatedBy = item.CreatedBy,
                                                                      CompanyName = item.CompanyName,
                                                                      PhoneNumbers = phoneNumbers.Where(p => p.CardId == item.CardId).ToList(),
                                                                      FrontImage = includeImages ? item.FrontImage : null,
                                                                      BackImage = includeImages ? item.BackImage : null                                                                      
                                                                  }
                                                     }).ToList();


            var tags = _GetCardTagsByIds(string.Join(",", cards.Select(c => c.CardId))).ToList();
            foreach (var userCard in cards)
            {
                userCard.Card.Tags = (from tag in tags
                                      where tag.CardId == userCard.CardId
                                      select new Tag
                                                 {
                                                     TagId = tag.TagId,
                                                     Text = tag.Text
                                                 }).ToList();
            }
            return cards.ToList();
            //}
        }

        public int ActivateUserAccount(long userAccountId)
        {
            var result = _ActivateUserAccount(userAccountId).SingleOrDefault();

            if (result != null)
            {
                return result.RowsUpdated;
            }

            return 0;
        }

        public void DeleteUserCard(UserCard uc, long userId)
        {

            _DeleteUserCard(uc.CardId, userId);

            List<UserCard> myBusidex = GetMyBusidex(userId, false);
            myBusidex.Remove(uc);
            var cache = new BusidexCacheProvider();
            cache.UpdateCache(BusidexCacheProvider.CachKeys.MyBusidex, myBusidex);

        }

        public IEnumerable<AccountType> GetActivePlans()
        {
            return _GetActivePlans().Select(p => new AccountType
                                                     {
                                                         AccountTypeId = p.AccountTypeId,
                                                         Active = p.Active,
                                                         Description = p.Description,
                                                         Name = p.Name,
                                                         DisplayOrder = p.DisplayOrder
                                                     }).AsEnumerable();
        }

        public int GetCardCount()
        {
            return _GetCardCount().Single().CardCount.GetValueOrDefault();
        }

        public List<PhoneNumberType> GetAllPhoneNumberTypes()
        {
            return _GetAllPhoneNumberTypes().Select(t => new PhoneNumberType { Deleted = t.Deleted, Name = t.Name, PhoneNumberTypeId = t.PhoneNumberTypeId }).ToList();
        }

        public PhoneNumber GetPhoneNumberById(long id)
        {
            var p = _GetPhoneNumberById(id).SingleOrDefault();
            if (p == null) return null;

            return new PhoneNumber
                       {
                           PhoneNumberId = p.PhoneNumberId,
                           PhoneNumberType = new PhoneNumberType
                                                 {
                                                     Name = p.Name,
                                                     PhoneNumberTypeId = p.PhoneNumberTypeId
                                                 },
                           PhoneNumberTypeId = p.PhoneNumberTypeId,
                           Extension = p.Extension,
                           CardId = p.CardId,
                           Number = p.Number,
                           Created = p.Created,
                           Updated = p.Updated
                       };
        }

        public Card GetCardById(long id)
        {
            var cardById = _GetCardById(id).SingleOrDefault();
            if (cardById != null)
            {
                var cardPhoneNumbers = _GetCardPhoneNumber(id).ToList();

                var phoneNumbers = cardPhoneNumbers.Select(p => new PhoneNumber
                                                                    {
                                                                        CardId = p.CardId,
                                                                        Created = p.Created,
                                                                        Deleted = p.Deleted,
                                                                        Extension = p.Extension,
                                                                        Number = p.Number,
                                                                        PhoneNumberTypeId = p.PhoneNumberTypeId,
                                                                        Updated = p.Updated,
                                                                        PhoneNumberId = p.PhoneNumberId,
                                                                        PhoneNumberType = new PhoneNumberType
                                                                                              {
                                                                                                  PhoneNumberTypeId = p.PhoneNumberTypeId,
                                                                                                  Name = p.Name
                                                                                              }
                                                                    }).ToList();

                var tags = _GetCardTags(id);
                var cardTags = tags.Select(t => new Tag
                                                    {
                                                        TagId = t.TagId,
                                                        Text = t.Text
                                                    });

                var addresses = _GetCardAddresses(id);
                var stateCodes = GetAllStateCodes();
                var cardAddresses = addresses.Select(a => new CardAddress
                                                              {
                                                                  Address1 = a.Address1,
                                                                  Address2 = a.Address2,
                                                                  CardAddressId = a.CardAddressId,
                                                                  CardId = a.CardId,
                                                                  City = a.City,
                                                                  Country = a.Country,
                                                                  Deleted = a.Deleted,
                                                                  Region = a.Region,
                                                                  State = stateCodes.SingleOrDefault(sc => sc.Code == a.State),
                                                                  ZipCode = a.ZipCode
                                                              }).ToList();

                DisplayType dispType;
                Enum.TryParse(cardById.DisplayType, true, out dispType);

                var card = new Card
                               {
                                   BackFileId = cardById.BackFileId,
                                   BackOrientation = cardById.BackOrientation,
                                   BackType = cardById.BackType,
                                   BackImage = cardById.BackImage,
                                   CardId = cardById.CardId,
                                   OwnerId = cardById.OwnerId,
                                   FrontFileId = cardById.FrontFileId,
                                   FrontImage = cardById.FrontImage,
                                   FrontType = cardById.FrontType,
                                   FrontOrientation = cardById.FrontOrientation,
                                   Email = cardById.Email,
                                   Url = cardById.Url,
                                   Title = cardById.Title,
                                   Name = cardById.Name,
                                   Searchable = cardById.Searchable,
                                   CreatedBy = cardById.CreatedBy,
                                   CompanyName = cardById.CompanyName,
                                   PhoneNumbers = phoneNumbers.ToList(),
                                   Tags = cardTags.ToList(),
                                   Addresses = cardAddresses,
                                   Markup = cardById.Markup,
                                   Display = dispType
                               };

                return card;
            }
            return null;
        }

        public UserCard GetUserCard(long cardId, long userId)
        {
            return _GetUserCard(cardId, userId).Select(uc => new UserCard
                                                                 {
                                                                     CardId = uc.CardId,
                                                                     Created = uc.Created,
                                                                     Deleted = uc.Deleted,
                                                                     Notes = uc.Notes,
                                                                     OwnerId = uc.OwnerId,
                                                                     UserId = uc.UserId,
                                                                     UserCardId = uc.UserCardId,
                                                                     SharedById = uc.SharedById,
                                                                     Card = GetCardById(uc.CardId)
                                                                 })
                                               .SingleOrDefault();
        }

        public UserCard GetUserCardById(long id)
        {
            return _GetUserCardById(id)
                .Select(uc => new UserCard
                                  {
                                      CardId = uc.CardId,
                                      Created = uc.Created,
                                      Deleted = uc.Deleted,
                                      Notes = uc.Notes,
                                      OwnerId = uc.OwnerId,
                                      UserId = uc.UserId,
                                      UserCardId = uc.UserCardId,
                                      SharedById = uc.SharedById,
                                      Card = GetCardById(uc.CardId)
                                  })
                .SingleOrDefault();
        }

        public bool IsACardOwner(long ownerId)
        {
            var cardsByOwner = _GetCardsByOwnerId(ownerId).ToList();
            return cardsByOwner.Any();
        }

        public List<Card> GetCardsByOwnerId(long ownerId)
        {
            var cardsByOwner = _GetCardsByOwnerId(ownerId).ToList();
            var stateCodes = GetAllStateCodes();

            return (from result in cardsByOwner
                    let cardPhoneNumbers = _GetCardPhoneNumber(result.CardId).ToList()
                    let phoneNumbers = cardPhoneNumbers.Select(p => new PhoneNumber
                                                                        {
                                                                            CardId = p.CardId,
                                                                            Created = p.Created,
                                                                            Deleted = p.Deleted,
                                                                            Extension = p.Extension,
                                                                            Number = p.Number,
                                                                            PhoneNumberTypeId = p.PhoneNumberTypeId,
                                                                            Updated = p.Updated,
                                                                            PhoneNumberId = p.PhoneNumberId,
                                                                            PhoneNumberType = new PhoneNumberType
                                                                                                  {
                                                                                                      PhoneNumberTypeId = p.PhoneNumberTypeId,
                                                                                                      Name = p.Name
                                                                                                  }
                                                                        }).ToList()
                    let tags = _GetCardTags(result.CardId)
                    let cardTags = tags.Select(t => new Tag
                                                        {
                                                            TagId = t.TagId,
                                                            Text = t.Text
                                                        })
                    let addresses = _GetCardAddresses(result.CardId)                    
                    let cardAddresses = addresses.Select(a => new CardAddress
                                                                  {
                                                                      Address1 = a.Address1,
                                                                      Address2 = a.Address2,
                                                                      CardAddressId = a.CardAddressId,
                                                                      CardId = a.CardId,
                                                                      City = a.City,
                                                                      Country = a.Country,
                                                                      Deleted = a.Deleted,
                                                                      Region = a.Region,
                                                                      State = stateCodes.SingleOrDefault(sc => sc.Code == a.State),
                                                                      ZipCode = a.ZipCode
                                                                  })
                    select new Card
                               {
                                   BackFileId = result.BackFileId,
                                   BackOrientation = result.BackOrientation,
                                   BackType = result.BackType,
                                   CardId = result.CardId,
                                   OwnerId = result.OwnerId,
                                   FrontFileId = result.FrontFileId,
                                   FrontType = result.FrontType,
                                   FrontOrientation = result.FrontOrientation,
                                   Email = result.Email,
                                   Url = result.Url,
                                   Title = result.Title,
                                   Name = result.Name,
                                   Searchable = result.Searchable,
                                   CreatedBy = result.CreatedBy,
                                   CompanyName = result.CompanyName,
                                   PhoneNumbers = phoneNumbers.ToList(),
                                   Tags = cardTags.ToList(),
                                   Addresses = cardAddresses.ToList(),
                                   Display = result.DisplayType == null ? DisplayType.IMG : (DisplayType)Enum.Parse(typeof(DisplayType), result.DisplayType),
                                   Markup = result.Markup
                               }).ToList();
        }

        public List<BusidexUser> GetAllBusidexUsers()
        {
            var users = _GetAllBusidexUsers().Select(bu => new BusidexUser
            {
                ApplicationId = bu.ApplicationId,
                Email = bu.Email,
                UserId = bu.UserId,
                IsAnonymous = bu.IsAnonymous,
                LastActivityDate = bu.LastActivityDate,
                UserName = bu.UserName,
                LoweredUserName = bu.LoweredUserName,
                MobileAlias = bu.MobileAlias
            }).ToList();

            return users;
        }

        public BusidexUser GetBusidexUserById(long id)
        {

            var user = _GetBusidexUserByUserId(id).Select(bu => new BusidexUser
                                                                    {
                                                                        ApplicationId = bu.ApplicationId,
                                                                        Email = bu.Email,
                                                                        UserId = bu.UserId,
                                                                        IsAnonymous = bu.IsAnonymous,
                                                                        LastActivityDate = bu.LastActivityDate,
                                                                        UserName = bu.UserName,
                                                                        LoweredUserName = bu.LoweredUserName,
                                                                        MobileAlias = bu.MobileAlias
                                                                    }).SingleOrDefault();


            var address = _GetUserAddress(id).FirstOrDefault();

            if (user != null)
            {
                if (address == null)
                {
                    user.Address = new UserAddress
                                       {
                                           UserId = id,
                                           UserAddressId = 0
                                       };
                }
                else
                {
                    user.Address = new UserAddress
                                       {
                                           Address1 = address.Address1,
                                           Address2 = address.Address2,
                                           City = address.City,
                                           State = address.State,
                                           ZipCode = address.ZipCode,
                                           Region = address.Region,
                                           Country = address.Country,
                                           UserAddressId = address.UserAddressId,
                                           UserId = address.UserId,
                                           Latitude = address.Latitude.GetValueOrDefault(),
                                           Longitude = address.Longitude.GetValueOrDefault()
                                       };
                }
                user.Settings = _GetSettingByUserId(user.UserId)
                    .Select(s => new Setting
                                     {
                                         AllowGoogleSync = s.AllowGoogleSync,
                                         Deleted = s.Deleted,
                                         StartPage = s.StartPage,
                                         Updated = s.Updated,
                                         SettingsId = s.SettingsId,
                                         UserId = s.UserId
                                     })
                    .SingleOrDefault();

            }
            return user;
        }

        public IEnumerable<Card> GetCardsByNameKeyword(string text, double? latitude, double? longitude, int? distance)
        {
            lock (this)
            {
                var allCards = _GetCardsByNameKeyword(text, latitude, longitude, distance);
                var cards = allCards.Select(item => new Card
                                                        {
                                                            BackFileId = item.BackFileId,
                                                            BackOrientation = item.BackOrientation,
                                                            BackType = item.BackType,
                                                            CardId = item.CardId,
                                                            FrontFileId = item.FrontFileId,
                                                            FrontType = item.FrontType,
                                                            FrontOrientation = item.FrontOrientation,
                                                            Email = item.Email,
                                                            OwnerId = item.OwnerId,
                                                            Url = item.Url,
                                                            Searchable = item.Searchable,
                                                            PhoneNumbers = _GetCardPhoneNumber(item.CardId).Select(p => new PhoneNumber
                                                                                                                            {
                                                                                                                                CardId = p.CardId,
                                                                                                                                Created = p.Created,
                                                                                                                                Deleted = p.Deleted,
                                                                                                                                Extension = p.Extension,
                                                                                                                                Number = p.Number,
                                                                                                                                PhoneNumberTypeId = p.PhoneNumberTypeId,
                                                                                                                                Updated = p.Updated,
                                                                                                                                PhoneNumberId = p.PhoneNumberId,
                                                                                                                                PhoneNumberType = new PhoneNumberType
                                                                                                                                                      {
                                                                                                                                                          PhoneNumberTypeId = p.PhoneNumberTypeId,
                                                                                                                                                          Name = p.Name
                                                                                                                                                      }
                                                                                                                            }).ToList(),
                                                            Title = item.Title,
                                                            Name = item.Name,
                                                            CompanyName = item.CompanyName

                                                        }).ToList();

                var tags = _GetCardTagsByIds(string.Join(",", cards.Select(c => c.CardId))).ToList();
                foreach (var card in cards)
                {
                    card.Tags = (from _tag in tags
                                 where _tag.CardId == card.CardId
                                 select new Tag
                                 {
                                     TagId = _tag.TagId,
                                     Text = _tag.Text
                                 }).ToList();
                }

                return cards;
            }
        }

        public IEnumerable<Card> GetCardsByTitle(string title, double? latitude, double? longitude, int? distance)
        {
            lock (this)
            {
                var allCards = _GetCardsByTitle(title, latitude, longitude, distance);
                var cards = allCards.Select(item => new Card
                                                        {
                                                            BackFileId = item.BackFileId,
                                                            BackOrientation = item.BackOrientation,
                                                            BackType = item.BackType,
                                                            CardId = item.CardId,
                                                            FrontFileId = item.FrontFileId,
                                                            FrontType = item.FrontType,
                                                            FrontOrientation = item.FrontOrientation,
                                                            Email = item.Email,
                                                            OwnerId = item.OwnerId,
                                                            Url = item.Url,
                                                            Searchable = item.Searchable,
                                                            Title = item.Title,
                                                            Name = item.Name,
                                                            PhoneNumbers = _GetCardPhoneNumber(item.CardId).Select(p => new PhoneNumber
                                                                                                                            {
                                                                                                                                CardId = p.CardId,
                                                                                                                                Created = p.Created,
                                                                                                                                Deleted = p.Deleted,
                                                                                                                                Extension = p.Extension,
                                                                                                                                Number = p.Number,
                                                                                                                                PhoneNumberTypeId = p.PhoneNumberTypeId,
                                                                                                                                Updated = p.Updated,
                                                                                                                                PhoneNumberId = p.PhoneNumberId,
                                                                                                                                PhoneNumberType = new PhoneNumberType
                                                                                                                                                      {
                                                                                                                                                          PhoneNumberTypeId = p.PhoneNumberTypeId,
                                                                                                                                                          Name = p.Name
                                                                                                                                                      }
                                                                                                                            }).ToList(),
                                                            CompanyName = item.CompanyName
                                                        }).ToList();

                var tags = _GetCardTagsByIds(string.Join(",", cards.Select(c => c.CardId))).ToList();
                foreach (var card in cards)
                {
                    card.Tags = (from _tag in tags
                                 where _tag.CardId == card.CardId
                                 select new Tag
                                 {
                                     TagId = _tag.TagId,
                                     Text = _tag.Text
                                 }).ToList();
                }

                return cards;
            }
        }

        public IEnumerable<Card> GetCardsByTag(string tag, double? latitude, double? longitude, int? distance)
        {
            lock (this)
            {
                var allCards = _GetCardsByTag(tag, latitude, longitude, distance);
                var cards = allCards.Select(item => new Card
                                                        {
                                                            BackFileId = item.BackFileId,
                                                            BackOrientation = item.BackOrientation,
                                                            BackType = item.BackType,
                                                            CardId = item.CardId,
                                                            FrontFileId = item.FrontFileId,
                                                            FrontType = item.FrontType,
                                                            FrontOrientation = item.FrontOrientation,
                                                            Email = item.Email,
                                                            OwnerId = item.OwnerId,
                                                            Url = item.Url,
                                                            Searchable = item.Searchable,
                                                            Title = item.Title,
                                                            Name = item.Name,
                                                            PhoneNumbers = _GetCardPhoneNumber(item.CardId).Select(p => new PhoneNumber
                                                                                                                            {
                                                                                                                                CardId = p.CardId,
                                                                                                                                Created = p.Created,
                                                                                                                                Deleted = p.Deleted,
                                                                                                                                Extension = p.Extension,
                                                                                                                                Number = p.Number,
                                                                                                                                PhoneNumberTypeId = p.PhoneNumberTypeId,
                                                                                                                                Updated = p.Updated,
                                                                                                                                PhoneNumberId = p.PhoneNumberId,
                                                                                                                                PhoneNumberType = new PhoneNumberType
                                                                                                                                                      {
                                                                                                                                                          PhoneNumberTypeId = p.PhoneNumberTypeId,
                                                                                                                                                          Name = p.Name
                                                                                                                                                      }
                                                                                                                            }).ToList(),
                                                            CompanyName = item.CompanyName
                                                        }).ToList();

                var tags = _GetCardTagsByIds(string.Join(",", cards.Select(c => c.CardId))).ToList();
                foreach (var card in cards)
                {
                    card.Tags = (from _tag in tags
                                 where _tag.CardId == card.CardId
                                 select new Tag
                                 {
                                     TagId = _tag.TagId,
                                     Text = _tag.Text
                                 }).ToList();
                }

                return cards;
            }
        }

        public IEnumerable<Card> GetCardsByPhoneNumber(string phone, double? latitude, double? longitude, int? distance)
        {
            lock (this)
            {
                var allCards = _GetCardsByPhoneNumber(phone, latitude, longitude, distance);
                var cards = allCards.Select(item => new Card
                                                        {
                                                            BackFileId = item.BackFileId,
                                                            BackOrientation = item.BackOrientation,
                                                            BackType = item.BackType,
                                                            CardId = item.CardId,
                                                            FrontFileId = item.FrontFileId,
                                                            FrontType = item.FrontType,
                                                            FrontOrientation = item.FrontOrientation,
                                                            Email = item.Email,
                                                            OwnerId = item.OwnerId,
                                                            Url = item.Url,
                                                            Searchable = item.Searchable,
                                                            Title = item.Title,
                                                            Name = item.Name,
                                                            PhoneNumbers = _GetCardPhoneNumber(item.CardId).Select(p => new PhoneNumber
                                                                                                                            {
                                                                                                                                CardId = p.CardId,
                                                                                                                                Created = p.Created,
                                                                                                                                Deleted = p.Deleted,
                                                                                                                                Extension = p.Extension,
                                                                                                                                Number = p.Number,
                                                                                                                                PhoneNumberTypeId = p.PhoneNumberTypeId,
                                                                                                                                Updated = p.Updated,
                                                                                                                                PhoneNumberId = p.PhoneNumberId,
                                                                                                                                PhoneNumberType = new PhoneNumberType
                                                                                                                                                      {
                                                                                                                                                          PhoneNumberTypeId = p.PhoneNumberTypeId,
                                                                                                                                                          Name = p.Name
                                                                                                                                                      }
                                                                                                                            }).ToList(),
                                                            CompanyName = item.CompanyName
                                                        }).ToList();

                var tags = _GetCardTagsByIds(string.Join(",", cards.Select(c => c.CardId))).ToList();
                foreach (var card in cards)
                {
                    card.Tags = (from _tag in tags
                                 where _tag.CardId == card.CardId
                                 select new Tag
                                 {
                                     TagId = _tag.TagId,
                                     Text = _tag.Text
                                 }).ToList();
                }

                return cards;
            }
        }

        public IEnumerable<Card> GetCardsByCompanyName(string name, double? latitude, double? longitude, int? distance)
        {
            lock (this)
            {
                var allCards = _GetCardsByCompany(name, latitude, longitude, distance);
                var cards = allCards.Select(item => new Card
                                                        {
                                                            BackFileId = item.BackFileId,
                                                            BackOrientation = item.BackOrientation,
                                                            BackType = item.BackType,
                                                            CardId = item.CardId,
                                                            FrontFileId = item.FrontFileId,
                                                            FrontType = item.FrontType,
                                                            FrontOrientation = item.FrontOrientation,
                                                            Email = item.Email,
                                                            OwnerId = item.OwnerId,
                                                            Url = item.Url,
                                                            Searchable = item.Searchable,
                                                            Title = item.Title,
                                                            Name = item.Name,
                                                            PhoneNumbers = _GetCardPhoneNumber(item.CardId).Select(p => new PhoneNumber
                                                                                                                            {
                                                                                                                                CardId = p.CardId,
                                                                                                                                Created = p.Created,
                                                                                                                                Deleted = p.Deleted,
                                                                                                                                Extension = p.Extension,
                                                                                                                                Number = p.Number,
                                                                                                                                PhoneNumberTypeId = p.PhoneNumberTypeId,
                                                                                                                                Updated = p.Updated,
                                                                                                                                PhoneNumberId = p.PhoneNumberId,
                                                                                                                                PhoneNumberType = new PhoneNumberType
                                                                                                                                                      {
                                                                                                                                                          PhoneNumberTypeId = p.PhoneNumberTypeId,
                                                                                                                                                          Name = p.Name
                                                                                                                                                      }
                                                                                                                            }).ToList(),
                                                            CompanyName = item.CompanyName
                                                        }).ToList();

                var tags = _GetCardTagsByIds(string.Join(",", cards.Select(c => c.CardId))).ToList();
                foreach (var card in cards)
                {
                    card.Tags = (from _tag in tags
                                 where _tag.CardId == card.CardId
                                 select new Tag
                                 {
                                     TagId = _tag.TagId,
                                     Text = _tag.Text
                                 }).ToList();
                }

                return cards;
            }
        }

        public IEnumerable<Card> SearchCards(string criteria, double? latitude, double? longitude, int? distance, bool searchableOnly)
        {
            //lock (this)
            //{
                var allCards = _SearchCards(criteria, latitude, longitude, distance, searchableOnly);
                var cards = allCards.Select(item => new Card
                {
                    BackFileId = item.BackFileId,
                    BackOrientation = item.BackOrientation,
                    BackType = item.BackType,
                    CardId = item.CardId,
                    FrontFileId = item.FrontFileId,
                    FrontType = item.FrontType,
                    FrontOrientation = item.FrontOrientation,
                    Email = item.Email,
                    OwnerId = item.OwnerId,
                    Url = item.Url,
                    Searchable = item.Searchable,
                    Title = item.Title,
                    Name = item.Name,
                    PhoneNumbers = _GetCardPhoneNumber(item.CardId).Select(p => new PhoneNumber
                    {
                        CardId = p.CardId,
                        Created = p.Created,
                        Deleted = p.Deleted,
                        Extension = p.Extension,
                        Number = p.Number,
                        PhoneNumberTypeId = p.PhoneNumberTypeId,
                        Updated = p.Updated,
                        PhoneNumberId = p.PhoneNumberId,
                        PhoneNumberType = new PhoneNumberType
                        {
                            PhoneNumberTypeId = p.PhoneNumberTypeId,
                            Name = p.Name
                        }
                    }).ToList(),
                    CompanyName = item.CompanyName,
                    Display = item.DisplayType == null ? DisplayType.IMG : (DisplayType)Enum.Parse(typeof(DisplayType), item.DisplayType),
                    Markup = item.Markup
                }).ToList();

                var tags = _GetCardTagsByIds(string.Join(",", cards.Select(c => c.CardId))).ToList();
                foreach (var card in cards)
                {
                    card.Tags = (from _tag in tags
                                 where _tag.CardId == card.CardId
                                 select new Tag
                                 {
                                     TagId = _tag.TagId,
                                     Text = _tag.Text
                                 }).ToList();
                }

                return cards;
            //}
        }

        public IEnumerable<Card> GetCardsByEmail(string email, double? latitude, double? longitude, int? distance)
        {
            lock (this)
            {
                var allCards = _GetCardsByEmail(email, latitude, longitude, distance);
                var cards = allCards.Select(item => new Card
                                                        {
                                                            BackFileId = item.BackFileId,
                                                            BackOrientation = item.BackOrientation,
                                                            BackType = item.BackType,
                                                            CardId = item.CardId,
                                                            FrontFileId = item.FrontFileId,
                                                            FrontType = item.FrontType,
                                                            FrontOrientation = item.FrontOrientation,
                                                            Email = item.Email,
                                                            OwnerId = item.OwnerId,
                                                            Url = item.Url,
                                                            Searchable = item.Searchable,
                                                            Title = item.Title,
                                                            Name = item.Name,
                                                            PhoneNumbers = _GetCardPhoneNumber(item.CardId).Select(p => new PhoneNumber
                                                                                                                            {
                                                                                                                                CardId = p.CardId,
                                                                                                                                Created = p.Created,
                                                                                                                                Deleted = p.Deleted,
                                                                                                                                Extension = p.Extension,
                                                                                                                                Number = p.Number,
                                                                                                                                PhoneNumberTypeId = p.PhoneNumberTypeId,
                                                                                                                                Updated = p.Updated,
                                                                                                                                PhoneNumberId = p.PhoneNumberId,
                                                                                                                                PhoneNumberType = new PhoneNumberType
                                                                                                                                                      {
                                                                                                                                                          PhoneNumberTypeId = p.PhoneNumberTypeId,
                                                                                                                                                          Name = p.Name
                                                                                                                                                      }
                                                                                                                            }).ToList(),
                                                            CompanyName = item.CompanyName
                                                        }).ToList();

                var tags = _GetCardTagsByIds(string.Join(",", cards.Select(c => c.CardId))).ToList();
                foreach (var card in cards)
                {
                    card.Tags = (from _tag in tags
                                 where _tag.CardId == card.CardId
                                 select new Tag
                                 {
                                     TagId = _tag.TagId,
                                     Text = _tag.Text
                                 }).ToList();
                }

                return cards;
            }
        }

        public Card GetCardByToken(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return null;
            }

            var ownerToken = new Guid(token);
            var cardById = _GetCardByOwnerToken(ownerToken).SingleOrDefault();
            if (cardById != null)
            {
                var cardPhoneNumbers = _GetCardPhoneNumber(cardById.CardId).ToList();

                var phoneNumbers = cardPhoneNumbers.Select(p => new PhoneNumber
                {
                    CardId = p.CardId,
                    Created = p.Created,
                    Deleted = p.Deleted,
                    Extension = p.Extension,
                    Number = p.Number,
                    PhoneNumberTypeId = p.PhoneNumberTypeId,
                    Updated = p.Updated,
                    PhoneNumberId = p.PhoneNumberId,
                    PhoneNumberType = new PhoneNumberType
                    {
                        PhoneNumberTypeId = p.PhoneNumberTypeId,
                        Name = p.Name
                    }
                }).ToList();

                var tags = _GetCardTags(cardById.CardId);
                var cardTags = tags.Select(t => new Tag
                {
                    TagId = t.TagId,
                    Text = t.Text
                });

                var addresses = _GetCardAddresses(cardById.CardId);
                var stateCodes = GetAllStateCodes();
                var cardAddresses = addresses.Select(a => new CardAddress
                {
                    Address1 = a.Address1,
                    Address2 = a.Address2,
                    CardAddressId = a.CardAddressId,
                    CardId = a.CardId,
                    City = a.City,
                    Country = a.Country,
                    Deleted = a.Deleted,
                    Region = a.Region,
                    State = stateCodes.SingleOrDefault(sc=> sc.Code == a.State),
                    ZipCode = a.ZipCode
                });
                var card = new Card
                {
                    BackFileId = cardById.BackFileId,
                    BackOrientation = cardById.BackOrientation,
                    BackType = cardById.BackType,
                    CardId = cardById.CardId,
                    OwnerId = cardById.OwnerId,
                    OwnerToken = cardById.OwnerToken,
                    FrontFileId = cardById.FrontFileId,
                    FrontType = cardById.FrontType,
                    FrontOrientation = cardById.FrontOrientation,
                    Email = cardById.Email,
                    Url = cardById.Url,
                    Title = cardById.Title,
                    Name = cardById.Name,
                    CreatedBy = cardById.CreatedBy,
                    CompanyName = cardById.CompanyName,
                    PhoneNumbers = phoneNumbers.ToList(),
                    Tags = cardTags.ToList(),
                    Addresses = cardAddresses.ToList()
                };

                return card;
            }
            return null;
        }

        public string GetCustomContentById(int id)
        {
            string content = "";
            CustomContent cc = _GetCustomContentById(id).Select(c => new CustomContent { ContentId = c.ContentId, PageContent = c.PageContent }).SingleOrDefault();
            if (cc != null)
            {
                content = cc.PageContent;
            }

            return content;
        }

        public List<Captcha> GetCaptchaItems()
        {
            return _GetAllCaptchas().Select(c => new Captcha { CaptchaId = c.CaptchaId, CaptchaText = c.CaptchaText, Deleted = c.Deleted }).ToList();
        }

        public Captcha GetCaptchaItemById(int id)
        {
            return _GetCaptchaById(id).Select(c => new Captcha { CaptchaId = c.CaptchaId, CaptchaText = c.CaptchaText, Deleted = c.Deleted }).SingleOrDefault();
        }

        public IEnumerable<Page> GetAllSitePages()
        {
            return _GetAllSitePages()
                .Select(p => new Page { Action = p.Action, ControllerName = p.ControllerName, Deleted = p.Deleted, PageId = p.PageId, Title = p.Title })
                .ToList();
        }

        public Page GetPageByViewName(string name)
        {
            return _GetPageByViewName(name)
                .Select(p => new Page { Action = p.Action, ControllerName = p.ControllerName, Deleted = p.Deleted, PageId = p.PageId, Title = p.Title })
                .SingleOrDefault();
        }

        public Setting GetUserSetting(BusidexUser user)
        {

            return _GetSettingByUserId(user.UserId)
                .Select(s => new Setting
                                 {
                                     AllowGoogleSync = s.AllowGoogleSync,
                                     Deleted = s.Deleted,
                                     Updated = s.Updated,
                                     StartPage = s.StartPage,
                                     SettingsId = s.SettingsId,
                                     UserId = s.UserId
                                 })
                .SingleOrDefault();
        }

        public Setting AddDefaultUserSetting(BusidexUser user)
        {

            Setting setting = GetUserSetting(user);
            if (setting == null)
            {

                var page = _GetPageByViewNameAndController("Index", "Home")
                    .Select(p => new Page { Action = p.Action, ControllerName = p.ControllerName, Deleted = p.Deleted, PageId = p.PageId, Title = p.Title })
                    .SingleOrDefault();

                if (page != null)
                {
                    int? pageId = page.PageId;

                    setting = new Setting
                                  {
                                      UserId = user.UserId,
                                      AllowGoogleSync = false,
                                      Deleted = false,
                                      StartPage = pageId,
                                      Updated = DateTime.Now
                                  };
                }

                if (setting != null)
                {
                    _AddSetting(setting.UserId, setting.StartPage, setting.AllowGoogleSync);
                }
            }

            return setting;
        }

        public void SaveUserAccountToken(long userId, Guid token)
        {
            _SaveUserActivationToken(token, userId);
        }

        public bool SaveCardOwnerToken(long cardId, Guid token)
        {
            var result = _UpdateCardOwnerToken(cardId, token).SingleOrDefault();
            return result != null && result.RowsUpdated > 0;
        }

        public bool SaveBusidexUser(BusidexUser user)
        {
            try
            {
                UserAddress address = user.Address;

                #region handle empty values that cannot be null
                if (user.Address == null)
                {
                    user.Address = new UserAddress
                                       {
                                           Address1 = string.Empty,
                                           Address2 = string.Empty,
                                           UserId = user.UserId,
                                           UserAddressId = 0,
                                           City = string.Empty,
                                           State = string.Empty,
                                           Country = string.Empty,
                                           Latitude = 0.0,
                                           Longitude = 0.0,
                                           Region = string.Empty,
                                           ZipCode = string.Empty
                                       };
                }
                if (string.IsNullOrEmpty(address.Address1))
                {
                    address.Address1 = string.Empty;
                }
                if (string.IsNullOrEmpty(address.State))
                {
                    address.State = string.Empty;
                }
                if (string.IsNullOrEmpty(address.City))
                {
                    address.City = string.Empty;
                }
                #endregion

                if (address.UserAddressId == 0)
                {

                    _AddUserAddress(user.UserId, address.Address1, address.Address2, address.City, address.State, address.ZipCode,
                                    address.Region, address.Country, address.Latitude, address.Longitude);
                }
                else
                {
                    _UpdateUserAddress(address.UserAddressId, address.Address1, address.Address2, address.City, address.State, address.ZipCode,
                                       address.Region, address.Country, address.Latitude, address.Longitude);
                }

                _UpdateBusidexUser(user.UserId, user.Email, user.UserName);

                return true;
            }
            catch (Exception ex)
            {
                SaveApplicationError(ex.Message, ex.InnerException != null ? ex.InnerException.Message : string.Empty, ex.StackTrace, user.UserId);
                return false;
            }
        }

        public UserAccount GetUserAccountByToken(Guid token)
        {
            return _GetUserAccountByToken(token).Select(ua => new UserAccount
                                                                  {
                                                                      AccountTypeId = ua.AccountTypeId,
                                                                      Active = ua.Active,
                                                                      Notes = ua.Notes,
                                                                      Created = ua.Created,
                                                                      UserId = ua.UserId,
                                                                      UserAccountId = ua.UserAccountId,
                                                                      ActivationToken = ua.ActivationToken,
                                                                      AccountType = new AccountType
                                                                                        {
                                                                                            AccountTypeId = ua.AccountTypeId,
                                                                                            Active = ua.AccountTypeActive,
                                                                                            Description = ua.Description,
                                                                                            Name = ua.Name,
                                                                                            DisplayOrder = ua.DisplayOrder
                                                                                        }
                                                                  }).SingleOrDefault();
        }

        public UserAccount GetUserAccountByUserId(long userId)
        {
            return _GetUserAccountByUserId(userId).Select(ua => new UserAccount
                                                                    {
                                                                        AccountTypeId = ua.AccountTypeId,
                                                                        Active = ua.Active,
                                                                        Notes = ua.Notes,
                                                                        Created = ua.Created,
                                                                        UserId = ua.UserId,
                                                                        UserAccountId = ua.UserAccountId,
                                                                        ActivationToken = ua.ActivationToken,
                                                                        AccountType = new AccountType
                                                                                          {
                                                                                              AccountTypeId = ua.AccountTypeId,
                                                                                              Active = ua.AccountTypeActive,
                                                                                              Description = ua.Description,
                                                                                              Name = ua.Name,
                                                                                              DisplayOrder = ua.DisplayOrder
                                                                                          }
                                                                    }).SingleOrDefault();
        }

        public UserAccount GetUserAccountByEmail(string email)
        {
            UserAccount userAccount = _GetUserAccountByEmail(email).Select(ua => new UserAccount
                                                                  {
                                                                      AccountTypeId = ua.AccountTypeId,
                                                                      Active = ua.Active,
                                                                      Notes = ua.Notes,
                                                                      Created = ua.Created,
                                                                      UserId = ua.UserId,
                                                                      UserAccountId = ua.UserAccountId,
                                                                      ActivationToken = ua.ActivationToken,
                                                                      AccountType = new AccountType
                                                                                        {
                                                                                            AccountTypeId = ua.AccountTypeId,
                                                                                            Active = ua.AccountTypeActive,
                                                                                            Description = ua.Description,
                                                                                            Name = ua.Name,
                                                                                            DisplayOrder = ua.DisplayOrder
                                                                                        }
                                                                  }).SingleOrDefault();
            if (userAccount != null)
            {
                userAccount.BusidexUser =
                    _GetBusidexUserByUserId(userAccount.UserId)
                        .Select(
                            bu =>
                            new BusidexUser
                                {
                                    UserId = bu.UserId,
                                    UserName = bu.UserName,
                                    ApplicationId = bu.ApplicationId,
                                    IsAnonymous = bu.IsAnonymous,
                                    LastActivityDate = bu.LastActivityDate,
                                    LoweredUserName = bu.LoweredUserName,
                                    MobileAlias = bu.MobileAlias
                                })
                        .SingleOrDefault();

            }
            return userAccount;
        }

        public UserAccount AddUserAccount(UserAccount userAccount)
        {
            UserAccount newAccount = null;
            if (userAccount.UserAccountId == 0)
            {
                var result = _AddUserAccount(userAccount.UserId, userAccount.AccountTypeId, userAccount.Notes,
                                             userAccount.ActivationToken);
                newAccount = result.Select(ua => new UserAccount
                                                     {
                                                         AccountTypeId = ua.AccountTypeId,
                                                         Active = ua.Active,
                                                         ActivationToken = ua.ActivationToken,
                                                         Created = ua.Created,
                                                         Notes = ua.Notes,
                                                         UserId = ua.UserId,
                                                         UserAccountId = ua.UserAccountId,
                                                         AccountType = new AccountType
                                                                           {
                                                                               AccountTypeId = ua.AccountTypeId,
                                                                               Name = ua.Name,
                                                                               Active = ua.AccountTypeActive,
                                                                               Description = ua.Description,
                                                                               DisplayOrder = ua.DisplayOrder
                                                                           }
                                                     }).SingleOrDefault();
                if (newAccount != null)
                {
                    newAccount.AccountType =
                        _GetAccountTypeById(newAccount.AccountTypeId).Select(a => new AccountType
                                                                                      {
                                                                                          AccountTypeId = a.AccountTypeId,
                                                                                          Name = a.Name,
                                                                                          Active = a.Active,
                                                                                          Description = a.Description,
                                                                                          DisplayOrder = a.DisplayOrder
                                                                                      }).SingleOrDefault();
                }
            }
            return newAccount;
        }

        public Setting SaveSetting(Setting setting)
        {
            var result = _AddSetting(setting.UserId, setting.StartPage, setting.AllowGoogleSync).SingleOrDefault();

            return result != null && result.SettingsId > 0
                ? setting
                : null;
        }

        public void UpdateSetting(Setting setting)
        {
            _UpdateSetting(setting.SettingsId, setting.StartPage, setting.AllowGoogleSync);

        }

        public List<Card> GetAllUnownedCards()
        {

            var cardResults = _GetAllUnownedCards().ToList();

            var cardIds = string.Join(",", cardResults.Select(c => c.CardId).ToArray());
            var phoneNumbersDTO = _GetCardPhoneNumbers(cardIds);

            var phoneNumbers = phoneNumbersDTO.Select(phoneNumber => new PhoneNumber
            {
                CardId = phoneNumber.CardId,
                Number = phoneNumber.Number,
                Extension = phoneNumber.Extension,
                PhoneNumberType = new PhoneNumberType
                {
                    Name = phoneNumber.Name,
                    PhoneNumberTypeId = phoneNumber.PhoneNumberTypeId
                }
            }).ToList();

            var cards = cardResults.Select(item => new Card
            {
                BackFileId = item.BackFileId,
                BackOrientation = item.BackOrientation,
                BackType = item.BackType,
                CardId = item.CardId,
                FrontFileId = item.FrontFileId,
                FrontType = item.FrontType,
                FrontOrientation = item.FrontOrientation,
                Email = item.Email,
                Url = item.Url,
                Title = item.Title,
                Name = item.Name,
                OwnerId = item.OwnerId,
                CreatedBy = item.CreatedBy,
                CompanyName = item.CompanyName,
                PhoneNumbers = phoneNumbers.Where(p => p.CardId == item.CardId).ToList()

            }).ToList();

            return cards;
        }

        public bool SaveCardOwner(long cardId, long ownerId)
        {

            bool ok = false;
            try
            {
                Card c = GetCardById(cardId);
                if (c != null)
                {
                    c.Searchable = true; // Now that the card is owned, it is searchable
                    _UpdateCard(c.CardId, c.Name, c.Title, c.FrontImage, c.FrontType, c.FrontOrientation, c.BackImage, c.BackType, c.BackOrientation, c.BusinessId,
                               c.Searchable, c.CompanyName, c.Email, c.Url, c.CreatedBy, ownerId, c.OwnerToken, c.Deleted, c.FrontFileId, c.BackFileId, c.Display.ToString(),
                               c.Markup);

                    ok = true;
                }
            }
            catch (Exception)
            {
                ok = false;
            }

            return ok;
        }

        public void DeleteCard(long id)
        {
            _DeleteCard(id);
        }

        public void SaveApplicationError(string error, string innerException, string stackTrace, long userId)
        {
            _AddApplicationError(error, innerException, stackTrace, userId);
        }

        public EmailTemplate GetEmailTemplate(EmailTemplateCode code)
        {
            return _GetEmailTemplateByCode(code.ToString())
                .Select(t => new EmailTemplate { Body = t.Body, Code = t.Code, EmailTemplateId = t.EmailTemplateId, Subject = t.Subject })
                .SingleOrDefault();
        }

        public void SaveCommunication(Communication communication)
        {
            _SaveCommunication(communication.EmailTemplateId, communication.UserId, communication.Email, communication.SentById, communication.OwnerToken,
                communication.DateSent, communication.Failed);
        }

        public Communication GetCommunicationByActivationToken(string token)
        {
            var gToken = new Guid(token);
            var result = _GetCommunicationByActivationToken(gToken).SingleOrDefault() ?? new usp_GetCommunicationByActivationTokenResult();
            var communication = new Communication
                                {
                                    EmailTemplateId = result.EmailTemplateId.GetValueOrDefault(),
                                    UserId = result.UserId,
                                    Email = result.Email,
                                    DateSent = result.DateSent,
                                    Failed = result.Failed,
                                    SentById = result.SentById.GetValueOrDefault(),
                                    OwnerToken = result.OwnerToken
                                };
            return communication;
        }

        public void SaveSharedCards(List<SharedCard> sharedCards)
        {
            foreach (var sharedCard in sharedCards)
            {
                _AddSharedCard(sharedCard.CardId, sharedCard.SendFrom, sharedCard.Email, sharedCard.ShareWith,
                              sharedCard.SharedDate, sharedCard.Accepted, sharedCard.Declined);
            }
        }

        public List<SharedCard> GetSharedCards(long userId)
        {
            var sharedCards = _GetSharedCardsByUserId(userId).Select(result => new SharedCard
                                                                        {
                                                                            Accepted = result.Accepted,
                                                                            CardId = result.CardId,
                                                                            Declined = result.Declined,
                                                                            Email = result.Email,
                                                                            SendFrom = result.SendFrom,
                                                                            SendFromEmail = result.SendFromEmail,
                                                                            SharedCardId = result.SharedCardId,
                                                                            SharedDate = result.SharedDate,
                                                                            ShareWith = result.ShareWith,
                                                                            Card = GetCardById(result.CardId)
                                                                        }).ToList();

            return sharedCards;
        }

        public void AcceptSharedCard(long cardId, long userId)
        {
            var sharedCards = _GetSharedCardsByUserId(userId).ToList();
            var sharedCard = sharedCards.FirstOrDefault(c => c.CardId == cardId);
            var theirBusidex = GetMyBusidex(userId, false);
            var card = GetCardById(cardId);
            if (sharedCard != null)
            {
                if (theirBusidex.All(c => c.CardId != sharedCard.CardId))
                {
                    _AddUserCard(cardId, userId, card.OwnerId, sharedCard.SendFrom, string.Empty);
                }
                _AcceptSharedCard(userId, cardId);
                _UpdateSharedById(cardId, sharedCard.SendFrom);
            }
        }

        public void DeclineSharedCard(long cardId, long userId)
        {
            var sharedCards = _GetSharedCardsByUserId(userId).ToList();
            var sharedCard = sharedCards.FirstOrDefault(c => c.CardId == cardId);
            if (sharedCard != null)
            {
                _DeclineSharedCard(userId, cardId);
            }
        }

        public void UpdateCardFileId(long cardId, Guid frontFileId, Guid backFileId)
        {
            _UpdateCardFileIds(cardId, frontFileId, backFileId);
        }

        public void UpdateBusidexCache()
        {

        }

        public List<Card> GetAllCards()
        {
            var allCards = _GetAllCards();
            var cards = allCards.Select(item => new Card
                {
                    BackFileId = item.BackFileId,
                    BackOrientation = item.BackOrientation,
                    BackType = item.BackType,
                    CardId = item.CardId,
                    FrontFileId = item.FrontFileId,
                    FrontType = item.FrontType,
                    FrontOrientation = item.FrontOrientation,
                    Email = item.Email,
                    Url = item.Url,
                    Title = item.Title,
                    Name = item.Name,
                    CompanyName = item.CompanyName,
                    Created = item.Created,
                    OwnerId = item.OwnerId,
                    Updated = item.Updated
                }).ToList();
            return cards;
        }

        public List<Card> GetUnownedCards()
        {
            var allCards = _GetUnownedCards();
            var cards = allCards.Select(item => new Card
            {
                BackFileId = item.BackFileId,
                BackOrientation = item.BackOrientation,
                BackType = item.BackType,
                CardId = item.CardId,
                FrontFileId = item.FrontFileId,
                FrontType = item.FrontType,
                FrontOrientation = item.FrontOrientation,
                Email = item.Email,
                Url = item.Url,
                Title = item.Title,
                Name = item.Name,
                PhoneNumbers = _GetCardPhoneNumber(item.CardId).Select(p => new PhoneNumber
                {
                    CardId = p.CardId,
                    Created = p.Created,
                    Deleted = p.Deleted,
                    Extension = p.Extension,
                    Number = p.Number,
                    PhoneNumberTypeId = p.PhoneNumberTypeId,
                    Updated = p.Updated,
                    PhoneNumberId = p.PhoneNumberId,
                    PhoneNumberType = new PhoneNumberType
                    {
                        PhoneNumberTypeId = p.PhoneNumberTypeId,
                        Name = p.Name
                    }
                }).ToList(),
                CompanyName = item.CompanyName,
                Created = item.Created,
                OwnerId = item.OwnerId,
                Updated = item.Updated
            }).ToList();
            return cards;
        }

        public long AddCard(Card card)
        {
            var id = _AddCard(card.Name, card.Title, card.FrontImage, card.FrontType, card.FrontOrientation,
                card.BackImage, card.BackType, card.BackOrientation,
                card.BusinessId, card.Searchable, card.CompanyName, card.Email, card.Url, card.CreatedBy,
                card.OwnerId, card.OwnerToken, card.FrontFileId, card.BackFileId, card.Display.ToString(), card.Markup);

            return id.Single().CardId.GetValueOrDefault();
        }

        public void AddTag(long cardId, Tag tag)
        {
            _AddCardTag(cardId, tag.Text);
        }

        public void DeleteTag(long cardId, long tagId)
        {
            _DeleteCardTag(cardId, tagId);
        }

        public void UpdateAddress(CardAddress address)
        {
            _UpdateCardAddress(address.CardAddressId, address.Address1, address.Address2, address.City, address.State.Code,
                               address.ZipCode, address.Region,
                               address.Country, address.Latitude, address.Longitude);
        }

        public void AddAddress(long cardId, CardAddress address)
        {
            _AddCardAddress(cardId, address.Address1, address.Address2, address.City, address.State.Code, address.ZipCode,
                            address.Region, address.Country, address.Latitude, address.Longitude);
        }

        public void DeleteAddress(long cardAddressId)
        {
            _DeleteCardAddress(cardAddressId);
        }

        public long AddPhoneNumber(PhoneNumber phoneNumber)
        {
            var id = _AddPhoneNumber(phoneNumber.PhoneNumberTypeId, phoneNumber.CardId, phoneNumber.Number,
                                       phoneNumber.Extension);

            return id.Single().PhoneNumberId.GetValueOrDefault();
        }

        public long AddUserCard(UserCard userCard)
        {
            var id = _AddUserCard(userCard.CardId, userCard.UserId, userCard.OwnerId, userCard.SharedById,
                                    userCard.Notes);

            var cache = new BusidexCacheProvider();
            cache.UpdateCache(BusidexCacheProvider.CachKeys.MyBusidex, null); // invalidate the cache

            return id.Single().UserCardId.GetValueOrDefault();
        }

        public long AddGroup(Group group)
        {
            var id = _AddGroup(group.UserId, group.Description, group.Notes);
            var result = id.SingleOrDefault();
            return result != null ? result.GroupId.GetValueOrDefault() : 0;
        }

        public void DeleteGroup(long id)
        {
            _DeleteGroup(id);
        }

        public long UpdateGroup(Group group)
        {
            var id = _UpdateGroup(group.GroupId, group.Description, group.Notes);
            var result = id.SingleOrDefault();
            return result != null ? result.GroupId.GetValueOrDefault() : 0;
        }

        public long AddUserGroupCard(UserGroupCard groupCard)
        {
            var id = _AddUserGroupCard(groupCard.GroupId, groupCard.CardId, groupCard.UserId, groupCard.PersonId, groupCard.SharedById, groupCard.Notes);

            var cache = new BusidexCacheProvider();
            cache.UpdateCache(BusidexCacheProvider.CachKeys.MyBusiGroups, null); // invalidate the cache

            return id.Single().UserGroupCardId.GetValueOrDefault();
        }

        public long AddUserGroupCards(string cardIds, long groupId, long userId)
        {
           return  _AddUserGroupCards(groupId, cardIds, userId);
        }

        public long? RemoveUserGroupCards(string cardIds, long groupId, long userId)
        {
            var id = _RemoveUserGroupCards(groupId, cardIds, userId).SingleOrDefault();
            return id != null ? id.GroupId : 0;
        }

        public void UpdatePhonenumber(PhoneNumber phoneNumber)
        {
            _UpdatePhoneNumber(phoneNumber.PhoneNumberId, phoneNumber.PhoneNumberTypeId, phoneNumber.CardId, phoneNumber.Number, phoneNumber.Extension, phoneNumber.Deleted);
        }

        public void UpdateCard(Card model)
        {
            _UpdateCard(model.CardId, model.Name, model.Title, model.FrontImage, model.FrontType, model.FrontOrientation,
                       model.BackImage, model.BackType, model.BackOrientation, model.BusinessId, model.Searchable, model.CompanyName, model.Email,
                       model.Url, model.CreatedBy, model.OwnerId, model.OwnerToken, false, model.FrontFileId, model.BackFileId, model.Display.ToString(), model.Markup);

        }

        public void UpdateCardBasicInfo(long cardId, string name, string company, string phone, string email)
        {
            _UpdateCardBasicInfo(cardId, name, company, email);

            const int BUSINESS_PHONE = 1;
            var existingPhones = _GetCardPhoneNumber(cardId).ToList();
            if (existingPhones.All(p => p.Number != phone))
            {
                _AddPhoneNumber(BUSINESS_PHONE, cardId, phone, string.Empty);
            }
        }

        public void UpdateUserCard(long userCardId, string notes)
        {
            _UpdateUserCard(userCardId, notes);
        }

        public void UpdateUserGroupCard(long userGroupCardId, string notes)
        {
            _UpdateUserGroupCard(userGroupCardId, notes);
        }

        public List<StateCode> GetAllStateCodes()
        {
            return _GetAllStateCodes().Select(s => new StateCode { StateCodeId = s.StateCodeId, Code = s.Code, Name = s.Name }).ToList();
        }

        public List<Card> GetDuplicateCardsByEmail(long cardId, string email)
        {
            var duplicates = _GetDuplicateCardsByEmail(cardId, email).Select(item => new Card
                                                                                         {
                                                                                             BackFileId = item.BackFileId,
                                                                                             BackOrientation = item.BackOrientation,
                                                                                             BackType = item.BackType,
                                                                                             CardId = item.CardId,
                                                                                             FrontFileId = item.FrontFileId,
                                                                                             FrontType = item.FrontType,
                                                                                             FrontOrientation = item.FrontOrientation,
                                                                                             Email = item.Email,
                                                                                             Url = item.Url,
                                                                                             Searchable = item.Searchable,
                                                                                             Title = item.Title,
                                                                                             Name = item.Name,
                                                                                             PhoneNumbers = _GetCardPhoneNumber(item.CardId).Select(p => new PhoneNumber
                                                                                                                                                             {
                                                                                                                                                                 CardId = p.CardId,
                                                                                                                                                                 Created = p.Created,
                                                                                                                                                                 Deleted = p.Deleted,
                                                                                                                                                                 Extension = p.Extension,
                                                                                                                                                                 Number = p.Number,
                                                                                                                                                                 PhoneNumberTypeId = p.PhoneNumberTypeId,
                                                                                                                                                                 Updated = p.Updated,
                                                                                                                                                                 PhoneNumberId = p.PhoneNumberId,
                                                                                                                                                                 PhoneNumberType = new PhoneNumberType
                                                                                                                                                                                       {
                                                                                                                                                                                           PhoneNumberTypeId = p.PhoneNumberTypeId,
                                                                                                                                                                                           Name = p.Name
                                                                                                                                                                                       }
                                                                                                                                                             }).ToList(),
                                                                                             CompanyName = item.CompanyName
                                                                                         }).ToList();
            return duplicates;
        }

        public List<Suggestion> GetAllSuggestions()
        {
            var results = _GetAllSuggestions();
            return results.Select(s => new Suggestion
                                           {
                                               SuggestionId = s.SuggestionId,
                                               Summary = s.Summary,
                                               Details = s.Details,
                                               Votes = s.Votes,
                                               CreatedBy = s.CreatedBy,
                                               Created = s.Created,
                                               Deleted = s.Deleted
                                           }).ToList();

        }

        public void AddNewSuggestion(Suggestion suggestion)
        {
            _AddNewSuggestion(suggestion.Summary, suggestion.Details, suggestion.CreatedBy);
        }

        public void UpdateSuggestionVoteCount(int suggestionId)
        {
            _UpdateSuggestionVoteCount(suggestionId);
        }

        public void UpdateMobileView(long id, bool isMobileView)
        {
            _UpdateMobileView(id, isMobileView);
        }

        public List<Group> GetMyBusiGroups(long userId)
        {
            var groups = _GetMyBusigroups(userId).ToList();
            return groups.Select(g => new Group
                                 {
                                     Description = g.Description,
                                     GroupId = g.GroupId,
                                     Notes = g.Notes,
                                     Created = g.Created,
                                     Updated = g.Updated,
                                     UserId = g.UserId
                                 }).ToList();
        }

        public Group GetBusiGroupById(long groupId)
        {
            var result = _GetBusigroupById(groupId).SingleOrDefault();

            if (result != null)
            {
                return new Group
                       {
                           Created = result.Created,
                           Description = result.Description,
                           GroupId = result.GroupId,
                           Notes = result.Notes,
                           Updated = result.Updated,
                           UserId = result.UserId
                       };
            }
            return null;
        }

        public List<UserGroupCard> GetBusiGroupCards(long groupId, bool includeImages)
        {
            var results = _GetBusigroup(groupId, includeImages).ToList();
            var cardIds = string.Join(",", results.Select(c => c.CardId).ToArray());
            var phoneNumbersDTO = _GetCardPhoneNumbers(cardIds);

            var phoneNumbers = phoneNumbersDTO.Select(phoneNumber => new PhoneNumber
            {
                CardId = phoneNumber.CardId,
                Number = phoneNumber.Number,
                Extension = phoneNumber.Extension,
                PhoneNumberType = new PhoneNumberType
                {
                    Name = phoneNumber.Name,
                    PhoneNumberTypeId = phoneNumber.PhoneNumberTypeId
                }
            }).ToList();


            var cards = results.Select(item => new UserGroupCard
                                               {
                                                   UserGroupCardId = item.UserGroupCardId,
                                                   GroupId = item.GroupId,
                                                   Notes = item.Notes,
                                                   CardId = item.CardId,
                                                   Card = new Card
                                                          {
                                                              BackFileId = item.BackFileId,
                                                              BackOrientation = item.BackOrientation,
                                                              BackType = item.BackType,
                                                              CardId = item.CardId,
                                                              FrontFileId = item.FrontFileId,
                                                              FrontType = item.FrontType,
                                                              FrontOrientation = item.FrontOrientation,
                                                              Email = item.Email,
                                                              Url = item.Url,
                                                              Title = item.Title,
                                                              Name = item.Name,
                                                              OwnerId = item.OwnerId,
                                                              CreatedBy = item.CreatedBy,
                                                              CompanyName = item.CompanyName,
                                                              PhoneNumbers = phoneNumbers.Where(p => p.CardId == item.CardId).ToList(),
                                                              FrontImage = includeImages ? item.FrontImage : null,
                                                              BackImage = includeImages ? item.BackImage : null,
                                                          }
                                               }).ToList();


            var tags = _GetCardTagsByIds(string.Join(",", cards.Select(c => c.CardId))).ToList();
            foreach (var userCard in cards)
            {
                userCard.Card.Tags = (from tag in tags
                                      where tag.CardId == userCard.CardId
                                      select new Tag
                                      {
                                          TagId = tag.TagId,
                                          Text = tag.Text
                                      }).ToList();
            }
            return cards.ToList();
        }

        [FunctionAttribute(Name = "dbo.usp_getMyBusidex")]
        [ResultType(typeof(CardRelation))]
        [ResultType(typeof(usp_getMyBusidexResult))]
        public IMultipleResults _GetMyBusidex([ParameterAttribute(DbType = "BigInt")] long? userId, [ParameterAttribute(DbType = "Bit")] bool? includeImages)
        {
            IExecuteResult result = ExecuteMethodCall(this, ((MethodInfo)(MethodBase.GetCurrentMethod())), userId, includeImages);
            if (result != null)
            {
                return ((IMultipleResults) (result.ReturnValue));
            }
            return null;
        }


        public partial class usp_getMyBusidexResult
        {

            private long _UserCardId;

            private Nullable<long> _SharedById;

            private string _Notes;

            private long _CardId;

            private string _Name;

            private string _Title;

            private string _FrontType;

            private string _FrontOrientation;

            private string _BackType;

            private string _BackOrientation;

            private bool _Searchable;

            private string _CompanyName;

            private string _Email;

            private string _Url;

            private Nullable<long> _CreatedBy;

            private Nullable<long> _OwnerId;

            private bool _Deleted;

            private Nullable<Guid> _FrontFileId;

            private Nullable<Guid> _BackFileId;

            private Binary _FrontImage;

            private Binary _BackImage;

            private bool _MobileView;

            [ColumnAttribute(Storage = "_UserCardId", DbType = "BigInt NOT NULL")]
            public long UserCardId
            {
                get
                {
                    return this._UserCardId;
                }
                set
                {
                    if ((this._UserCardId != value))
                    {
                        this._UserCardId = value;
                    }
                }
            }

            [ColumnAttribute(Storage = "_SharedById", DbType = "BigInt")]
            public System.Nullable<long> SharedById
            {
                get
                {
                    return this._SharedById;
                }
                set
                {
                    if ((this._SharedById != value))
                    {
                        this._SharedById = value;
                    }
                }
            }

            [ColumnAttribute(Storage = "_Notes", DbType = "VarChar(MAX)")]
            public string Notes
            {
                get
                {
                    return this._Notes;
                }
                set
                {
                    if ((this._Notes != value))
                    {
                        this._Notes = value;
                    }
                }
            }

            [ColumnAttribute(Storage = "_CardId", DbType = "BigInt NOT NULL")]
            public long CardId
            {
                get
                {
                    return this._CardId;
                }
                set
                {
                    if ((this._CardId != value))
                    {
                        this._CardId = value;
                    }
                }
            }

            [ColumnAttribute(Storage = "_Name", DbType = "VarChar(150)")]
            public string Name
            {
                get
                {
                    return this._Name;
                }
                set
                {
                    if ((this._Name != value))
                    {
                        this._Name = value;
                    }
                }
            }

            [ColumnAttribute(Storage = "_Title", DbType = "VarChar(150)")]
            public string Title
            {
                get
                {
                    return this._Title;
                }
                set
                {
                    if ((this._Title != value))
                    {
                        this._Title = value;
                    }
                }
            }

            [ColumnAttribute(Storage = "_FrontType", DbType = "VarChar(10) NOT NULL", CanBeNull = false)]
            public string FrontType
            {
                get
                {
                    return this._FrontType;
                }
                set
                {
                    if ((this._FrontType != value))
                    {
                        this._FrontType = value;
                    }
                }
            }

            [ColumnAttribute(Storage = "_FrontOrientation", DbType = "VarChar(1)")]
            public string FrontOrientation
            {
                get
                {
                    return this._FrontOrientation;
                }
                set
                {
                    if ((this._FrontOrientation != value))
                    {
                        this._FrontOrientation = value;
                    }
                }
            }

            [ColumnAttribute(Storage = "_BackType", DbType = "VarChar(10)")]
            public string BackType
            {
                get
                {
                    return this._BackType;
                }
                set
                {
                    if ((this._BackType != value))
                    {
                        this._BackType = value;
                    }
                }
            }

            [ColumnAttribute(Storage = "_BackOrientation", DbType = "VarChar(1) NOT NULL", CanBeNull = false)]
            public string BackOrientation
            {
                get
                {
                    return this._BackOrientation;
                }
                set
                {
                    if ((this._BackOrientation != value))
                    {
                        this._BackOrientation = value;
                    }
                }
            }

            [ColumnAttribute(Storage = "_Searchable", DbType = "Bit NOT NULL")]
            public bool Searchable
            {
                get
                {
                    return this._Searchable;
                }
                set
                {
                    if ((this._Searchable != value))
                    {
                        this._Searchable = value;
                    }
                }
            }

            [ColumnAttribute(Storage = "_CompanyName", DbType = "VarChar(150)")]
            public string CompanyName
            {
                get
                {
                    return this._CompanyName;
                }
                set
                {
                    if ((this._CompanyName != value))
                    {
                        this._CompanyName = value;
                    }
                }
            }

            [ColumnAttribute(Storage = "_Email", DbType = "VarChar(150)")]
            public string Email
            {
                get
                {
                    return this._Email;
                }
                set
                {
                    if ((this._Email != value))
                    {
                        this._Email = value;
                    }
                }
            }

            [ColumnAttribute(Storage = "_Url", DbType = "VarChar(250)")]
            public string Url
            {
                get
                {
                    return this._Url;
                }
                set
                {
                    if ((this._Url != value))
                    {
                        this._Url = value;
                    }
                }
            }

            [ColumnAttribute(Storage = "_CreatedBy", DbType = "BigInt")]
            public System.Nullable<long> CreatedBy
            {
                get
                {
                    return this._CreatedBy;
                }
                set
                {
                    if ((this._CreatedBy != value))
                    {
                        this._CreatedBy = value;
                    }
                }
            }

            [ColumnAttribute(Storage = "_OwnerId", DbType = "BigInt")]
            public System.Nullable<long> OwnerId
            {
                get
                {
                    return this._OwnerId;
                }
                set
                {
                    if ((this._OwnerId != value))
                    {
                        this._OwnerId = value;
                    }
                }
            }

            [ColumnAttribute(Storage = "_Deleted", DbType = "Bit NOT NULL")]
            public bool Deleted
            {
                get
                {
                    return this._Deleted;
                }
                set
                {
                    if ((this._Deleted != value))
                    {
                        this._Deleted = value;
                    }
                }
            }

            [ColumnAttribute(Storage = "_FrontFileId", DbType = "UniqueIdentifier")]
            public System.Nullable<System.Guid> FrontFileId
            {
                get
                {
                    return this._FrontFileId;
                }
                set
                {
                    if ((this._FrontFileId != value))
                    {
                        this._FrontFileId = value;
                    }
                }
            }

            [ColumnAttribute(Storage = "_BackFileId", DbType = "UniqueIdentifier")]
            public System.Nullable<System.Guid> BackFileId
            {
                get
                {
                    return this._BackFileId;
                }
                set
                {
                    if ((this._BackFileId != value))
                    {
                        this._BackFileId = value;
                    }
                }
            }

            [ColumnAttribute(Storage = "_FrontImage", DbType = "VarBinary(MAX)")]
            public System.Data.Linq.Binary FrontImage
            {
                get
                {
                    return this._FrontImage;
                }
                set
                {
                    if ((this._FrontImage != value))
                    {
                        this._FrontImage = value;
                    }
                }
            }

            [ColumnAttribute(Storage = "_BackImage", DbType = "VarBinary(MAX)")]
            public System.Data.Linq.Binary BackImage
            {
                get
                {
                    return this._BackImage;
                }
                set
                {
                    if ((this._BackImage != value))
                    {
                        this._BackImage = value;
                    }
                }
            }

            [ColumnAttribute(Storage = "_MobileView", DbType = "Bit NOT NULL")]
            public bool MobileView
            {
                get
                {
                    return this._MobileView;
                }
                set
                {
                    if ((this._MobileView != value))
                    {
                        this._MobileView = value;
                    }
                }
            }
        }

    }

    public partial class BusidexDataContext
    {
       
    }
}
