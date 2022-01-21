using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Busidex.Api.DataAccess.DTO;
using Busidex.Api.Models;

namespace Busidex.Api.DataAccess
{
    [DbConfigurationType(typeof(BusidexDatabaseConfiguration))]
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
        }

        public IEnumerable<DTO.AccountType> GetActivePlans()
        {
            return _GetActivePlans().Select(p => new DTO.AccountType
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

        public bool IsACardOwner(long ownerId)
        {
            var cardsByOwner = _GetCardsByOwnerId(ownerId).ToList();
            return cardsByOwner.Any();
        }

        
        public List<CardDetailModel> GetCardsByOwnerId(long ownerId)
        {
            var cardsByOwner = _GetCardsByOwnerId(ownerId).ToList();
            var stateCodes = GetAllStateCodes();
            
            var results = (from result in cardsByOwner
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
                                                            Text = t.Text,
                                                            TagType = (TagType)t.TagTypeId
                                                        }).ToList()
                    let addresses = _GetCardAddresses(result.CardId)                    
                    let cardAddresses = addresses.Select(a => new DTO.CardAddress
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
                    let links = usp_GetExternalLinksByCardId(result.CardId)
                    .Select(link => new DTO.ExternalLink
                    {
                        CardId = link.CardId,
                        Link = link.Link,
                        ExternalLinkId = link.ExternalLinkId,
                        ExternalLinkTypeId = link.ExternalLinkTypeId
                    }).ToList()

                    select new CardDetailModel(new DTO.Card
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
                                   CreatedBy = result.CreatedBy.GetValueOrDefault(),
                                   CompanyName = result.CompanyName,
                                   PhoneNumbers = phoneNumbers.ToList(),
                                   Tags = cardTags.ToList(),
                                   Addresses = cardAddresses.ToList(),
                                   Display = result.DisplayType == null ? DisplayType.IMG : (DisplayType)Enum.Parse(typeof(DisplayType), result.DisplayType),
                                   Markup = result.Markup,
                                   ExternalLinks = links
                               })).ToList();

            results.ForEach(c=>c.HasBackImage = cardsByOwner.Single(o=>o.CardId == c.CardId).HasBackImage != 0);
            return results;
        }

        public void UpdateUserAccount(long userId, int accountTypeId)
        {
            _UpdateUserAccount(userId, accountTypeId);
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

        public void SaveUserAccountCode(long userId, string code)
        {
            _SaveUserActivationCode(code, userId);
        }

        public bool SaveCardOwnerToken(long cardId, Guid token)
        {
            var result = _UpdateCardOwnerToken(cardId, token).SingleOrDefault();
            return result != null && result.RowsUpdated > 0;
        }

        public bool UpdateUserName(long userId, string newUserName)
        {
            var result = _UpdateUserName(userId, newUserName).SingleOrDefault();

            return result != null && result.UserId > 0;
        }

        public bool SaveUserPassword(string userName, string password)
        {
            return _UpdatePassword(password, userName) > 0;
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
                    address = user.Address;
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

        public UserAccount GetUserAccountByCode(string code)
        {
            var userAccount = _GetUserAccountByCode(code);
            if (userAccount == null)
            {
                return null;
            }
            return userAccount.Select(ua => new UserAccount
            {
                AccountTypeId = ua.AccountTypeId,
                Active = ua.Active,
                Notes = ua.Notes,
                Created = ua.Created,
                UserId = ua.UserId,
                UserAccountId = ua.UserAccountId,
                DisplayName = ua.DisplayName,
                ActivationToken = ua.ActivationToken,
                AccountType = new DTO.AccountType
                {
                    AccountTypeId = ua.AccountTypeId,
                    Active = ua.AccountTypeActive,
                    Description = ua.Description,
                    Name = ua.Name,
                    DisplayOrder = ua.DisplayOrder
                }
            }).SingleOrDefault();
        }

        public UserAccount GetUserAccountByToken(Guid token)
        {
            var userAccount = _GetUserAccountByToken(token);
            if (userAccount == null)
            {
                return null;
            }
            return userAccount.Select(ua => new UserAccount
            {
                AccountTypeId = ua.AccountTypeId,
                Active = ua.Active,
                Notes = ua.Notes,
                Created = ua.Created,
                UserId = ua.UserId,
                UserAccountId = ua.UserAccountId,
                DisplayName = ua.DisplayName,
                ActivationToken = ua.ActivationToken,
                AccountType = new DTO.AccountType
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
                                                                      DisplayName = ua.DisplayName,
                                                                      AccountType = new DTO.AccountType
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

        public List<SystemSettingDto> GetSystemSettings()
        {
            var settings = _GetSystemSettings();
            return settings.Select(s => new SystemSettingDto
            {
                SystemSettingId = s.SystemSettingId,
                Setting = s.Setting,
                Value = s.Value
            }).ToList();
        }

        public List<UnownedCard> GetAllUnownedCards()
        {

            var cardResults = _GetAllUnownedCards().ToList();

            //var cardIds = string.Join(",", cardResults.Select(c => c.CardId).ToArray());
            //var phoneNumbersDto = _GetCardPhoneNumbers(cardIds);

            //var phoneNumbers = phoneNumbersDto.Select(phoneNumber => new PhoneNumber
            //{
            //    CardId = phoneNumber.CardId,
            //    Number = phoneNumber.Number,
            //    Extension = phoneNumber.Extension,
            //    PhoneNumberType = new PhoneNumberType
            //    {
            //        Name = phoneNumber.Name,
            //        PhoneNumberTypeId = phoneNumber.PhoneNumberTypeId
            //    }
            //}).ToList();

            var cards = cardResults.Select(item => new UnownedCard
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
                Created = item.Created,
                Updated = item.Updated,
                CreatedBy = item.CreatedBy,
                CompanyName = item.CompanyName,
                PhoneNumbers = new List<PhoneNumber>(),// phoneNumbers.Where(p => p.CardId == item.CardId).ToList(),
                LastContactDate = item.DateSent,
                EmailSentTo = item.SentTo

            }).ToList();

            return cards;
        }

        public void DeleteCard(long id)
        {
            _DeleteCard(id);
        }

        public void SaveApplicationError(string error, string innerException, string stackTrace, long userId)
        {
            _AddApplicationError(error, innerException, stackTrace, userId);
        }

        //public EmailTemplate GetEmailTemplate(EmailTemplateCode code)
        //{
        //    return _GetEmailTemplateByCode(code.ToString())
        //        .Select(t => new EmailTemplate { Body = t.Body, Code = t.Code, EmailTemplateId = t.EmailTemplateId, Subject = t.Subject })
        //        .SingleOrDefault();
        //}

        //public void SaveCommunication(Communication communication)
        //{
        //    _SaveCommunication(communication.EmailTemplateId, communication.UserId, communication.Email, communication.SentById, communication.OwnerToken,
        //        communication.DateSent, communication.Failed);
        //}

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

        public void UpdateCardFileId(long cardId, Guid frontFileId, string frontType, Guid backFileId, string backType)
        {
            usp_UpdateCardFileId(cardId, frontFileId, frontType, backFileId, backType);
        }

        public List<DTO.Card> GetAllCards()
        {
            var allCards = _GetAllCards();
            var cards = allCards.Select(item => new DTO.Card
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

        public List<UnownedCard> GetUnownedCards()
        {
            var allCards = _GetUnownedCards();
            var cards = allCards.Select(item => new UnownedCard
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

        public long AddCard(DataAccess.DTO.Card card)
        {
            byte visibility = 1;
            switch (card.Visibility)
            {
                case 1:
                    visibility = 1;
                    break;
                case 2:
                    visibility = 2;
                    break;
                case 3:
                    visibility = 3;
                    break;
            }
            var id = _AddCard(card.Name, card.Title, card.FrontImage, card.FrontType, card.FrontOrientation,
                card.BackImage, card.BackType, card.BackOrientation,
                card.BusinessId, card.Searchable, card.CompanyName, card.Email, card.Url, card.CreatedBy,
                card.OwnerId, card.OwnerToken, card.FrontFileId, card.BackFileId, card.Display.ToString(), card.Markup, visibility, 1);

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

        public UserCard GetUserCard(long id, long userId)
        {
            var uc = usp_GetUserCard(id, userId).FirstOrDefault();
            var result = new UserCard
            {
                CardId = uc.CardId,
                Created = uc.Created,
                Deleted = uc.Deleted,
                Notes = uc.Notes,
                OwnerId = uc.OwnerId,
                UserId = uc.UserId,
                UserCardId = uc.UserCardId,
                SharedById = uc.SharedById
            };
            return result;
        }

        public void AddUserCard(UserCard uc)
        {
            usp_AddUserCard(uc.CardId, uc.UserId, uc.OwnerId, uc.SharedById, uc.Notes, (int)UserCardAddStatus.Accepted);
        }

        public DTO.Card GetCardById(long? id, long? userId)
        {
            var result = _GetCardById(id).SingleOrDefault();
            return _populateCard(result);
        }

        private DTO.Card _populateCard(usp_getCardByIdResult c)
        {
            DTO.Card card = new DTO.Card();
            card.BackFileId = c.BackFileId;
            card.BackImage = c.BackImage.ToArray();
            card.BackOrientation = c.BackOrientation;
            card.BackType = c.BackType;
            card.Title = c.Title;
            card.CardId = c.CardId;
            card.CardType = CardType.Professional;
            card.CompanyName = c.CompanyName;
            card.CreatedBy = c.CreatedBy;
            card.Deleted = c.Deleted;
            card.Email = c.Email;
            card.FrontFileId = c.FrontFileId;
            card.FrontImage = c.FrontImage.ToArray();
            card.FrontOrientation = c.FrontOrientation;
            card.FrontType = c.FrontType;
            card.Name = c.Name;
            card.OwnerId = c.OwnerId;
            card.Searchable = c.Searchable;
            card.Title = c.Title;
            card.Url = c.Url;
            //card.Visibility = (byte)c.Visibility;

            var cardPhoneNumbers = _GetCardPhoneNumber(card.CardId);
            var phoneNumberTypes = GetAllPhoneNumberTypes().ToDictionary(t => t.PhoneNumberTypeId, t => t.Name);

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
                    Name = phoneNumberTypes[p.PhoneNumberTypeId]
                }
            }).ToList();

            var tags = GetCardTags(card.CardId);
            var cardTags = tags.Select(t => new Tag
            {
                TagId = t.TagId,
                Text = t.Text,
                TagType = (TagType)t.TagTypeId
            }).ToList();

            var addresses = _GetCardAddresses(card.CardId);
            var stateCodes = GetAllStateCodes();
            var cardAddresses = addresses.Select(a => new DTO.CardAddress
            {
                Address1 = a.Address1,
                Address2 = a.Address2,
                CardAddressId = a.CardAddressId,
                CardId = a.CardId,
                City = a.City,
                Country = a.Country,
                Deleted = a.Deleted,
                Region = a.Region,
                State = stateCodes.SingleOrDefault(sc => sc.Name == a.State),
                ZipCode = a.ZipCode
            }).ToList();

            var links = GetExternalLinks(card.CardId);

            card.PhoneNumbers = phoneNumbers;
            card.Tags = cardTags;
            card.Addresses = cardAddresses;
            card.ExternalLinks = links;

            return card;
        }

        public List<DTO.ExternalLink> GetExternalLinks(long cardId)
        {
            var links = usp_GetExternalLinksByCardId(cardId)
                .Select(link => new DTO.ExternalLink
                {
                    Link = link.Link,
                    ExternalLinkId = link.ExternalLinkId,
                    ExternalLinkTypeId = link.ExternalLinkTypeId,
                    ExternalLinkType = new DTO.ExternalLinkType
                    {
                        ExternalLinkTypeId = link.ExternalLinkTypeId,
                        LinkType = link.LinkType,
                    },
                    CardId = link.CardId,
                }).ToList();
            return links;
        }

        public void UpdateCardOrientation(long cardId, string frontOrientation, string backOrientation)
        {
            usp_UpdateCardOrientation(cardId, frontOrientation, backOrientation);
        }

        public void UpdateAddress(DTO.CardAddress address)
        {
            _UpdateCardAddress(address.CardAddressId, address.Address1, address.Address2, address.City,
                address.State != null ? address.State.Code : "",
                address.ZipCode, address.Region,
                address.Country, address.Latitude, address.Longitude);
        }

        public void AddAddress(long cardId, DTO.CardAddress address)
        {
            _AddCardAddress(cardId, address.Address1, address.Address2, address.City, address.State != null ? address.State.Code : string.Empty, address.ZipCode,
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

        public long AddGroup(Group group)
        {
            var id = _AddGroup(group.OwnerId, group.GroupTypeId, group.Description, group.Notes);
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

        public List<Tag> GetCardTags(long cardId)
        {
            return _GetCardTags(cardId).Select(t => new Tag
            {
                TagId = t.TagId,
                Text = t.Text,
                Deleted = false,
                TagType = (TagType)t.TagTypeId
            }).ToList();
        }

        public void UpdatePhonenumber(PhoneNumber phoneNumber)
        {
            _UpdatePhoneNumber(phoneNumber.PhoneNumberId, phoneNumber.PhoneNumberTypeId, phoneNumber.CardId, phoneNumber.Number, phoneNumber.Extension, phoneNumber.Deleted);
        }

        public void UpdateCard(DataAccess.DTO.Card model)
        {
            byte visibility = 1;
            switch (model.Visibility)
            {
                case 1:
                    visibility = 1;
                    break;
                case 2:
                    visibility = 2;
                    break;
                case 3:
                    visibility = 3;
                    break;
            }
            _UpdateCard(model.CardId, model.Name, model.Title, model.FrontImage, model.FrontType, model.FrontOrientation,
                       model.BackImage, model.BackType, model.BackOrientation, model.BusinessId, model.Searchable, model.CompanyName, model.Email,
                       model.Url, model.CreatedBy, model.OwnerId, model.OwnerToken, false, model.FrontFileId, model.BackFileId, model.Display.ToString(), 
                       model.Markup, visibility);

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

        public List<DTO.Card> GetDuplicateCardsByEmail(long cardId, string email)
        {
            var duplicates = _GetDuplicateCardsByEmail(cardId, email).Select(item => new DTO.Card
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
                                               Deleted = s.Deleted,
                                               Done = s.Done.GetValueOrDefault()
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
                                     OwnerId = g.UserId
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
                           OwnerId = result.UserId
                       };
            }
            return null;
        }

        public List<UserGroupCard> GetBusiGroupCards(long groupId, bool includeImages)
        {
            var results = _GetBusigroup(groupId, includeImages).ToList();
            var cardIds = string.Join(",", results.Select(c => c.CardId).ToArray());
            var phoneNumbersDto = _GetCardPhoneNumbers(cardIds);

            var phoneNumbers = phoneNumbersDto.Select(phoneNumber => new PhoneNumber
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
                                                   Card = new DTO.Card
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
                                                              FrontImage = includeImages ? item.FrontImage.ToArray() : null,
                                                              BackImage = includeImages ? item.BackImage.ToArray() : null,
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
                                          Text = tag.Text,
                                          TagType = (TagType)tag.TagTypeId
                                      }).ToList();
            }
            return cards.ToList();
        }

        public List<EventSource> GetAllEventSources()
        {
            return _GetAllEventSources().Select(e => new EventSource
            {
                EventSourceId = e.EventSourceId,
                EventCode = e.EventCode,
                Description = e.Description,
                Active = e.Active
            }).ToList();
        } 

        public void AddEventActivity(EventActivity activity)
        {
            _AddEventActivity(activity.EventSourceId, activity.CardId, activity.UserId, activity.ActivityDate);
        }

        public List<EventActivity> GetEventActivities(long cardId, byte month)
        {
            return _GetEventActivities(cardId, month).Select(a => new EventActivity
            {
                EventCode = a.EventCode,
                CardId = cardId,
                UserId = a.UserId,
                EventActivityId = a.EventActivityId,
                EventSourceId = a.EventSourceId,
                Description = a.Description,
                ActivityDate = a.ActivityDate
            }).ToList();
        }
    }

    public partial class BusidexDataContext
    {
       
    }
}
