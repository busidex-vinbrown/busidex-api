using System;
using System.Collections.Generic;
using System.Data.Entity.Core.EntityClient;
using System.Linq;
using Busidex.Api.DataAccess.DTO;
using Busidex.Api.Models;
using AccountType = Busidex.Api.DataAccess.DTO.AccountType;
using ApplicationError = Busidex.Api.DataAccess.DTO.ApplicationError;
using PhoneNumber = Busidex.Api.DataAccess.DTO.PhoneNumber;

namespace Busidex.Api.DataAccess
{
    public class BusidexDao
    {
        private readonly string _connectionString;

        public BusidexDao() { _connectionString = null; }
        public BusidexDao(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Communication> GetCommunications(string[] emails, long userId)
        {
            using (var ctx = new busidexEntities())
            {
                return
                    ctx.Database.SqlQuery<Communication>(
                        "exec dbo.usp_GetCommunications @EmailList={0}, @SentById={1}", string.Join(",", emails), userId)
                        .ToList();
            }
        }

        public void SaveCommunication(Communication communication)
        {
            using (var ctx = new busidexEntities()) 
            {

                ctx.Database.ExecuteSqlCommand(
                    "exec dbo.usp_SaveCommunications @EmailTemplateId={0}, @UserId={1}, @Email={2}, @Body={3}, @SentById={4}, @OwnerToken={5}, @DateSent={6}, @Failed={7}",
                    communication.EmailTemplateId, communication.UserId, communication.Email, communication.Body, communication.SentById, communication.OwnerToken, communication.DateSent, communication.Failed);
            }
        }

        public EmailTemplate GetEmailTemplate(EmailTemplateCode code)
        {
            using (var ctx = new busidexEntities())
            {
                return ctx.Database.SqlQuery<EmailTemplate>(
                        "exec dbo.usp_GetEmailTemplateByCode @code={0}", code.ToString())
                        .SingleOrDefault();
            }         
        }

        public List<DeviceSummary> GetDeviceSummary()
        {
            using (var ctx = new busidexEntities())
            {
                return ctx.Database.SqlQuery<DeviceSummary>("exec dbo.usp_GetDeviceCount").ToList();
            }
        }

        public List<DeviceDetail> GetDeviceDetails()
        {
            using (var ctx = new busidexEntities())
            {
                return ctx.Database.SqlQuery<DeviceDetail>("exec dbo.usp_GetDeviceDetails").ToList();
            }
        }

        public List<EventTag> GetEventTags()
        {
            using (var ctx = new busidexEntities())
            {
                return ctx.Database.SqlQuery<EventTag>("exec dbo.usp_GetEventTags").ToList();
            }
        }

        public List<Tag> GetSystemTags()
        {
            using (var ctx = new busidexEntities())
            {
                return ctx.Database.SqlQuery<Tag>("exec dbo.usp_GetSystemTags").ToList();
            }
        }

        public CardDetailModel GetFeaturedCard()
        {
            using (var ctx = new busidexEntities())
            {
                return ctx.Database.SqlQuery<CardDetailModel>("exec dbo.usp_GetFeaturedCard").SingleOrDefault();
            }

        }

        public OrgCardDetailModel GetOrganizationCardByOwnerId(long ownerId)
        {
            using (var ctx = new busidexEntities())
            {
                return ctx.Database.SqlQuery<OrgCardDetailModel>("exec dbo.usp_GetOrganizationCardByOwnerId @OwnerId={0}", ownerId).SingleOrDefault();
            }

        }

        public List<Tag> GetUserAccountTags(long userId)
        {
            using (var ctx = new busidexEntities())
            {
                var results = ctx.Database.SqlQuery<Tag>("exec dbo.usp_GetUserAccountTags @UserId={0}", userId);
                return results.Select(t => new Tag
                {
                    TagId = t.TagId,
                    Text = t.Text,
                    TagType = t.TagType
                }).ToList();
            }
        }

        public bool UpdateDisplayName(long userAccountId, string name)
        {
            using (var ctx = new busidexEntities())
            {
                ctx.Database.ExecuteSqlCommand("exec dbo.usp_UpdateDisplayName @UserAccountId={0}, @DisplayName={1}", userAccountId, name);
            }
            return true;
        }

        public Tag GetTag(TagType type, string tag)
        {
            using (var ctx = new busidexEntities())
            {
                return
                    ctx.Database.SqlQuery<Tag>("exec dbo.usp_GetTag @TagTypeId={0}, @Tag={1}", (int) type, tag)
                        .SingleOrDefault();
            }
        }

        public void AddTag(long cardId, Tag tag)
        {
            if (!string.IsNullOrEmpty(tag.Text) && tag.Text.Trim().Equals("NEST", StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }

            using (var ctx = new busidexEntities())
            {
                ctx.Database.ExecuteSqlCommand(
                    "exec dbo.usp_AddCardTag @CardId={0}, @TagText={1}",
                    cardId, tag.Text);
            }
        }

        public void AddTag(string text, int type)
        {
            if (!string.IsNullOrEmpty(text) && text.Trim().Equals("NEST", StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }

            using (var ctx = new busidexEntities())
            {
                ctx.Database.ExecuteSqlCommand("exec dbo.usp_AddTag @Text={0}, @TagTypeId={1}", text, type);
            }
        }

        public void AddSystemTagToCard(long cardid, string tag)
        {
            using (var ctx = new busidexEntities())
            {
                ctx.Database.ExecuteSqlCommand("exec dbo.usp_AddCardTag @CardId={0}, @TagText={1}", cardid, tag);
            }  
        }

        public long AddUserAccountTag(long userAccountId, long tagId)
        {
            using (var ctx = new busidexEntities())
            {
                return ctx.Database.ExecuteSqlCommand("exec dbo.usp_AddUserAccountTag @UserAccountId={0}, @TagId={1}",
                    userAccountId, tagId);
            }
        }

        public List<ApplicationError> GetApplicationErrors(int daysBack)
        {
            using (var ctx = new busidexEntities())
            {
                return
                    ctx.Database.SqlQuery<ApplicationError>("exec dbo.usp_GetApplicationErrors @DaysBack={0}", daysBack)
                        .ToList();
            }
        }

        public UserAuth GetUserAuth(Guid token)
        {
            using (var ctx = new busidexEntities())
            {
                return ctx.Database.SqlQuery<UserAuth>("exec dbo.usp_GetUserAuth @Token={0}", token).SingleOrDefault();
            }
        }

        public BusidexUser GetUserByUserName(string userName)
        {
            using (var ctx = new busidexEntities())
            {
                return ctx.Database.SqlQuery<BusidexUser>("exec dbo.usp_GetBusidexUserByUserName @UserName={0}", userName).SingleOrDefault();
            }
        }

        public BusidexUser GetUserByDisplayName(string userName)
        {
            using (var ctx = new busidexEntities())
            {
                return ctx.Database.SqlQuery<BusidexUser>("exec dbo.usp_GetBusidexUserByDisplayName @UserName={0}", userName).SingleOrDefault();
            }
        }

        public BusidexUser GetUserByEmail(string email)
        {
            using (var ctx = new busidexEntities())
            {
                return ctx.Database.SqlQuery<BusidexUser>("exec dbo.usp_GetUserByEmail @email={0}", email).SingleOrDefault();
            }
        }

        public void SaveUserAuth(long userId, Guid token, string scope, DateTime from, DateTime to)
        {
            using (var ctx = new busidexEntities())
            {
                ctx.Database.ExecuteSqlCommand(
                    "exec dbo.usp_AddUserAuth @UserId={0}, @Token={1}, @Scope={2}, @ValidFrom={3}, @ValidTo={4}",
                    userId, token, scope, from, to);
            }
        }

        public bool SaveCardOwner(long cardId, long ownerId)
        {
	        bool ok;
            
	        using (var ctx = new busidexEntities())
	        {
                using(var trns = ctx.Database.BeginTransaction()) {
                    try {
                        var c = ctx.Cards.Find(cardId);
                        if (c != null) {
                            c.Deleted = false;
                            c.Searchable = true; // Now that the card is owned, it is searchable
                            c.OwnerId = ownerId;
                            ctx.SaveChanges();
                        }

                        ok = true;
                    } catch(Exception ex) {                            
                        trns.Rollback();
                        ok = false;
                        SaveApplicationError(ex.Message, ex.InnerException?.Message, ex.StackTrace, ownerId);
                    }
                }
	        }
 
            return ok;
        }

        public UserCard GetUserCard(long cardId, long userId)
        {
            using (var ctx = new busidexEntities())
            {
                return ctx.Database.SqlQuery<UserCard>(
                    "exec dbo.usp_GetUserCard @cardId={0}, @userId={1}", cardId, userId).Select(
                        uc => new UserCard
                        {
                            CardId = uc.CardId,
                            Created = uc.Created,
                            Deleted = uc.Deleted,
                            Notes = uc.Notes,
                            OwnerId = uc.OwnerId,
                            UserId = uc.UserId,
                            UserCardId = uc.UserCardId,
                            SharedById = uc.SharedById
                        }).FirstOrDefault();
            }
        }

        public List<UserCard> GetMyBusidex(long userId, bool includeImages)
        {

            // This may include deleted cards. If someone deletes their card, that should not
            // necessarily remove that card from everyone's busidex collection.
            using (var ctx = new busidexEntities())
            {
                var cardResults = ctx.Database.SqlQuery<usp_getMyBusidexResult>("exec dbo.usp_getMyBusidex @UserId={0},@includeImages={1}",
                    userId, includeImages).ToList();
                var aCardIds = cardResults.Select(c => c.CardId).ToArray();
                var cardIds = string.Join(",", aCardIds);

                if (string.IsNullOrEmpty(cardIds)) cardIds = "-1";

                var phoneNumbersDto = GetCardPhoneNumbers(cardIds).ToList();
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

                var addresses = GetCardAddressesByIds(cardIds).Select(address => new DTO.CardAddress
                {
                    CardAddressId = address.CardAddressId,
                    CardId = address.CardId,
                    Address1 = address.Address1,
                    Address2 = address.Address2,
                    City = address.City,
                    State = new StateCode
                    {
                        StateCodeId = 0,
                        Code = address.State
                    },
                    ZipCode = address.ZipCode,
                    Region = address.Region,
                    Country = address.Country,
                    Deleted = address.Deleted
                }).ToList();

                var links = ctx.Database.SqlQuery<DTO.ExternalLink>($"SELECT * FROM [dbo].[ExternalLink] WHERE CardId in ({cardIds})").ToList()
                 ?? new List<DTO.ExternalLink>();

               var cards = cardResults.Select(item => new UserCard
                {
                    UserCardId = item.UserCardId,
                    Notes = System.Web.HttpUtility.UrlDecode(item.Notes),
                    CardId = item.CardId,
                    RelatedCards = new List<CardRelation>(), 
                    MobileView = item.MobileView,
                    SharedById = item.SharedById,
                    Card = new DTO.Card
                    {
                        Name = item.Name,
                        CompanyName = item.CompanyName,
                        BackFileId = item.BackFileId,
                        BackOrientation = item.BackOrientation,
                        BackType = item.BackType,
                        CardId = item.CardId,
                        CreatedBy = item.CreatedBy,
                        Email = item.Email,
                        ExistsInMyBusidex = true,
                        FrontFileId = item.FrontFileId,
                        FrontOrientation = item.FrontOrientation,
                        FrontType = item.FrontType,
                        OwnerId = item.OwnerId,
                        Searchable = item.Searchable,
                        Title = item.Title,
                        Url = item.Url,
                        PhoneNumbers =  phoneNumbers.Where(p=>p.CardId == item.CardId).ToList(),
                        Addresses = addresses.Where(a=>a.CardId == item.CardId).ToList(),
                        ExternalLinks = links.Where(link => link.CardId == item.CardId).ToList()
                    }
                }).ToList();

               var tags = GetCardTagsByIds(string.Join(",", cards.Select(c => c.CardId))).ToList();
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
        }

        public void AddUserCard(UserCard userCard)
        {
            using (var ctx = new busidexEntities())
            {
                ctx.Database.ExecuteSqlCommand(
                    "exec dbo.usp_AddUserCard @CardId={0}, @UserId={1}, @OwnerId={2}, @SharedById={3}, @Notes={4}",
                    userCard.CardId, userCard.UserId, userCard.OwnerId, userCard.SharedById, userCard.Notes);
            }
        }

        public void AcceptSharedCard(long cardId, long userId)
        {
            var sharedCards = GetSharedCards(userId);
            var sharedCard = sharedCards.FirstOrDefault(c => c.CardId == cardId);
            var theirBusidex = GetMyBusidex(userId, false);
            //var card = GetCardById(cardId);
            if (sharedCard != null)
            {
                if (theirBusidex.All(c => c.CardId != sharedCard.CardId))
                {
                    AddUserCard(new UserCard
                    {
                        CardId = cardId,
                        UserId = userId,
                        OwnerId = sharedCard.Card.OwnerId,
                        SharedById = sharedCard.SendFrom,
                        Notes = sharedCard.Recommendation
                    });
                }
                using (var ctx = new busidexEntities())
                {
                    ctx.Database.ExecuteSqlCommand("exec dbo.usp_AcceptSharedCard @UserId={0}, @CardId={1}", userId, cardId);
                    ctx.Database.ExecuteSqlCommand("exec dbo.usp_updateSharedById @CardId={0}, @UserId={1}", cardId, sharedCard.SendFrom);
                }
            }
        }

        public void DeclineSharedCard(long cardId, long userId)
        {
            var sharedCards = GetSharedCards(userId).ToList();
            var sharedCard = sharedCards.FirstOrDefault(c => c.CardId == cardId);
            if (sharedCard != null)
            {
                using (var ctx = new busidexEntities())
                {
                    ctx.Database.ExecuteSqlCommand("exec dbo.usp_DeclineSharedCard @UserId={0}, @CardId={1}", userId, cardId);
                }
            }
        }

        public List<SharedCard> GetSharedCards(long userId)
        {

            using (var ctx = new busidexEntities())
            {
                var sharedCards = ctx.Database.SqlQuery<SharedCard>(
                        "exec dbo.usp_GetSharedCardsByUserId @userId={0}", userId).ToList();

                foreach (var sharedCard in sharedCards)
                {
                    var sendFrom = GetUserAccountByUserId(sharedCard.SendFrom);
                    if (sendFrom == null)
                    {
                        sendFrom = GetUserByEmail(sharedCard.SendFromEmail).UserAccount;
                    }
                    sharedCard.Card = GetCardById(sharedCard.CardId, userId);
                    if (sendFrom != null && sharedCard.Card != null)
                    {
                        sharedCard.SendFromDisplayName = !string.IsNullOrEmpty(sendFrom.DisplayName)
                            ? sendFrom.DisplayName
                            : sharedCard.Card.Name;
                        sharedCard.Card.FrontImage = sharedCard.Card.BackImage = null;
                    }
                }
                return sharedCards.Where(c => c.Card != null && c.Card.CardType == CardType.Professional).ToList();
            }            
        }

        public List<SharedCard> GetOrganizationInvitations(long userId)
        {

            using (var ctx = new busidexEntities())
            {
                var sharedCards = ctx.Database.SqlQuery<SharedCard>(
                        "exec dbo.usp_GetSharedCardsByUserId @userId={0}", userId).ToList();

                foreach (var sharedCard in sharedCards)
                {
                    sharedCard.Card = GetCardById(sharedCard.CardId, userId);
                    sharedCard.Card.FrontImage = sharedCard.Card.BackImage = null;
                }
                return sharedCards.Where(c=>c.Card.CardType == CardType.Organization).ToList();
            }
        }

        public void SaveSharedCard(SharedCard sharedCard)
        {
            using (var ctx = new busidexEntities())
            {
                ctx.Database.ExecuteSqlCommand(
                    "exec dbo.usp_AddSharedCard @CardId={0}, @SendFrom={1}, @Email={2}, @ShareWith={3}, @SharedDate={4}, @Accepted={5}, @Declined={6}, @Recommendation={7}",
                    sharedCard.CardId, sharedCard.SendFrom, sharedCard.Email, sharedCard.ShareWith, sharedCard.SharedDate, sharedCard.Accepted, sharedCard.Declined, sharedCard.Recommendation);
            }
        }

        public CardDetailModel GetCardByEmail(string email)
        {
            using (var ctx = new busidexEntities())
            {
                return
                    ctx.Database.SqlQuery<CardDetailModel>(
                        "exec dbo.usp_GetCardsByEmail @email={0}, @latitude={1}, @longitude={2}, @radiusInMiles={3}",
                        email, null, null, null).FirstOrDefault();
            }
        }

        public List<CardDetailModel> GetCardsByPhoneNumber(long userId, string phoneNumber)
        {
            using (var ctx = new busidexEntities())
            {
                return
                    ctx.Database.SqlQuery<CardDetailModel>(
                        "exec dbo.usp_getCardsByPhoneNumber @userId={0}, @phone={1}, @latitude={2}, @longitude={3}, @radiusInMiles={4}",
                        userId, phoneNumber, null, null, null).ToList();
            }
        }

        public List<SharedCard> Get30DaySharedCards()
        {
            using (var ctx = new busidexEntities())
            {
                var sharedCards = ctx.Database.SqlQuery<SharedCard>("exec dbo.usp_GetUnclaimedSharedCards").ToList();
                return sharedCards.Where(c => c.SharedDate < DateTime.Today.AddDays(-30)).ToList();
            }
            
        }

        public SharedCard GetSharedCard(long cardId, long sendFrom, long shareWith)
        {
            using (var ctx = new busidexEntities())
            {
                return ctx.Database.SqlQuery<SharedCard>("exec dbo.usp_GetSharedCard @CardId={0}, @SendFrom={1}, @ShareWith={2}", cardId, sendFrom, shareWith).FirstOrDefault();
            }
        }

        public List<string> GetUsersThatHaveCard(long cardId)
        {
            using (var ctx = new busidexEntities())
            {
                return ctx.Database.SqlQuery<string>("exec dbo.usp_GetUsersThatHaveCard @CardId={0}", cardId).ToList();
            }
        }

        public void DeletePhoneNumber(long phoneNumberId)
        {
            using (var ctx = new busidexEntities())
            {
                ctx.Database.ExecuteSqlCommand("exec dbo.usp_DeletePhoneNumber @PhoneNumberId={0}", phoneNumberId);
            }
        }

        public List<PhoneNumber> GetCardPhoneNumber(long cardId)
        {
            using (var ctx = new busidexEntities())
            {
                return
                    ctx.Database.SqlQuery<PhoneNumber>("exec dbo.usp_getCardPhoneNumber @cardId={0}", cardId).ToList();
            }
        }

        public List<PhoneNumberDTO> GetCardPhoneNumbers(string cardIds)
        {
            using (var ctx = new busidexEntities())
            {
                return
                    ctx.Database.SqlQuery<PhoneNumberDTO>("exec dbo.usp_getCardPhoneNumbers @cardIds={0}", cardIds)
                        .ToList();
            }
        }

        public List<Tag> GetCardTags(long cardId)
        {
            using (var ctx = new busidexEntities())
            {
                //var b = new EntityConnectionStringBuilder();
                //b.Metadata = "metadata=res://*/DataAccess.BusidexData.csdl|res://*/DataAccess.BusidexData.ssdl|res://*/DataAccess.BusidexData.msl";
                //b.ProviderConnectionString = _connectionString;
                //b.Provider = "System.Data.SqlClient";
                //ctx.Database.Connection.ConnectionString = b.ConnectionString;
                return ctx.Database.SqlQuery<Tag>("exec dbo.usp_GetCardTags @cardId={0}", cardId).ToList();
            }
        }

        public List<CardTag> GetCardTagsByIds(string cardIds)
        {
            using (var ctx = new busidexEntities())
            {
                return ctx.Database.SqlQuery<CardTag>("exec dbo.usp_GetCardTagsByIds @cardIds={0}", cardIds).ToList();
            }
        }

        public List<DTO.CardAddress> GetCardAddresses(long cardId)
        {
            using (var ctx = new busidexEntities())
            {
                return ctx.Database.SqlQuery<DTO.CardAddress>("exec dbo.usp_GetCardAddresses @cardId={0}", cardId).ToList();
            }
        }

        public List<DTO.ExternalLink> GetExternalLinks(long cardId)
        {
            using (var ctx = new busidexEntities())
            {
                var links = ctx.Database.SqlQuery<DTO.ExternalLink>("exec dbo.usp_GetExternalLinksByCardId @cardId={0}", cardId).ToList();
                foreach(var link in links)
                {
                    var linkType = ctx.Database.SqlQuery<DTO.ExternalLinkType>($"SELECT * from dbo.ExternalLinkType where ExternalLinkTypeId = {link.ExternalLinkTypeId}").FirstOrDefault();
                    link.ExternalLinkType = linkType;
                }
                return links;
            }
        }

        public List<CardAddressesDTO> GetCardAddressesByIds(string cardIds)
        {
            using (var ctx = new busidexEntities())
            {
                return ctx.Database.SqlQuery<CardAddressesDTO>("exec dbo.usp_GetCardAddressesByIds @CardIds={0}", cardIds).ToList();
            }
        }

        public List<StateCode> GetAllStateCodes()
        {
            using (var ctx = new busidexEntities())
            {
                return ctx.Database.SqlQuery<StateCode>("exec dbo.usp_GetAllStateCodes").ToList();
            }
        }

        public List<PhoneNumberType> GetAllPhoneNumberTypes()
        {
            using (var ctx = new busidexEntities())
            {
                return ctx.Database.SqlQuery<PhoneNumberType>("exec dbo.usp_GetAllPhoneNumberTypes").ToList();
            }
        }

        public List<long> GetRecentlyUpdatedCards()
        {

            using (var ctx = new busidexEntities())
            {
                return ctx.Database.SqlQuery<long>("exec dbo.usp_GetRecentlyUpdatedCards").ToList();
            }
            
        }

        private DTO.Card _populateCard(DTO.Card c)
        {
            var card = c;
            var cardPhoneNumbers = GetCardPhoneNumber(card.CardId);
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
                TagType = (TagType) t.TagTypeId
            }).ToList();

            var addresses = GetCardAddresses(card.CardId);
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
                State = stateCodes.SingleOrDefault(sc => sc.StateCodeId == a.StateCodeId),
                ZipCode = a.ZipCode
            }).ToList();

            var links = GetExternalLinks(card.CardId);

            card.PhoneNumbers = phoneNumbers;
            card.Tags = cardTags;
            card.Addresses = cardAddresses;
            card.ExternalLinks = links;

            return card;
        }

        public DTO.Card GetCardById(long id, long userId = 0)
        {
            using (var ctx = new busidexEntities())
            {
                var card = ctx.Database.SqlQuery<DTO.Card>("exec dbo.usp_getCardById @cardId={0}, @userId={1}", id, userId).SingleOrDefault();

                if (card != null)
                {
                    return _populateCard(card);
                }
                return null;
            }
        }

        public DTO.Card GetCardByToken(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return null;
            }

            var ownerToken = new Guid(token);
            using (var ctx = new busidexEntities())
            {
                var card = ctx.Database.SqlQuery<DTO.Card>("exec dbo.usp_GetCardByOwnerToken @Token={0}", ownerToken).SingleOrDefault();

                if (card != null)
                {
                    return _populateCard(card);
                }
                return null;
            }
        }

        public List<BusidexUser> GetCardOwners()
        {
            using (var ctx = new busidexEntities())
            {
                return ctx.Database.SqlQuery<BusidexUser>("exec dbo.usp_GetCardOwners").ToList();
            }
        }

        public Organization GetOrganizationById(long id)
        {
            using (var ctx = new busidexEntities())
            {
                return
                    ctx.Database.SqlQuery<Organization>("exec dbo.usp_GetOrganizationById @Id={0}", id)
                        .SingleOrDefault();
            }
        }

        public List<Organization> GetOrganizationsByUserId(long userId)
        {
            using (var ctx = new busidexEntities())
            {
                return
                    ctx.Database.SqlQuery<Organization>(
                        "exec dbo.usp_GetOrganizationsByUserId @userId={0}", userId)
                        .ToList();
            }
        }

        public void UpdateOrganization(Organization organization, long userId)
        {
            using (var ctx = new busidexEntities())
            {
                ctx.Database.ExecuteSqlCommand(
                    "exec dbo.usp_UpdateOrganization @OrganizationId={0}, @UserId={1}, @Name={2}, @Contacts={3}, @Description={4}, @HomePage={5}, @Email={6}, @Url={7}, @Phone1={8}, @Phone2={9}, @Twitter={10}, @Facebook={11}",
                    organization.OrganizationId, userId, organization.Name, organization.Contacts,
                    organization.Description, organization.HomePage, organization.Email, organization.Url, organization.Phone1,
                    organization.Phone2, organization.Twitter, organization.Facebook);
            }
        }

        public void UpdateOrganizationLogoId(long organizationId, Guid fileId, string fileType)
        {
            using (var ctx = new busidexEntities())
            {
                ctx.Database.ExecuteSqlCommand(
                    "exec dbo.usp_UpdateOrganizationLogoId @OrganizationId={0}, @FileId={1}, @FileType={2}",
                    organizationId, fileId, fileType);
            }
        }

        public void AddOrganization(Organization organization, long userId)
        {

            using (var ctx = new busidexEntities())
            {
                ctx.Database.ExecuteSqlCommand(
                    "exec dbo.usp_AddOrganization @UserId={0}, @Name={1}, @Contacts={2}, @Description={3}, @HomePage={4}, @EMail={6}, @Url={6}, @Phone1={7}, @Phone2={8}, @Twitter={9}, @Facebook={10}",
                    userId, organization.Name, organization.Contacts, organization.Description,
                    organization.HomePage, organization.Email, organization.Url, organization.Phone1, organization.Phone2,
                    organization.Twitter, organization.Facebook);
            }
        }

        public void AddOrganizationCard(long organizationId, long cardId, long userId)
        {
            using (var ctx = new busidexEntities())
            {
                ctx.Database.ExecuteSqlCommand(
                    "exec dbo.usp_AddOrganizationCard @OrganizationId={0}, @CardId={1}, @UserId={2}", organizationId,
                    cardId, userId);
            }
        }

        public void DeleteOrganizationCard(long organizationId, long cardId, long userId)
        {
            using (var ctx = new busidexEntities())
            {
                ctx.Database.ExecuteSqlCommand(
                    "exec dbo.usp_DeleteOrganizationCard @OrganizationId={0}, @CardId={1}, @UserId={2}", organizationId,
                    cardId, userId);
            }
        }

        public List<OrganizationGuest> GetOrganizationGuests(long organizationId, long userId)
        {
            using (var ctx = new busidexEntities())
            {
                var items =
                    ctx.Database.SqlQuery<OrganizationGuest>("exec dbo.usp_GetOrganizationGuests @organizationId={0}, @userId={1}",
                        organizationId, userId).ToList();
                return items;
            }
        } 

        public List<CardDetailModel> GetOrganizationCards(long organizationId, long userId, bool includeImages)
        {
            using (var ctx = new busidexEntities())
            {
                var items =
                    ctx.Database.SqlQuery<DTO.Card>("exec dbo.usp_GetOrganizationCards @organizationId={0}, @userId={1}",
                        organizationId, userId).ToList();
                var cardIds = string.Join(",", items.Select(c => c.CardId).ToArray());

                var phoneNumberData =
                    ctx.Database.SqlQuery<PhoneNumber>("exec dbo.usp_GetCardPhoneNumbers @cardIds={0}", cardIds)
                        .ToList();
                var phoneNumberTypes =
                    ctx.Database.SqlQuery<PhoneNumberType>("exec dbo.usp_GetAllPhoneNumberTypes")
                        .ToDictionary(t => t.PhoneNumberTypeId, t => t.Name);
                var tagData =
                    ctx.Database.SqlQuery<CardTag>("exec dbo.usp_GetCardTagsByIds @CardIds={0}", cardIds).ToList();
                var stateCodes = ctx.Database.SqlQuery<StateCode>("exec dbo.usp_GetAllStateCodes").ToList();
                var addressData =
                    ctx.Database.SqlQuery<DTO.CardAddress>("exec dbo.usp_GetCardAddressesByIds @CardIds={0}", cardIds)
                        .ToList();
                var cardAddresses = addressData.Select(a => new DTO.CardAddress
                {
                    Address1 = a.Address1,
                    Address2 = a.Address2,
                    CardAddressId = a.CardAddressId,
                    CardId = a.CardId,
                    City = a.City,
                    Country = a.Country,
                    Deleted = a.Deleted,
                    Region = a.Region,
                    State = stateCodes.SingleOrDefault(sc => sc.StateCodeId == a.StateCodeId),
                    ZipCode = a.ZipCode
                }).ToList();

                var phoneNumbers = phoneNumberData.Select(p => new PhoneNumber
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

                var cards = items.Select(item =>
                    new CardDetailModel
                    {
                        BackFileId = item.BackFileId,
                        BackOrientation = item.BackOrientation,
                        BackType = item.BackType,
                        BackFileType = item.BackType,
                        CardId = item.CardId,
                        FrontFileId = item.FrontFileId,
                        FrontType = item.FrontType,
                        FrontFileType = item.FrontType,
                        FrontOrientation = item.FrontOrientation,
                        Email = item.Email,
                        Url = item.Url,
                        Title = item.Title,
                        Name = item.Name,
                        OwnerId = item.OwnerId,
                        CreatedBy = item.CreatedBy.GetValueOrDefault(),
                        CompanyName = item.CompanyName,
                        Tags =
                            tagData.Where(t => t.CardId == item.CardId)
                                .Select(t => new Tag {TagId = t.TagId, Text = t.Text, TagType = (TagType) t.TagTypeId})
                                .ToList(),
                        PhoneNumbers = phoneNumbers.Where(p => p.CardId == item.CardId).ToList(),
                        Addresses = cardAddresses.Where(a => a.CardId == item.CardId).ToList(),
                        //FrontImage = includeImages ? item.FrontImage : null,
                        //BackImage = includeImages ? item.BackImage : null,
                        IsMyCard = item.OwnerId == userId,
                        ExistsInMyBusidex = item.ExistsInMyBusidex,
                        SEO_Name = item.SEO_Name
                    }
                    ).ToList();

                return cards;
            }
        }

        public List<Group> GetOrganizationGroups(long ownerId, long userId)
        {
            using (var ctx = new busidexEntities())
            {
                const int GROUP_TYPE_ORGANIZATION = 2;
                return
                    ctx.Database.SqlQuery<Group>(
                        "exec dbo.usp_GetMyBusigroups @ownerId={0}, @userId={1}, @groupTypeId={2}", ownerId, userId,
                        GROUP_TYPE_ORGANIZATION)
                        .ToList();
            }
        }

        public void AddGroupCards(long groupId, string cardIds)
        {
            using (var ctx = new busidexEntities())
            {
                ctx.Database.ExecuteSqlCommand(
                    "exec dbo.usp_AddGroupCards @GroupId={0}, @CardIds={1}",
                    groupId, cardIds);
            }
        }

        public List<Group> GetGroupsByOwnerId(long ownerId, int groupTypeId)
        {
            using (var ctx = new busidexEntities())
            {
                return ctx.Database.SqlQuery<Group>(
                    "exec dbo.usp_GetGroupsByOwnerId @OwnerId={0}, @GroupTypeId={1}",
                    ownerId, groupTypeId).ToList();
            }
        }

        public Group GetGroupById(long groupId)
        {
            using (var ctx = new busidexEntities())
            {
                return ctx.Database.SqlQuery<Group>(
                    "exec dbo.usp_GetGroupById @GroupId={0}",
                    groupId).SingleOrDefault();
            }
        }

        public Group GetGroupByName(string groupName)
        {
            using (var ctx = new busidexEntities())
            {
                return ctx.Database.SqlQuery<Group>(
                    "exec dbo.usp_GetGroupByName @groupName={0}",
                    groupName).SingleOrDefault();
            }
        }

        public void DeleteGroup(long groupId)
        {
            using (var ctx = new busidexEntities())
            {
                ctx.Database.ExecuteSqlCommand(
                    "exec dbo.usp_DeleteGroup @GroupId={0}",
                    groupId);
            }

        }

        public List<GroupCard> GetGroupCards(long groupId)
        {
            using (var ctx = new busidexEntities())
            {
                var results = ctx.Database.SqlQuery<GroupCardResult>(
                    "exec dbo.usp_GetGroupCards @GroupId={0}",
                    groupId).ToList();

                var groupCards = results.Select(r => new GroupCard
                {
                    GroupCardId = r.GroupCardId,
                    Deleted = r.Deleted,
                    CardId = r.CardId,
                    Card = new DTO.Card
                    {
                        CardId = r.CardId.GetValueOrDefault(),
                        Name = r.Name,
                        Title = r.Title,
                        FrontType = r.FrontType,
                        FrontOrientation = r.FrontOrientation,
                        BackType = r.BackType,
                        BackOrientation = r.BackOrientation,
                        Searchable = r.Searchable,
                        CompanyName = r.CompanyName,
                        Email = r.Email,
                        Url = r.Url,
                        CreatedBy = r.CreatedBy,
                        OwnerId = r.OwnerId,
                        Deleted = r.Deleted,
                        FrontFileId = r.FrontFileId,
                        BackFileId = r.BackFileId
                    }
                }).ToList();

                var cardIds = string.Join(",", results.Select(c => c.CardId).ToArray());
                var phoneNumbersDto = GetCardPhoneNumbers(cardIds);
                var phoneNumberTypes = GetAllPhoneNumberTypes();

                var phoneNumbers = phoneNumbersDto.Select(phoneNumber => new PhoneNumber
                {
                    CardId = phoneNumber.CardId,
                    Number = phoneNumber.Number,
                    Extension = phoneNumber.Extension,
                    PhoneNumberType = phoneNumberTypes.Select(t => new PhoneNumberType
                    {
                        Name = t.Name,
                        PhoneNumberTypeId = t.PhoneNumberTypeId
                    }).SingleOrDefault(t => t.PhoneNumberTypeId == phoneNumber.PhoneNumberTypeId)
                }).ToList();


                var cards = groupCards.Select(item => new GroupCard
                {
                    GroupCardId = item.GroupCardId,
                    GroupId = item.GroupId,
                    Notes = item.Notes,
                    CardId = item.CardId,
                    Card = new DTO.Card
                    {
                        BackFileId = item.Card.BackFileId,
                        BackOrientation = item.Card.BackOrientation,
                        BackType = item.Card.BackType,
                        CardId = item.CardId.GetValueOrDefault(),
                        FrontFileId = item.Card.FrontFileId,
                        FrontType = item.Card.FrontType,
                        FrontOrientation = item.Card.FrontOrientation,
                        Email = item.Card.Email,
                        Url = item.Card.Url,
                        Title = item.Card.Title,
                        Name = item.Card.Name,
                        OwnerId = item.Card.OwnerId,
                        CreatedBy = item.Card.CreatedBy,
                        CompanyName = item.Card.CompanyName,
                        PhoneNumbers = phoneNumbers.Where(p => p.CardId == item.CardId).ToList(),
                        FrontImage = null,
                        BackImage = null,
                    }
                }).ToList();


                var tags = GetCardTagsByIds(string.Join(",", cards.Select(c => c.CardId))).ToList();
                foreach (var userCard in cards)
                {
                    userCard.Card.Tags = (from tag in tags
                        where tag.CardId == userCard.CardId
                        select new Tag
                        {
                            TagId = tag.TagId,
                            Text = tag.Text,
                            TagType = (TagType) tag.TagTypeId
                        }).ToList();
                }
                return cards.ToList();
            }
        }

        public void RemoveGroupCards(long groupId, string cardIds, long userId)
        {
            using (var ctx = new busidexEntities())
            {
                ctx.Database.ExecuteSqlCommand(
                    "exec dbo.usp_RemoveGroupCards @GroupId={0}, @CardIds={1}, @UserId={2}",
                    groupId, cardIds, userId);
            }

        }

        public void UpdateCardLinks(long cardId, List<ExternalLink> links)
        {
            using (var ctx = new busidexEntities())
            {
                using(var trns = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        var card = ctx.Cards.Find(cardId);
                        if(card != null)
                        {
                            ctx.ExternalLinks.RemoveRange(card.ExternalLinks);
                            ctx.ExternalLinks.AddRange(links);
                            ctx.SaveChanges();
                            trns.Commit();
                        }
                        
                    }catch(Exception ex)
                    {
                        trns.Rollback();
                        SaveApplicationError(ex.Message, ex.InnerException?.ToString(), ex.StackTrace, 0);
                    }
                }
            }
        }

        public void AddGroup(Group group, string cardIds)
        {
            using (var ctx = new busidexEntities())
            {
                BusidexDatabaseConfiguration.SuspendExecutionStrategy = true;

                using (var t = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        long groupId = ctx.Database.SqlQuery<long>(
                            "exec dbo.usp_AddGroup @OwnerId={0}, @GroupTypeId={1}, @Description={2}, @Notes={3}",
                            group.OwnerId, group.GroupTypeId, group.Description, group.Notes).SingleOrDefault();
                        if (groupId > 0)
                        {
                            ctx.Database.ExecuteSqlCommand(
                                "exec dbo.usp_AddGroupCards @GroupId={0}, @CardIds={1}",
                                groupId, cardIds);

                            t.Commit();
                            ctx.SaveChanges();
                        }
                    }
                    catch (Exception ex)
                    {
                        t.Rollback();
                        SaveApplicationError(ex.Message, ex.InnerException?.ToString(), ex.StackTrace, 0);
                    }
                }
                BusidexDatabaseConfiguration.SuspendExecutionStrategy = false;
            }
        }


        public List<UserCard> GetOrganizationReferrals(long userId, long organizationId)
        {
            using (var ctx = new busidexEntities())
            {
                var results = ctx.Database.SqlQuery<UserCardResult>(
                    "exec dbo.usp_GetOrganizationReferrals @UserId={0}, @OrganizationId={1}",
                    userId, organizationId).ToList();

                var cardIds = string.Join(",", results.Select(c => c.CardId).ToArray());
                var phoneNumbersDto = GetCardPhoneNumbers(cardIds);
                var phoneNumberTypes = GetAllPhoneNumberTypes();

                var phoneNumbers = phoneNumbersDto.Select(phoneNumber => new PhoneNumber
                {
                    CardId = phoneNumber.CardId,
                    Number = phoneNumber.Number,
                    Extension = phoneNumber.Extension,
                    PhoneNumberType = phoneNumberTypes.Select(t => new PhoneNumberType
                    {
                        Name = t.Name,
                        PhoneNumberTypeId = t.PhoneNumberTypeId
                    }).SingleOrDefault(t => t.PhoneNumberTypeId == phoneNumber.PhoneNumberTypeId)
                }).ToList();

                var referralCards = results.Select(r => new UserCard
                {
                    UserCardId = r.UserCardId,
                    Deleted = r.Deleted,
                    CardId = r.CardId,
                    Notes = r.Notes,
                    Card = new DTO.Card
                    {
                        CardId = r.CardId,
                        Name = r.Name,
                        Title = r.Title,
                        FrontType = r.FrontType,
                        FrontOrientation = r.FrontOrientation,
                        BackType = r.BackType,
                        BackOrientation = r.BackOrientation,
                        Searchable = r.Searchable,
                        CompanyName = r.CompanyName,
                        Email = r.Email,
                        Url = r.Url,
                        CreatedBy = r.CreatedBy,
                        OwnerId = r.OwnerId,
                        Deleted = r.Deleted,
                        FrontFileId = r.FrontFileId,
                        BackFileId = r.BackFileId,
                        PhoneNumbers = phoneNumbers.Where(p => p.CardId == r.CardId).ToList(),
                        IsMyCard = r.OwnerId.GetValueOrDefault() == userId
                    }
                }).ToList();

                var tags = GetCardTagsByIds(string.Join(",", referralCards.Select(c => c.CardId))).ToList();
                foreach (var userCard in referralCards)
                {
                    userCard.Card.Tags = (from tag in tags
                        where tag.CardId == userCard.CardId
                        select new Tag
                        {
                            TagId = tag.TagId,
                            Text = tag.Text,
                            TagType = (TagType) tag.TagTypeId
                        }).ToList();
                }
                return referralCards.ToList();
            }
        }

        List<DTO.Card> RefineSearch(List<DTO.Card> list, string[] criteria)
        {
            if (criteria.Length == 0)
            {
                return list;
            }

            foreach (string t in criteria)
            {
                string item = t.ToLowerInvariant();
                list.RemoveAll(
                    c =>
                        !(c.Name ?? "").ToLowerInvariant().Contains(item) &&
                        !(c.CompanyName ?? "").ToLowerInvariant().Contains(item) &&
                        !(c.Title ?? "").ToLowerInvariant().Contains(item) &&
                        !(c.Email ?? "").ToLowerInvariant().Contains(item) &&
                        !(c.TagList ?? "").ToLowerInvariant().Contains(item));
            }

            return list;
        }

        public void SaveApplicationError(string error, string innerException, string stackTrace, long userId)
        {
            using (var ctx = new busidexEntities())
            {
                ctx.Database.ExecuteSqlCommand(
                    "exec dbo.usp_AddApplicationError @message={0}, @innerException={1}, @stackTrace={2}, @userId={3}",
                    error, innerException, stackTrace, userId);
            }
        }

        public IEnumerable<DTO.Card> SearchCards(string criteria, double? latitude, double? longitude, int? distance, bool searchableOnly, CardType cardType, long? userId = 0)
        {
            using (var ctx = new busidexEntities())
            {
                var results = ctx.Database.SqlQuery<DTO.Card>(
                    "exec dbo.usp_SearchCards @criteria={0}, @latitude={1}, @longitude={2}, @radiusInMiles={3}, @SearchableOnly = {4}, @CardType={5}, @UserId={6}",
                    criteria, latitude, longitude, distance, searchableOnly, cardType, userId).ToList();


                //SaveApplicationError(criteria + ", " + latitude.GetValueOrDefault().ToString() + ", " +
                //                     longitude.GetValueOrDefault().ToString() +
                //                     ", " + distance.GetValueOrDefault().ToString() + ", " + searchableOnly.ToString() +
                //                     cardType.ToString() + userId.GetValueOrDefault().ToString(), "", "", userId.GetValueOrDefault());

                //var allCards = _SearchCards(criteria, latitude, longitude, distance, searchableOnly, (int) cardType);
                var cards = results.Select(item => new DTO.Card
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
                    CompanyName = item.CompanyName,
                    Markup = item.Markup,
                    Visibility = item.Visibility,
                    ExistsInMyBusidex = item.ExistsInMyBusidex,
                    SEO_Name = item.SEO_Name
                }).ToList();

                var tags = GetCardTagsByIds(string.Join(",", cards.Select(c => c.CardId))).ToList();
                foreach (var userCard in cards)
                {
                    userCard.Tags = (from tag in tags
                                          where tag.CardId == userCard.CardId
                                          select new Tag
                                          {
                                              TagId = tag.TagId,
                                              Text = tag.Text,
                                              TagType = (TagType)tag.TagTypeId
                                          }).ToList();
                }

                // only return the cards that contain ALL of the search criteria
                cards = RefineSearch(cards, criteria.Split(','));
                for (var i=0; i < cards.Count; i++)
                {
                    cards[i] = _populateCard(cards[i]);
                }
                return cards;
            }
        }


        public List<DTO.Card> SearchBySystemTag(string systag, long? userId)
        {
            const bool INCLUDE_UNOWNED_CARDS = true;
            using (var ctx = new busidexEntities())
            {
                var results = ctx.Database.SqlQuery<DTO.Card>(
                    "exec dbo.usp_GetCardsByTag @tag={0}, @latitude={1}, @longitude={2}, @radiusInMiles={3}, @includeUnOwned = {4}, @userId={5}",
                    systag, null, null, null, INCLUDE_UNOWNED_CARDS, userId).ToList();

                var cardIds = string.Join(",", results.Select(c => c.CardId).ToArray());

				var phoneNumbersDto = GetCardPhoneNumbers(cardIds);
                var phoneNumberTypes = GetAllPhoneNumberTypes();

                var phoneNumbers = phoneNumbersDto.Select(phoneNumber => new PhoneNumber
                {
                    CardId = phoneNumber.CardId,
                    Number = phoneNumber.Number,
                    Extension = phoneNumber.Extension,
                    PhoneNumberType = phoneNumberTypes.Select(t => new PhoneNumberType
                    {
                        Name = t.Name,
                        PhoneNumberTypeId = t.PhoneNumberTypeId
                    }).SingleOrDefault(t => t.PhoneNumberTypeId == phoneNumber.PhoneNumberTypeId)
                }).ToList();

                var cards = results.Select(r => new DTO.Card
                {

                    CardId = r.CardId,
                    Name = r.Name,
                    Title = r.Title,
                    FrontType = r.FrontType,
                    FrontOrientation = r.FrontOrientation,
                    BackType = r.BackType,
                    BackOrientation = r.BackOrientation,
                    Searchable = r.Searchable,
                    CompanyName = r.CompanyName,
                    Email = r.Email,
                    Url = r.Url,
                    CreatedBy = r.CreatedBy,
                    OwnerId = r.OwnerId,
                    Deleted = r.Deleted,
                    FrontFileId = r.FrontFileId,
                    BackFileId = r.BackFileId,
                    Addresses = GetCardAddresses(r.CardId),
                    PhoneNumbers = phoneNumbers.Where(p=>p.CardId == r.CardId).ToList(),
                    ExistsInMyBusidex = r.ExistsInMyBusidex,
                    SEO_Name = r.SEO_Name

                }).ToList();

                
                return cards.ToList();
            }
        }

        public List<DTO.Card> SearchByGroupName(string groupName, long? userId)
        {
            const bool INCLUDE_UNOWNED_CARDS = true;
            using (var ctx = new busidexEntities())
            {
                var results = ctx.Database.SqlQuery<DTO.Card>(
                    "exec dbo.usp_getCardsByGroupName @GroupName={0}, @latitude={1}, @longitude={2}, @radiusInMiles={3}, @includeUnOwned = {4}, @userId={5}",
                    groupName, null, null, null, INCLUDE_UNOWNED_CARDS, userId).ToList();

                var cardIds = string.Join(",", results.Select(c => c.CardId).ToArray());

                var phoneNumbersDto = GetCardPhoneNumbers(cardIds);
                var phoneNumberTypes = GetAllPhoneNumberTypes();

                var phoneNumbers = phoneNumbersDto.Select(phoneNumber => new PhoneNumber
                {
                    CardId = phoneNumber.CardId,
                    Number = phoneNumber.Number,
                    Extension = phoneNumber.Extension,
                    PhoneNumberType = phoneNumberTypes.Select(t => new PhoneNumberType
                    {
                        Name = t.Name,
                        PhoneNumberTypeId = t.PhoneNumberTypeId
                    }).SingleOrDefault(t => t.PhoneNumberTypeId == phoneNumber.PhoneNumberTypeId)
                }).ToList();

                var cards = results.Select(r => new DTO.Card
                {

                    CardId = r.CardId,
                    Name = r.Name,
                    Title = r.Title,
                    FrontType = r.FrontType,
                    FrontOrientation = r.FrontOrientation,
                    BackType = r.BackType,
                    BackOrientation = r.BackOrientation,
                    Searchable = r.Searchable,
                    CompanyName = r.CompanyName,
                    Email = r.Email,
                    Url = r.Url,
                    CreatedBy = r.CreatedBy,
                    OwnerId = r.OwnerId,
                    Deleted = r.Deleted,
                    FrontFileId = r.FrontFileId,
                    BackFileId = r.BackFileId,
                    Addresses = r.Addresses.Where(a => a.CardId == r.CardId).ToList(),
                    PhoneNumbers = phoneNumbers.Where(p => p.CardId == r.CardId).ToList(),
                    ExistsInMyBusidex = r.ExistsInMyBusidex,
                    SEO_Name = r.SEO_Name

                }).ToList();


                return cards.ToList();
            }
        }

        public List<SEOCardResult> GetSeoCardResult()
        {
            using (var ctx = new busidexEntities())
            {
                return ctx.Database.SqlQuery<SEOCardResult>(
                    "exec dbo.usp_getSEOCardNames").ToList();
            }
        }

        public UserAccount AddUserAccount(UserAccount userAccount)
        {
            if (userAccount.UserAccountId == 0)
            {
                using (var ctx = new busidexEntities())
                {
                    UserAccount newAccount = ctx.Database.SqlQuery<UserAccount>(
                        "exec dbo.usp_AddUserAccount @UserId={0}, @AccountTypeId={1}, @Notes={2}, @ActivationToken={3}, @ReferredBy={4}, @DisplayName={5}",
                        userAccount.UserId, userAccount.AccountTypeId, userAccount.Notes, userAccount.ActivationToken,
                        userAccount.ReferredBy, userAccount.DisplayName).SingleOrDefault();

                    if (newAccount != null)
                    {
                        newAccount.AccountType = ctx.Database.SqlQuery<AccountType>(
                            "exec usp_GetAccountTypeById @AccountTypeId={0}", newAccount.AccountTypeId).SingleOrDefault();
                    }
                    return newAccount;
                }
        
            }
            return null;
        }

        public void SaveUserAccountDeactivateToken(long userId, string token)
        {
            using (var ctx = new busidexEntities())
            {
                ctx.Database.ExecuteSqlCommand(
                    "exec dbo.usp_SaveUserAccountDeactivateToken @UserId={0}, @Token={1}",
                    userId, token);
            }
        }

        public UserAccount GetUserAccountByDeactivateToken(string token)
        {
            using (var ctx = new busidexEntities())
            {
                return ctx.Database.SqlQuery<UserAccount>(
                    "exec dbo.usp_GetUserAccountByDeactivateToken @Token={0}", token).SingleOrDefault();
            } 
        }

        public void DeleteTag(long cardId, long tagId)
        {
            using (var ctx = new busidexEntities())
            {
                ctx.Database.ExecuteSqlCommand(
                    "exec dbo.usp_DeleteCardTag @CardId={0}, @TagId={1}",
                    cardId, tagId);
            }
        }

        public void DeleteUserAccount(long userId)
        {
            using (var ctx = new busidexEntities())
            {
                ctx.Database.ExecuteSqlCommand(
                    "exec dbo.usp_DeleteUserAccount @UserId={0}",
                    userId);
            }
        }

        public void UpdateUserCardStatus(long userCardId, UserCardAddStatus status)
        {
            using (var ctx = new busidexEntities())
            {
                ctx.Database.ExecuteSqlCommand(
                    "exec dbo.usp_UpdateUserCardStatus @UserCardId={0}, @Status={1}",
                    userCardId, status);
            }
        }

        public bool UpdateEmail(long userId, string email)
        {
            using (var ctx = new busidexEntities())
            {
                ctx.Database.ExecuteSqlCommand(
                    "exec dbo.usp_UpdateEmail @userId={0}, @email={1}",
                    userId, email);
            }
            return true;
        }

        public void UpdateCardFileId(long cardId, Guid frontFileId, string frontType, Guid backFileId, string backType)
        {
            using (var ctx = new busidexEntities())
            {
                ctx.Database.ExecuteSqlCommand(
                    "exec dbo.usp_UpdateCardFileId @CardId={0}, @FrontFileId={1}, @FrontType={2}, @BackFileId={3}, @BackType={4}",
                    cardId, frontFileId, frontType, backFileId, backType);
            }
        }

        public void UpdateCardOrientation(long cardId, string frontOrientation, string backOrientation)
        {
            using (var ctx = new busidexEntities())
            {
                ctx.Database.ExecuteSqlCommand(
                    "exec dbo.usp_UpdateCardOrientation @CardId={0}, @FrontOrientation={1}, @BackOrientation={2}",
                    cardId, frontOrientation, backOrientation);
            }
        }

        public void SaveSmsShare(SMSShare model)
        {
            using (var ctx = new busidexEntities())
            {
                ctx.Database.ExecuteSqlCommand(
                    "exec dbo.usp_AddSMSShare @FromUserId={0}, @CardId={1}, @PhoneNmber={2}, @Message={3}",
                    model.FromUserId, model.CardId, model.PhoneNumber, model.Message);
            }
            

        }
        public void UpdateOnboardingComplete(long userId, bool complete)
        {
            using (var ctx = new busidexEntities())
            {
                ctx.Database.ExecuteSqlCommand(
                    "exec dbo.usp_UpdateOnboardingComplete @UserId={0}, @Complete={1}", userId, complete);
            }
        }

        public UserAccount GetUserAccountByPhoneNumber(string phoneNumber)
        {
            using (var ctx = new busidexEntities())
            {
                var userAccount = ctx.Database.SqlQuery<UserAccount>("exec dbo.usp_GetUserAccountByPhoneNumber @PhoneNumber={0}",
                    phoneNumber);
                if (userAccount == null)
                {
                    return null;
                }

                var accountTypes = getActivePlans();

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
                    AccountType = accountTypes.SingleOrDefault(at => at.AccountTypeId == ua.AccountTypeId),
                    BusidexUser = GetBusidexUserById(ua.UserId)
                }).SingleOrDefault();
            }
        }

        public UserAccount GetUserAccountByUserId(long userId)
        {
            using (var ctx = new busidexEntities())
            {
                var userAccount = ctx.Database.SqlQuery<UserAccount>("exec dbo.usp_GetUserAccountByUserId @UserId={0}",
                    userId);
                if (userAccount == null)
                {
                    return null;
                }

                var accountTypes = getActivePlans();

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
                    AccountType = accountTypes.SingleOrDefault(at => at.AccountTypeId == ua.AccountTypeId),
                    BusidexUser = GetBusidexUserById(userId)
                }).SingleOrDefault();
            }
        }

        public List<UserTerm> GetUserTerms(long userId)
        {
            using (var ctx = new busidexEntities())
            {
                return ctx.Database.SqlQuery<UserTerm>(
                    "exec dbo.usp_GetUserTerms @UserId={0}", userId).ToList();
            }
        }

        public void AcceptUserTerms(long userId)
        {
            using (var ctx = new busidexEntities())
            {
                ctx.Database.ExecuteSqlCommand("exec dbo.usp_AcceptUserTerms @UserId={0}", userId);
            }
        }


        public BusidexUser GetBusidexUserById(long id)
        {

            using (var ctx = new busidexEntities())
            {
                var user = ctx.Database.SqlQuery<BusidexUser>(
                    "exec dbo.usp_GetBusidexUserByUserId @UserId={0}", id).FirstOrDefault();

                var address =
                    ctx.Database.SqlQuery<UserAddress>("exec dbo.usp_GetUserAddress @UserId={0}", id).FirstOrDefault();

                var accountTypes = ctx.Database.SqlQuery<AccountType>("exec dbo.usp_GetActivePlans").ToList();

                var account =
                    ctx.Database.SqlQuery<UserAccount>("exec dbo.usp_GetUserAccountByUserId @UserId={0}", id)
                        .Select(a => new UserAccount
                        {
                            AccountTypeId = a.AccountTypeId,
                            ActivationToken = a.ActivationToken,
                            Created = a.Created,
                            UserId = a.UserId,
                            UserAccountId = a.UserAccountId,
                            DisplayName = a.DisplayName,
                            Notes = a.Notes,
                            Active = a.Active,
                            OnboardingComplete = a.OnboardingComplete,
                            AccountType = accountTypes.SingleOrDefault(at => at.AccountTypeId == a.AccountTypeId)
                        }).FirstOrDefault();

                if (user != null)
                {
                    user.UserAccount = account;

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
                        user.Address = address;
                    }

                    user.Settings = ctx.Database.SqlQuery<Setting>("exec dbo.usp_GetSettingByUserId @UserId={0}", id).FirstOrDefault();

                }
                return user;
            }
        }

        public List<BusidexUser> GetAllBusidexUsers()
        {
            using (var ctx = new busidexEntities())
            {
                var users = ctx.Database.SqlQuery<BusidexUser>(
                    "exec dbo.usp_GetAllBusidexUsers").ToList();

                return users.Select(bu => new BusidexUser
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

            }
        }

        List<AccountType> getActivePlans()
        {
            using (var ctx = new busidexEntities())
            {
                return ctx.Database.SqlQuery<AccountType>("exec dbo.usp_GetActivePlans").ToList();
            }
        }

        public void AddUserDeviceType(long userId, DeviceType deviceType)
        {
            using (var ctx = new busidexEntities())
            {
                 ctx.Database.ExecuteSqlCommand("usp_AddUserDevice @Userid={0}, @DeviceTypeId={1}", userId, deviceType);
            }
        }

        public UserDevice GetUserDevice(long userId, DeviceType deviceType)
        {
            using (var ctx = new busidexEntities())
            {
                return ctx.Database.SqlQuery<UserDevice>("exec dbo.usp_GetUserDevice @UserId={0}, @DeviceTypeId={1}",
                    userId, deviceType).SingleOrDefault();
            }
        }

        public void UpdateUserDevice(UserDevice device)
        {
            using (var ctx = new busidexEntities())
            {
                ctx.Database.ExecuteSqlCommand("usp_UpdateUserDevice @Userid={0}, @DeviceTypeId={1}, @Version={2}",
                    device.UserId, device.DeviceTypeId, device.Version);
            }
        }
        #region Admin
        public Dictionary<string, int> GetPopularTags()
        {
            using (var ctx = new busidexEntities())
            {
                var tags = ctx.Database.SqlQuery<usp_GetPopularTagsResult>(
                    "exec dbo.usp_GetPopularTags").ToList();

                return tags.ToDictionary(t => t.text, t => t.cnt.GetValueOrDefault());
            }
            
        }
        #endregion

    }
}