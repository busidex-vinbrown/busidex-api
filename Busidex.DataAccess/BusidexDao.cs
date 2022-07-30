﻿using Busidex.DomainModels;
using Busidex.DomainModels.DTO;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Busidex.DataAccess
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
                    ctx.Database.SqlQuery<Tag>("exec dbo.usp_GetTag @TagTypeId={0}, @Tag={1}", (int)type, tag)
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

        public List<DomainModels.ApplicationError> GetApplicationErrors(int daysBack)
        {
            using (var ctx = new busidexEntities())
            {
                return
                    ctx.Database.SqlQuery<DomainModels.ApplicationError>("exec dbo.usp_GetApplicationErrors @DaysBack={0}", daysBack)
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

        public async Task<bool> SaveCardOwner(long cardId, long ownerId)
        {
            bool ok;

            using (var ctx = new busidexEntities())
            {
                using (var trns = ctx.Database.BeginTransaction())
                {
                    try
                    {
                        var c = ctx.Cards.Find(cardId);
                        if (c != null)
                        {
                            c.Deleted = false;
                            c.Searchable = true; // Now that the card is owned, it is searchable
                            c.OwnerId = ownerId;
                            ctx.SaveChanges();
                        }

                        ok = true;
                    }
                    catch (Exception ex)
                    {
                        trns.Rollback();
                        ok = false;
                        await SaveApplicationError(ex.Message, ex.InnerException?.Message, ex.StackTrace, ownerId);
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
            throw new NotImplementedException();
        }

        public async Task AddUserCard(UserCard userCard)
        {
            using (var ctx = new busidexEntities())
            {
                await ctx.Database.ExecuteSqlCommandAsync(
                    "exec dbo.usp_AddUserCard @CardId={0}, @UserId={1}, @OwnerId={2}, @SharedById={3}, @Notes={4}",
                    userCard.CardId, userCard.UserId, userCard.OwnerId, userCard.SharedById, userCard.Notes);
            }
        }

        public async Task AcceptSharedCard(long cardId, long userId)
        {
            var sharedCards = GetSharedCards(userId);
            var sharedCard = sharedCards.FirstOrDefault(c => c.CardId == cardId);
            var theirBusidex = GetMyBusidex(userId, false);
            //var card = GetCardById(cardId);
            if (sharedCard != null)
            {
                if (theirBusidex.All(c => c.CardId != sharedCard.CardId))
                {
                    await AddUserCard(new UserCard
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
                    await ctx.Database.ExecuteSqlCommandAsync("exec dbo.usp_AcceptSharedCard @UserId={0}, @CardId={1}", userId, cardId);
                    await ctx.Database.ExecuteSqlCommandAsync("exec dbo.usp_updateSharedById @CardId={0}, @UserId={1}", cardId, sharedCard.SendFrom);
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

        public List<DomainModels.DTO.SharedCard> GetSharedCards(long userId)
        {
            using (var ctx = new busidexEntities())
            {
                var sharedCards = ctx.Database.SqlQuery<DomainModels.DTO.SharedCard>(
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

        public List<DomainModels.DTO.SharedCard> GetOrganizationInvitations(long userId)
        {

            using (var ctx = new busidexEntities())
            {
                var sharedCards = ctx.Database.SqlQuery<DomainModels.DTO.SharedCard>(
                        "exec dbo.usp_GetSharedCardsByUserId @userId={0}", userId).ToList();

                foreach (var sharedCard in sharedCards)
                {
                    sharedCard.Card = GetCardById(sharedCard.CardId, userId);
                    sharedCard.Card.FrontImage = sharedCard.Card.BackImage = null;
                }
                return sharedCards.Where(c => c.Card.CardType == CardType.Organization).ToList();
            }
        }

        public void SaveSharedCard(DomainModels.DTO.SharedCard sharedCard)
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

        public List<DomainModels.DTO.SharedCard> Get30DaySharedCards()
        {
            using (var ctx = new busidexEntities())
            {
                var sharedCards = ctx.Database.SqlQuery<DomainModels.DTO.SharedCard>("exec dbo.usp_GetUnclaimedSharedCards").ToList();
                return sharedCards.Where(c => c.SharedDate < DateTime.Today.AddDays(-30)).ToList();
            }

        }

        public DomainModels.DTO.SharedCard GetSharedCard(long cardId, long sendFrom, long shareWith)
        {
            using (var ctx = new busidexEntities())
            {
                return ctx.Database.SqlQuery<DomainModels.DTO.SharedCard>("exec dbo.usp_GetSharedCard @CardId={0}, @SendFrom={1}, @ShareWith={2}", cardId, sendFrom, shareWith).FirstOrDefault();
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

        public DomainModels.DTO.PhoneNumber GetPhoneNumberById(long id)
        {
            using (var ctx = new busidexEntities())
            {
                return
                    ctx.Database.SqlQuery<DomainModels.DTO.PhoneNumber>("exec dbo.usp_getPhoneNumberById @id={0}", id).SingleOrDefault();
            }
        }

        public async Task UpdatePhoneNumber(DomainModels.DTO.PhoneNumber phoneNumber)
        {
            using (var ctx = new busidexEntities())
            {
                await ctx.Database.ExecuteSqlCommandAsync("exec dbo.usp_UpdatePhoneNumber @PhoneNumberId={0}, @PhoneNumberTypeId={1}, @CardId={2}, @Number={3}, @Extension={4}", phoneNumber.PhoneNumberId, phoneNumber.PhoneNumberTypeId, phoneNumber.CardId, phoneNumber.Number, phoneNumber.Extension);
            }
        }

        public async Task AddPhoneNumber(DomainModels.DTO.PhoneNumber phoneNumber)
        {
            using (var ctx = new busidexEntities())
            {
                await ctx.Database.ExecuteSqlCommandAsync("exec dbo.usp_AddPhoneNumber @PhoneNumberTypeId={0}, @CardId={1}, @Number={2}, @Extension={3}", phoneNumber.PhoneNumberTypeId, phoneNumber.CardId, phoneNumber.Number, phoneNumber.Extension);
            }
        }

        public List<DomainModels.DTO.PhoneNumber> GetCardPhoneNumber(long cardId)
        {
            using (var ctx = new busidexEntities())
            {
                return
                    ctx.Database.SqlQuery<DomainModels.DTO.PhoneNumber>("exec dbo.usp_getCardPhoneNumber @cardId={0}", cardId).ToList();
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

        public List<DomainModels.DTO.CardAddress> GetCardAddresses(long cardId)
        {
            using (var ctx = new busidexEntities())
            {
                return ctx.Database.SqlQuery<DomainModels.DTO.CardAddress>("exec dbo.usp_GetCardAddresses @cardId={0}", cardId).ToList();
            }
        }

        public async Task AddAddress(long cardId, DomainModels.DTO.CardAddress address)
        {
            using (var ctx = new busidexEntities())
            {
                await ctx.Database.ExecuteSqlCommandAsync("exec dbo.usp_AddCardAddress @CardId={0}, @Address1={1}, @Address2={2}, @City={3}, @State={4}, @Zipcode={5}, @Region={6}, @Country={7}, @Latitude={8}, @Longitude={9}",
                    cardId, address.Address1, address.Address2, address.City, address.State?.Code, address.ZipCode, address.Region, address.Country, address.Latitude, address.Longitude);
            }
        }

        public async Task UpdateAddress(DomainModels.DTO.CardAddress address)
        {
            using (var ctx = new busidexEntities())
            {
                await ctx.Database.ExecuteSqlCommandAsync("exec dbo.usp_UpdateCardAddress @CarAddressdId={0}, @Address1={1}, @Address2={2}, @City={3}, @State={4}, @Zipcode={5}, @Region={6}, @Country={7}, @Latitude={8}, @Longitude={9}",
                    address.CardAddressId, address.Address1, address.Address2, address.City, address.State?.Code, address.ZipCode, address.Region, address.Country, address.Latitude, address.Longitude);
            }
        }

        public async Task DeleteAddress(long cardAddressId)
        {
            using (var ctx = new busidexEntities())
            {
                await ctx.Database.ExecuteSqlCommandAsync("exec dbo.usp_DeleteCardAddress @CardAddressId={0}", cardAddressId);
            }
        }

        public List<ExternalLink> GetExternalLinks(long cardId)
        {
            using (var ctx = new busidexEntities())
            {
                var links = ctx.Database.SqlQuery<ExternalLink>("exec dbo.usp_GetExternalLinksByCardId @cardId={0}", cardId).ToList();
                foreach (var link in links)
                {
                    var linkType = ctx.Database.SqlQuery<ExternalLinkType>($"SELECT * from dbo.ExternalLinkType where ExternalLinkTypeId = {link.ExternalLinkTypeId}").FirstOrDefault();
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

        public List<DomainModels.StateCode> GetAllStateCodes()
        {
            using (var ctx = new busidexEntities())
            {
                return ctx.Database.SqlQuery<StateCode>("exec dbo.usp_GetAllStateCodes").ToList();
            }
        }

        public List<DomainModels.DTO.PhoneNumberType> GetAllPhoneNumberTypes()
        {
            using (var ctx = new busidexEntities())
            {
                return ctx.Database.SqlQuery<DomainModels.DTO.PhoneNumberType>("exec dbo.usp_GetAllPhoneNumberTypes").ToList();
            }
        }

        public List<long> GetRecentlyUpdatedCards()
        {

            using (var ctx = new busidexEntities())
            {
                return ctx.Database.SqlQuery<long>("exec dbo.usp_GetRecentlyUpdatedCards").ToList();
            }

        }

        private DomainModels.DTO.Card _populateCard(DomainModels.DTO.Card c)
        {
            var card = c;
            var cardPhoneNumbers = GetCardPhoneNumber(card.CardId);
            var phoneNumberTypes = GetAllPhoneNumberTypes().ToDictionary(t => t.PhoneNumberTypeId, t => t.Name);

            var phoneNumbers = cardPhoneNumbers.Select(p => new DomainModels.DTO.PhoneNumber
            {
                CardId = p.CardId,
                Created = p.Created,
                Deleted = p.Deleted,
                Extension = p.Extension,
                Number = p.Number,
                PhoneNumberTypeId = p.PhoneNumberTypeId,
                Updated = p.Updated,
                PhoneNumberId = p.PhoneNumberId,
                PhoneNumberType = new DomainModels.DTO.PhoneNumberType
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

            var addresses = GetCardAddresses(card.CardId);
            var stateCodes = GetAllStateCodes();
            var cardAddresses = addresses.Select(a => new DomainModels.DTO.CardAddress
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

        public DomainModels.DTO.Card GetCardById(long id, long userId = 0)
        {
            using (var ctx = new busidexEntities())
            {
                var card = ctx.Database.SqlQuery<DomainModels.DTO.Card>("exec dbo.usp_getCardById @cardId={0}, @userId={1}", id, userId).SingleOrDefault();

                if (card != null)
                {
                    return _populateCard(card);
                }
                return null;
            }
        }

        public DomainModels.DTO.Card GetCardByToken(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return null;
            }

            var ownerToken = new Guid(token);
            using (var ctx = new busidexEntities())
            {
                var card = ctx.Database.SqlQuery<DomainModels.DTO.Card>("exec dbo.usp_GetCardByOwnerToken @Token={0}", ownerToken).SingleOrDefault();

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
                    ctx.Database.SqlQuery<DomainModels.DTO.Card>("exec dbo.usp_GetOrganizationCards @organizationId={0}, @userId={1}",
                        organizationId, userId).ToList();
                var cardIds = string.Join(",", items.Select(c => c.CardId).ToArray());

                var phoneNumberData =
                    ctx.Database.SqlQuery<DomainModels.DTO.PhoneNumber>("exec dbo.usp_GetCardPhoneNumbers @cardIds={0}", cardIds)
                        .ToList();
                var phoneNumberTypes =
                    ctx.Database.SqlQuery<DomainModels.DTO.PhoneNumberType>("exec dbo.usp_GetAllPhoneNumberTypes")
                        .ToDictionary(t => t.PhoneNumberTypeId, t => t.Name);
                var tagData =
                    ctx.Database.SqlQuery<CardTag>("exec dbo.usp_GetCardTagsByIds @CardIds={0}", cardIds).ToList();
                var stateCodes = ctx.Database.SqlQuery<StateCode>("exec dbo.usp_GetAllStateCodes").ToList();
                var addressData =
                    ctx.Database.SqlQuery<DomainModels.DTO.CardAddress>("exec dbo.usp_GetCardAddressesByIds @CardIds={0}", cardIds)
                        .ToList();
                var cardAddresses = addressData.Select(a => new DomainModels.DTO.CardAddress
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

                var phoneNumbers = phoneNumberData.Select(p => new DomainModels.DTO.PhoneNumber
                {
                    CardId = p.CardId,
                    Created = p.Created,
                    Deleted = p.Deleted,
                    Extension = p.Extension,
                    Number = p.Number,
                    PhoneNumberTypeId = p.PhoneNumberTypeId,
                    Updated = p.Updated,
                    PhoneNumberId = p.PhoneNumberId,
                    PhoneNumberType = new DomainModels.DTO.PhoneNumberType
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
                                .Select(t => new Tag { TagId = t.TagId, Text = t.Text, TagType = (TagType)t.TagTypeId })
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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public void AddGroup(Group group, string cardIds)
        {
            throw new NotImplementedException();
        }


        public List<UserCard> GetOrganizationReferrals(long userId, long organizationId)
        {
            throw new NotImplementedException();
        }

        List<DomainModels.DTO.Card> RefineSearch(List<DomainModels.DTO.Card> list, string[] criteria)
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

        public async Task SaveApplicationError(string error, string innerException, string stackTrace, long userId)
        {
            using (var ctx = new busidexEntities())
            {
                await ctx.Database.ExecuteSqlCommandAsync(
                    "exec dbo.usp_AddApplicationError @message={0}, @innerException={1}, @stackTrace={2}, @userId={3}",
                    error, innerException, stackTrace, userId);
            }
        }

        public IEnumerable<DomainModels.DTO.Card> SearchCards(string criteria, double? latitude, double? longitude, int? distance, bool searchableOnly, CardType cardType, long? userId = 0)
        {
            throw new NotImplementedException();
        }


        public List<DomainModels.DTO.Card> SearchBySystemTag(string systag, long? userId)
        {
            throw new NotImplementedException();
        }

        public List<DomainModels.DTO.Card> SearchByGroupName(string groupName, long? userId)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
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

        public async Task UpdateUserCard(long userCardId, string notes)
        {
            using (var ctx = new busidexEntities())
            {
                await ctx.Database.ExecuteSqlCommandAsync(
                    "exec dbo.usp_UpdateUserCard @UserCardId={0}, @Notes={1}",
                    userCardId, notes);
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

        public async Task<long> AddCard(DomainModels.DTO.Card card)
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
            var cardId = new SqlParameter
            {
                ParameterName = "cardId",
                DbType = System.Data.DbType.Int64,
                Direction = System.Data.ParameterDirection.Output
            };

            using (var ctx = new busidexEntities())
            {
                await ctx.Database.ExecuteSqlCommandAsync("exec dbo.usp_addCard @Name={0}, @Title={1}, @FrontImage={2}, @FrontType={3}, @FrontOrientation={4}, @BackImage={5}, @BackType={6}, @BackOrientation={7}, @BusinessId={ 8},@Searchable={ 9}, @CompanyName={ 10}, @Email={ 11}, @Url={ 12}, @CreatedBy={ 13}, @OwnerId={ 14}, @OwnerToken={ 15}, @FrontFileId={ 16}, @BackFileId={ 17}, @DisplayType={ 1}, @Markup={ 19}, @Visibility={ 20}, @CardTypeId={ 21}, @CardId={22}",
                    card.Name, card.Title, card.FrontImage, card.FrontType, card.FrontOrientation,
                card.BackImage, card.BackType, card.BackOrientation,
                card.BusinessId, card.Searchable, card.CompanyName, card.Email, card.Url, card.CreatedBy,
                card.OwnerId, card.OwnerToken, card.FrontFileId, card.BackFileId, card.Display.ToString(),
                card.Markup, visibility, 1, cardId);
            }
            return Convert.ToInt64(cardId.Value);
        }

        public async Task UpdateCard(DomainModels.DTO.Card model)
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
            using (var ctx = new busidexEntities())
            {
                await ctx.Database.ExecuteSqlCommandAsync("exec dbo.usp_updateCard @CardId={0}, @Name={1},	@Title={2},	@FrontImage={3},	@FrontType={4},	@FrontOrientation={5},	@BackImage={6},	@BackType={7},	@BackOrientation={8},	@BusinessId={9},	@Searchable={10},	@CompanyName={11},	@Email={12},	@Url={13},	@CreatedBy={14},	@OwnerId={15},	@OwnerToken={16},	@Deleted={17},	@FrontFileId={18},	@BackFileId={19},	@DisplayType={20},	@Markup={21},	@Visibility={22}",
                    model.CardId, model.Name, model.Title, model.FrontImage, model.FrontType, model.FrontOrientation,
                       model.BackImage, model.BackType, model.BackOrientation, model.BusinessId, model.Searchable, model.CompanyName, model.Email,
                       model.Url, model.CreatedBy, model.OwnerId, model.OwnerToken, false, model.FrontFileId, model.BackFileId, model.Display.ToString(),
                       model.Markup, visibility);
            }
        }

        public async Task UpdateCardFileId(long cardId, Guid frontFileId, string frontType, Guid backFileId, string backType)
        {
            using (var ctx = new busidexEntities())
            {
                await ctx.Database.ExecuteSqlCommandAsync(
                    "exec dbo.usp_UpdateCardFileId @CardId={0}, @FrontFileId={1}, @FrontType={2}, @BackFileId={3}, @BackType={4}",
                    cardId, frontFileId, frontType, backFileId, backType);
            }
        }

        public async Task UpdateCardOrientation(long cardId, string frontOrientation, string backOrientation)
        {
            using (var ctx = new busidexEntities())
            {
                await ctx.Database.ExecuteSqlCommandAsync(
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

                var accountTypes = ctx.Database.SqlQuery<DomainModels.AccountType>("exec dbo.usp_GetActivePlans").ToList();

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

        List<DomainModels.AccountType> getActivePlans()
        {
            using (var ctx = new busidexEntities())
            {
                return ctx.Database.SqlQuery<DomainModels.AccountType>("exec dbo.usp_GetActivePlans").ToList();
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