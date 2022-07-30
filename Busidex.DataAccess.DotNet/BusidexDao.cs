using Busidex.DomainModels.DotNet;
using Busidex.DomainModels.DotNet.DTO;
using Microsoft.Data.SqlClient;

namespace Busidex.DataAccess.DotNet
{
    public class BusidexDao
    {
        private readonly string? _connectionString;

        public BusidexDao() { _connectionString = null; }
        public BusidexDao(string connectionString)
        {
            _connectionString = connectionString;
        }

        internal async Task<IEnumerable<T>> ExecuteSqlCommandAsync<T>(string sql, List<SqlParameter> sqlParams)
            where T : new()
        {
            try
            {
                using (var con = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand(sql, con))
                    {
                        try
                        {                          
                            con.Open();
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;

                            cmd.Parameters.AddRange(sqlParams.ToArray());
                            var outputParam = sqlParams.Where(p => p.Direction == System.Data.ParameterDirection.Output).SingleOrDefault();

                            using (var sdr = await cmd.ExecuteReaderAsync())
                            {
                                var results = new List<T>();
                                while (sdr.Read())
                                {
                                    results.Add( (T)Activator.CreateInstance(typeof(T), sdr));                                   
                                }

                                //var result = sdr.ConvertTo<T>();
                                if (outputParam != null) return new List<T> { (T)outputParam.Value };
                                else return results;
                            }
                        }
                        finally
                        {
                            con.Close();
                        }
                    }
                }
            }catch(Exception ex)
            {

            }
            return null;
        }

        internal async Task ExecuteSqlNonQueryAsync(string sql, List<SqlParameter> sqlParams)
        {
            try
            {
                using (var con = new SqlConnection(_connectionString))
                {
                    using (var cmd = new SqlCommand(sql, con))
                    {
                        try
                        {                            
                            con.Open();
                            cmd.CommandType = System.Data.CommandType.StoredProcedure;

                            cmd.Parameters.AddRange(sqlParams.ToArray());
                            
                            await cmd.ExecuteNonQueryAsync();
                        }
                        finally
                        {
                            con.Close();
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public async Task<List<Communication>> GetCommunications(string[] emails, long userId)
        {
            var sql = $"usp_GetCommunications";
            var sqlParams = new List<SqlParameter>();
            sqlParams.Add(new SqlParameter("@EmailList", string.Join(",", emails)));
            sqlParams.Add(new SqlParameter("@SentById", userId));
            var result = await ExecuteSqlCommandAsync<Communication>(sql, sqlParams);
            return result.ToList();
        }

        public async Task SaveCommunication(Communication communication)
        {
            var sql = $"usp_SaveCommunications";
            var sqlParams = new List<SqlParameter>();
            sqlParams.Add(new SqlParameter("@EmailTemplateId", communication.EmailTemplateId));
            sqlParams.Add(new SqlParameter("@UserId", communication.UserId));
            sqlParams.Add(new SqlParameter("@Email", communication.Email));
            sqlParams.Add(new SqlParameter("@Body", communication.Body));
            sqlParams.Add(new SqlParameter("@SentById", communication.SentById));
            sqlParams.Add(new SqlParameter("@OwnerToken", communication.OwnerToken));
            sqlParams.Add(new SqlParameter("@DateSent", communication.DateSent));
            sqlParams.Add(new SqlParameter("@Failed", communication.Failed));

            await ExecuteSqlNonQueryAsync(sql, sqlParams); 
        }

        public async Task SaveCardOwner(long cardId, long ownerId)
        {
            var sql = $"usp_SaveCardOwner";
            var sqlParams = new List<SqlParameter>();
            sqlParams.Add(new SqlParameter("@CardId", cardId));
            sqlParams.Add(new SqlParameter("@OwnerId", ownerId));

            await ExecuteSqlNonQueryAsync(sql, sqlParams);
        }

        public async Task<EmailTemplate?> GetEmailTemplate(EmailTemplateCode code)
        {
            var sql = $"usp_GetEmailTemplateByCode";
            var sqlParams = new List<SqlParameter>();
            sqlParams.Add(new SqlParameter("@code", code));

            var result = await ExecuteSqlCommandAsync<EmailTemplate>(sql, sqlParams);
            return result.SingleOrDefault();
        }
        /*
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
        */

        public async Task<Tag?> GetTag(TagType type, string tag)
        {
            var sql = $"usp_GetTag";
            var sqlParams = new List<SqlParameter>();
            sqlParams.Add(new SqlParameter("@TagTypeId", (int)type));
            sqlParams.Add(new SqlParameter("@Tag", tag));

            var result = await ExecuteSqlCommandAsync<Tag>(sql, sqlParams);
            return result.SingleOrDefault();
        }

        public async Task AddTag(long cardId, Tag tag)
        {
            if (!string.IsNullOrEmpty(tag.Text) && tag.Text.Trim().Equals("NEST", StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }

            var sql = $"usp_AddCardTag";
            var sqlParams = new List<SqlParameter>();
            sqlParams.Add(new SqlParameter("@CardId", cardId));
            sqlParams.Add(new SqlParameter("@TagText", tag.Text));

            await ExecuteSqlNonQueryAsync(sql, sqlParams);      
        }

        public async Task AddTag(string text, int type)
        {
            if (!string.IsNullOrEmpty(text) && text.Trim().Equals("NEST", StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }

            var sql = $"usp_AddTag";
            var sqlParams = new List<SqlParameter>();
            sqlParams.Add(new SqlParameter("@Text", text));
            sqlParams.Add(new SqlParameter("@TagTypeId", type));

            await ExecuteSqlNonQueryAsync(sql, sqlParams);
        }

        /*
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
        */

        public async Task<BusidexUser?> GetUserByUserName(string userName)
        {
            var sql = $"usp_GetBusidexUserByUserName";
            var sqlParams = new List<SqlParameter>();
            sqlParams.Add(new SqlParameter("@UserName", userName));

            var result = await ExecuteSqlCommandAsync<BusidexUser>(sql, sqlParams);
            return result.SingleOrDefault();
        }

        public async Task<BusidexUser?> GetUserByDisplayName(string userName)
        {
            var sql = $"usp_GetBusidexUserByDisplayName";
            var sqlParams = new List<SqlParameter>();
            sqlParams.Add(new SqlParameter("@UserName", userName));

            var result = await ExecuteSqlCommandAsync<BusidexUser>(sql, sqlParams);
            return result.SingleOrDefault();
        }

        public async Task<BusidexUser?> GetUserByEmail(string email)
        {
            var sql = $"usp_GetUserByEmail";
            var sqlParams = new List<SqlParameter>();
            sqlParams.Add(new SqlParameter("@email", email));

            var result = await ExecuteSqlCommandAsync<BusidexUser>(sql, sqlParams);
            return result.SingleOrDefault();
        }

        /*
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
        */

        public async Task<UserCard?> GetUserCard(long cardId, long userId)
        {
            var sql = $"usp_GetUserCard";
            var sqlParams = new List<SqlParameter>();
            sqlParams.Add(new SqlParameter("@cardId", cardId));
            sqlParams.Add(new SqlParameter("@userId", userId));

            var result = await ExecuteSqlCommandAsync<UserCard>(sql, sqlParams);
            var uc = result.SingleOrDefault();
            
            if (uc == null) return null;

            return new UserCard
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
        }

        public List<UserCard> GetMyBusidex(long userId, bool includeImages)
        {
            throw new NotImplementedException();
        }

        public async Task AddUserCard(UserCard userCard)
        {
            var sql = $"usp_AddUserCard";
            var sqlParams = new List<SqlParameter>();
            sqlParams.Add(new SqlParameter("@CardId", userCard.CardId));
            sqlParams.Add(new SqlParameter("@UserId", userCard.UserId));
            sqlParams.Add(new SqlParameter("@OwnerId", userCard.OwnerId));
            sqlParams.Add(new SqlParameter("@SharedById", userCard.SharedById));
            sqlParams.Add(new SqlParameter("@Notes", userCard.Notes));

            await ExecuteSqlNonQueryAsync(sql, sqlParams);
        }
        /*
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
        */

        public async Task<CardDetailModel?> GetCardByEmail(string email)
        {
            var sql = $"usp_GetCardsByEmail";
            var sqlParams = new List<SqlParameter>();
            sqlParams.Add(new SqlParameter("@email", email));
            sqlParams.Add(new SqlParameter("@latitude", DBNull.Value));
            sqlParams.Add(new SqlParameter("@longitude", DBNull.Value));
            sqlParams.Add(new SqlParameter("@radiusInMiles", DBNull.Value));

            var result = await ExecuteSqlCommandAsync<CardDetailModel>(sql, sqlParams);
            return result.SingleOrDefault();
        }

        public async Task<List<CardDetailModel>> GetCardsByPhoneNumber(long userId, string phoneNumber)
        {
            var sql = $"usp_getCardsByPhoneNumber";
            var sqlParams = new List<SqlParameter>();
            sqlParams.Add(new SqlParameter("@userId", userId));
            sqlParams.Add(new SqlParameter("@phone", phoneNumber));
            sqlParams.Add(new SqlParameter("@latitude", DBNull.Value));
            sqlParams.Add(new SqlParameter("@longitude", DBNull.Value));
            sqlParams.Add(new SqlParameter("@radiusInMiles", DBNull.Value));

            var result = await ExecuteSqlCommandAsync<CardDetailModel>(sql, sqlParams);
            return result.ToList();
        }
        /*
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
        */

        public async Task DeletePhoneNumber(long phoneNumberId)
        {
            var sql = $"usp_DeletePhoneNumber";
            var sqlParams = new List<SqlParameter>();
            sqlParams.Add(new SqlParameter("@PhoneNumberId", phoneNumberId));

            await ExecuteSqlNonQueryAsync(sql, sqlParams);
        }

        public async Task<PhoneNumber?> GetPhoneNumberById(long id)
        {
            var sql = $"usp_getPhoneNumberById";
            var sqlParams = new List<SqlParameter>();
            sqlParams.Add(new SqlParameter("@id", id));

            var result = await ExecuteSqlCommandAsync<PhoneNumber>(sql, sqlParams);
            return result.SingleOrDefault();
        }

        public async Task UpdatePhoneNumber(PhoneNumber phoneNumber)
        {
            var sql = $"usp_UpdatePhoneNumber";
            var sqlParams = new List<SqlParameter>();
            sqlParams.Add(new SqlParameter("@PhoneNumberId", phoneNumber.PhoneNumberId));
            sqlParams.Add(new SqlParameter("@PhoneNumberTypeId", phoneNumber.PhoneNumberTypeId));
            sqlParams.Add(new SqlParameter("@CardId", phoneNumber.CardId));
            sqlParams.Add(new SqlParameter("@Number", phoneNumber.Number));
            sqlParams.Add(new SqlParameter("@Extension", phoneNumber.Extension));

            await ExecuteSqlNonQueryAsync(sql, sqlParams);
        }

        public async Task AddPhoneNumber(PhoneNumber phoneNumber)
        {
            var sql = $"usp_AddPhoneNumber";
            var sqlParams = new List<SqlParameter>();
            sqlParams.Add(new SqlParameter("@PhoneNumberTypeId", phoneNumber.PhoneNumberTypeId));
            sqlParams.Add(new SqlParameter("@CardId", phoneNumber.CardId));
            sqlParams.Add(new SqlParameter("@Number", phoneNumber.Number));
            sqlParams.Add(new SqlParameter("@Extension", phoneNumber.Extension));

            await ExecuteSqlNonQueryAsync(sql, sqlParams);
        }

        public async Task<List<PhoneNumber>> GetCardPhoneNumber(long cardId)
        {
            var sql = $"usp_getCardPhoneNumber";
            var sqlParams = new List<SqlParameter>();
            sqlParams.Add(new SqlParameter("@cardId", cardId));

            var result = await ExecuteSqlCommandAsync<PhoneNumber>(sql, sqlParams);
            return result.ToList();
        }

        public async Task<List<PhoneNumberDTO>> GetCardPhoneNumbers(string cardIds)
        {
            var sql = $"usp_getCardPhoneNumbers";
            var sqlParams = new List<SqlParameter>();
            sqlParams.Add(new SqlParameter("@cardIds", cardIds));

            var result = await ExecuteSqlCommandAsync<PhoneNumberDTO>(sql, sqlParams);
            return result.ToList();
        }

        public async Task<List<Tag>> GetCardTags(long cardId)
        {
            var sql = $"usp_GetCardTags";
            var sqlParams = new List<SqlParameter>();
            sqlParams.Add(new SqlParameter("@cardId", cardId));

            var result = await ExecuteSqlCommandAsync<Tag>(sql, sqlParams);
            var list = result.ToList();
            return list;
        }

        public async Task<List<CardTag>> GetCardTagsByIds(string cardIds)
        {
            var sql = $"usp_GetCardTagsByIds";
            var sqlParams = new List<SqlParameter>();
            sqlParams.Add(new SqlParameter("@cardIds", cardIds));

            var result = await ExecuteSqlCommandAsync<CardTag>(sql, sqlParams);
            return result.ToList();
        }

        public async Task<List<CardAddress>> GetCardAddresses(long cardId)
        {
            var sql = $"usp_GetCardAddresses";
            var sqlParams = new List<SqlParameter>();
            sqlParams.Add(new SqlParameter("@cardId", cardId));

            var result = await ExecuteSqlCommandAsync<CardAddress>(sql, sqlParams);
            return result.ToList();
        }

        public async Task AddAddress(long cardId, CardAddress address)
        {
            var sql = $"usp_AddCardAddress";
            var sqlParams = new List<SqlParameter>();
            sqlParams.Add(new SqlParameter("@CardId", cardId));
            sqlParams.Add(new SqlParameter("@Address1", address.Address1));
            sqlParams.Add(new SqlParameter("@Address2", address.Address2));
            sqlParams.Add(new SqlParameter("@City", address.City));
            sqlParams.Add(new SqlParameter("@State", address.State?.Code));
            sqlParams.Add(new SqlParameter("@Zipcode", address.ZipCode));
            sqlParams.Add(new SqlParameter("@Region", address.Region));
            sqlParams.Add(new SqlParameter("@Country", address.Country));
            sqlParams.Add(new SqlParameter("@Latitude", address.Latitude));
            sqlParams.Add(new SqlParameter("@Longitude", address.Longitude));

            await ExecuteSqlNonQueryAsync(sql, sqlParams);
        }

        public async Task UpdateAddress(CardAddress address)
        {
            var sql = $"usp_UpdateCardAddress";
            var sqlParams = new List<SqlParameter>();
            sqlParams.Add(new SqlParameter("@CardAddressId", address.CardAddressId));
            sqlParams.Add(new SqlParameter("@Address1", address.Address1));
            sqlParams.Add(new SqlParameter("@Address2", address.Address2));
            sqlParams.Add(new SqlParameter("@City", address.City));
            sqlParams.Add(new SqlParameter("@State", address.State?.Code));
            sqlParams.Add(new SqlParameter("@Zipcode", address.ZipCode));
            sqlParams.Add(new SqlParameter("@Region", address.Region));
            sqlParams.Add(new SqlParameter("@Country", address.Country));
            sqlParams.Add(new SqlParameter("@Latitude", address.Latitude));
            sqlParams.Add(new SqlParameter("@Longitude", address.Longitude));

            await ExecuteSqlNonQueryAsync(sql, sqlParams);
        }

        public async Task DeleteAddress(long cardAddressId)
        {
            var sql = $"usp_DeleteCardAddress";
            var sqlParams = new List<SqlParameter>();
            sqlParams.Add(new SqlParameter("@CarAddressdId", cardAddressId));

            await ExecuteSqlNonQueryAsync(sql, sqlParams);
        }

        public async Task<List<ExternalLink>> GetExternalLinks(long cardId)
        {
            var sql = $"usp_GetExternalLinksByCardId";
            var sqlParams = new List<SqlParameter>();
            sqlParams.Add(new SqlParameter("@cardId", cardId));

            var result = await ExecuteSqlCommandAsync<ExternalLink>(sql, sqlParams);
            var links = result.ToList();

            //foreach (var link in links)
            //{
            //    sql = "usp_GetExternalLinksById";
            //    var sqlParams2 = new List<SqlParameter>();
            //    sqlParams2.Add(new SqlParameter("@ExternalLinkTypeId", link.ExternalLinkTypeId));

            //    var linkType = await ExecuteSqlCommandAsync<ExternalLinkType>(sql, sqlParams2);
            //    link.ExternalLinkType = linkType.Single();
            //}
            return links;
        }

        public async Task<List<CardAddressDTO>> GetCardAddressesByIds(string cardIds)
        {
            var sql = $"usp_GetCardAddressesByIds";
            var sqlParams = new List<SqlParameter>();
            sqlParams.Add(new SqlParameter("@CardIds", cardIds));

            var result = await ExecuteSqlCommandAsync<CardAddressDTO>(sql, sqlParams);
            return result.ToList();
        }

        public async Task<List<StateCode>> GetAllStateCodes()
        {
            var sql = $"usp_GetAllStateCodes";
            var sqlParams = new List<SqlParameter>();

            var result = await ExecuteSqlCommandAsync<StateCode>(sql, sqlParams);
            return result.ToList();
        }

        public async Task<List<PhoneNumberType>> GetAllPhoneNumberTypes()
        {
            var sql = $"usp_GetAllPhoneNumberTypes";
            var sqlParams = new List<SqlParameter>();

            var result = await ExecuteSqlCommandAsync<PhoneNumberType>(sql, sqlParams);
            return result.ToList();
        }
        /*
        public List<long> GetRecentlyUpdatedCards()
        {

            using (var ctx = new busidexEntities())
            {
                return ctx.Database.SqlQuery<long>("exec dbo.usp_GetRecentlyUpdatedCards").ToList();
            }

        }
        */

        private async Task<Card> _populateCard(Card c)
        {
            var card = c;
            var cardPhoneNumbers = await GetCardPhoneNumber(card.CardId);
            var phoneNumberTypes = (await GetAllPhoneNumberTypes()).ToDictionary(t => t.PhoneNumberTypeId, t => t.Name);

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

            var tags = await GetCardTags(card.CardId);
            var cardTags = tags.Select(t => new Tag
            {
                TagId = t.TagId,
                Text = t.Text,
                TagType = (TagType)t.TagTypeId
            }).ToList();

            var addresses = await GetCardAddresses(card.CardId);
            var stateCodes = await GetAllStateCodes();
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
                State = stateCodes.SingleOrDefault(sc => sc.StateCodeId == a.StateCodeId),
                ZipCode = a.ZipCode
            }).ToList();

            var links = await GetExternalLinks(card.CardId);

            card.PhoneNumbers = phoneNumbers;
            card.Tags = cardTags;
            card.Addresses = cardAddresses;
            card.ExternalLinks = links;

            return card;

        }

        public async Task<Card?> GetCardById(long id, long userId = 0)
        {
            var sql = $"usp_getCardById";
            var sqlParams = new List<SqlParameter>();
            sqlParams.Add(new SqlParameter("@cardId", id));
            sqlParams.Add(new SqlParameter("@userId", userId));

            var result = await ExecuteSqlCommandAsync<Card>(sql, sqlParams);
            var card = result.SingleOrDefault();
            if (card != null)
            {
                return await _populateCard(card);
            }
            return null;            
        }

        public async Task<List<EventSource>> GetAllEventSources()
        {
            var sql = $"usp_GetAllEventSources";
            var sqlParams = new List<SqlParameter>();
            var result = await ExecuteSqlCommandAsync<EventSource>(sql, sqlParams);
            return result.ToList();
        }

        public async Task AddEventActivity(EventActivity ea)
        {
            var sql = $"usp_GetAllEventSources";
            var sqlParams = new List<SqlParameter>();
            sqlParams.Add(new SqlParameter("@EventSourceId", ea.EventSourceId));
            sqlParams.Add(new SqlParameter("@CardId", ea.CardId));
            sqlParams.Add(new SqlParameter("@UserId", ea.UserId));
            sqlParams.Add(new SqlParameter("@ActivityDate", ea.ActivityDate));

            await ExecuteSqlCommandAsync<EventSource>(sql, sqlParams);
        }

        public async Task<Card?> GetCardByToken(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return null;
            }

            var ownerToken = new Guid(token);
            var sql = $"usp_GetCardByOwnerToken";
            var sqlParams = new List<SqlParameter>();
            sqlParams.Add(new SqlParameter("@Token", ownerToken));

            var result = await ExecuteSqlCommandAsync<Card>(sql, sqlParams);
            var card = result.SingleOrDefault(); 
            if (card != null)
            {
                return await _populateCard(card);
            }
            return null;
        }
        /*
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
        

        

        public IEnumerable<Card> SearchCards(string criteria, double? latitude, double? longitude, int? distance, bool searchableOnly, CardType cardType, long? userId = 0)
        {
            throw new NotImplementedException();
        }


        public List<Card> SearchBySystemTag(string systag, long? userId)
        {
            throw new NotImplementedException();
        }

        public List<Card> SearchByGroupName(string groupName, long? userId)
        {
            throw new NotImplementedException();
        }
        */
        public async Task SaveApplicationError(string error, string innerException, string stackTrace, long userId)
        {
            var sql = $"usp_AddApplicationError";
            var sqlParams = new List<SqlParameter>();
            sqlParams.Add(new SqlParameter("@message", error));
            sqlParams.Add(new SqlParameter("@innerException", innerException));
            sqlParams.Add(new SqlParameter("@stackTrace", stackTrace));
            sqlParams.Add(new SqlParameter("@userId", userId));

            await ExecuteSqlNonQueryAsync(sql, sqlParams);
        }

        public async Task<List<SEOCardResult>> GetSeoCardResult()
        {
            var sql = "usp_getSEOCardNames";
            var sqlParams = new List<SqlParameter>();

            var result = await ExecuteSqlCommandAsync<SEOCardResult>(sql, sqlParams);
            return result.ToList();
        }
        /*
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
        */

        public async Task DeleteTag(long cardId, long tagId)
        {
            var sql = $"usp_DeleteCardTag";
            var sqlParams = new List<SqlParameter>();
            sqlParams.Add(new SqlParameter("@CardId", cardId));
            sqlParams.Add(new SqlParameter("@TagId", tagId));

            await ExecuteSqlCommandAsync<SEOCardResult>(sql, sqlParams);
        }
        /*
        public void DeleteUserAccount(long userId)
        {
            using (var ctx = new busidexEntities())
            {
                ctx.Database.ExecuteSqlCommand(
                    "exec dbo.usp_DeleteUserAccount @UserId={0}",
                    userId);
            }
        }
        */

        public async Task UpdateUserCard(long userCardId, string notes)
        {
            var sql = $"usp_UpdateUserCard";
            var sqlParams = new List<SqlParameter>();
            sqlParams.Add(new SqlParameter("@UserCardId", userCardId));
            sqlParams.Add(new SqlParameter("@Notes", notes));

            await ExecuteSqlNonQueryAsync(sql, sqlParams);        
        }

        public async Task UpdateUserCardStatus(long userCardId, UserCardAddStatus status)
        {
            var sql = $"usp_UpdateUserCardStatus";
            var sqlParams = new List<SqlParameter>();
            sqlParams.Add(new SqlParameter("@UserCardId", userCardId));
            sqlParams.Add(new SqlParameter("@Status", status));

            await ExecuteSqlNonQueryAsync(sql, sqlParams);                    
        }

        /*
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
        */

        public async Task<long> AddCard(Card card)
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

            var sql = "usp_addCard";
            var sqlParams = new List<SqlParameter>();
            sqlParams.Add(new SqlParameter("@Name", card.Name));
            sqlParams.Add(new SqlParameter("@FrontImage", card.FrontImage));
            sqlParams.Add(new SqlParameter("@FrontType", card.FrontType));
            sqlParams.Add(new SqlParameter("@FrontOrientation", card.FrontOrientation));
            sqlParams.Add(new SqlParameter("@BackImage", card.BackImage));
            sqlParams.Add(new SqlParameter("@BackType", card.BackType));
            sqlParams.Add(new SqlParameter("@BackOrientation", card.BackOrientation));
            sqlParams.Add(new SqlParameter("@BusinessId", card.BusinessId));
            sqlParams.Add(new SqlParameter("@Searchable", card.Searchable));
            sqlParams.Add(new SqlParameter("@CompanyName", card.Name));
            sqlParams.Add(new SqlParameter("@Email", card.Email));
            sqlParams.Add(new SqlParameter("@Url", card.Url));
            sqlParams.Add(new SqlParameter("@CreatedBy", card.CreatedBy));
            sqlParams.Add(new SqlParameter("@OwnerId", card.OwnerId));
            sqlParams.Add(new SqlParameter("@OwnerToken", card.OwnerToken));
            sqlParams.Add(new SqlParameter("@FrontFileId", card.FrontFileId));
            sqlParams.Add(new SqlParameter("@BackFileId", card.BackFileId));
            sqlParams.Add(new SqlParameter("@DisplayType", card.Display.ToString()));
            sqlParams.Add(new SqlParameter("@Markup", card.Markup));
            sqlParams.Add(new SqlParameter("@Visibility", visibility));
            sqlParams.Add(new SqlParameter("@CardTypeId", 1));
            var outputParam = new SqlParameter("@CardId", System.Data.SqlDbType.BigInt);
            outputParam.Direction = System.Data.ParameterDirection.Output;
            sqlParams.Add(outputParam);

            var result = await ExecuteSqlCommandAsync<long>(sql, sqlParams);
            return result.SingleOrDefault();
        }

        public async Task UpdateCard(Card model)
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
            var sql = "usp_updateCard";
            var sqlParams = new List<SqlParameter>();
            sqlParams.Add(new SqlParameter("@CardId", model.CardId));
            sqlParams.Add(new SqlParameter("@Name", model.Name));
            sqlParams.Add(new SqlParameter("@Title", model.Title));
            sqlParams.Add(new SqlParameter("@FrontImage", model.FrontImage));
            sqlParams.Add(new SqlParameter("@FrontType", model.FrontType));
            sqlParams.Add(new SqlParameter("@FrontOrientation", model.FrontOrientation));
            sqlParams.Add(new SqlParameter("@BackImage", model.BackImage));
            sqlParams.Add(new SqlParameter("@BackType", model.BackType));
            sqlParams.Add(new SqlParameter("@BackOrientation", model.BackOrientation));
            sqlParams.Add(new SqlParameter("@BusinessId", model.BusinessId));
            sqlParams.Add(new SqlParameter("@Searchable", model.Searchable));
            sqlParams.Add(new SqlParameter("@CompanyName", model.Name));
            sqlParams.Add(new SqlParameter("@Email", model.Email));
            sqlParams.Add(new SqlParameter("@Url", model.Url));
            sqlParams.Add(new SqlParameter("@CreatedBy", model.CreatedBy));
            sqlParams.Add(new SqlParameter("@OwnerId", model.OwnerId));
            sqlParams.Add(new SqlParameter("@OwnerToken", model.OwnerToken));
            sqlParams.Add(new SqlParameter("@Deleted", model.Deleted));
            sqlParams.Add(new SqlParameter("@FrontFileId", model.FrontFileId));
            sqlParams.Add(new SqlParameter("@BackFileId", model.BackFileId));
            sqlParams.Add(new SqlParameter("@DisplayType", model.Display.ToString()));
            sqlParams.Add(new SqlParameter("@Markup", model.Markup));
            sqlParams.Add(new SqlParameter("@Visibility", visibility));
    
            await ExecuteSqlNonQueryAsync(sql, sqlParams);
        }

        public async Task UpdateCardFileId(long cardId, Guid frontFileId, string frontType, Guid backFileId, string backType)
        {
            var sql = $"usp_UpdateCardFileId";
            var sqlParams = new List<SqlParameter>();
            sqlParams.Add(new SqlParameter("@CardId", cardId));
            sqlParams.Add(new SqlParameter("@FrontFileId", frontFileId));
            sqlParams.Add(new SqlParameter("@FrontType", frontType));
            sqlParams.Add(new SqlParameter("@BackFileId", backFileId));
            sqlParams.Add(new SqlParameter("@BackType", backType));

            await ExecuteSqlNonQueryAsync(sql, sqlParams);
        }

        public async Task UpdateCardOrientation(long cardId, string frontOrientation, string backOrientation)
        {
            var sql = $"usp_UpdateCardOrientation";
            var sqlParams = new List<SqlParameter>();
            sqlParams.Add(new SqlParameter("@CardId", cardId));
            sqlParams.Add(new SqlParameter("@FrontOrientation", frontOrientation));
            sqlParams.Add(new SqlParameter("@BackOrientation", backOrientation));

            await ExecuteSqlNonQueryAsync(sql, sqlParams);
        }
        /*
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
        */

        public async Task<UserAccount?> GetUserAccountByPhoneNumber(string phoneNumber)
        {
            var sql = $"usp_GetUserAccountByPhoneNumber";
            var sqlParams = new List<SqlParameter>();
            sqlParams.Add(new SqlParameter("@PhoneNumber", phoneNumber));

            var result = await ExecuteSqlCommandAsync<UserAccount>(sql, sqlParams);
            var userAccount = result.SingleOrDefault();
            if (userAccount == null)
            {
                return null;
            }

            var accountTypes = await getActivePlans();
            userAccount.AccountType = accountTypes.SingleOrDefault(at => at.AccountTypeId == userAccount.AccountTypeId);
            userAccount.BusidexUser = await GetBusidexUserById(userAccount.UserId);
         
            return userAccount;
        }
        
        public async Task<UserAccount?> GetUserAccountByUserId(long userId)
        {
            var sql = $"usp_GetUserAccountByUserId";
            var sqlParams = new List<SqlParameter>();
            sqlParams.Add(new SqlParameter("@UserId", userId));

            var result = await ExecuteSqlCommandAsync<UserAccount>(sql, sqlParams);
            var userAccount = result.SingleOrDefault();

            if(userAccount != null)
            {
                userAccount.BusidexUser = await GetBusidexUserById(userId);

                var accountTypes = await getActivePlans();
                userAccount.AccountType = accountTypes.SingleOrDefault(at => at.AccountTypeId == userAccount.AccountTypeId);
            }

            return userAccount;
        }

        /*
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
        */

        public async Task<BusidexUser> GetBusidexUserById(long id)
        {
            var sql = $"usp_GetBusidexUserByUserId";
            var sqlParams = new List<SqlParameter>();
            sqlParams.Add(new SqlParameter("@UserId", id));

            var result = await ExecuteSqlCommandAsync<BusidexUser>(sql, sqlParams);
            var busidexUser = result.SingleOrDefault();

            sql = $"usp_GetUserAddress";

            var result2 = await ExecuteSqlCommandAsync<UserAddress>(sql, sqlParams);
            var address = result2.SingleOrDefault();

            sql = "usp_GetActivePlans";
            var result3 = await ExecuteSqlCommandAsync<AccountType>(sql, new List<SqlParameter>());
            var accountTypes = result3.ToList();

            sql = $"usp_GetUserAccountByUserId";
            var result4 = await ExecuteSqlCommandAsync<UserAccount>(sql, sqlParams);
            var a = result4.SingleOrDefault();

            var account =
                 new UserAccount
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
                 };

            if (busidexUser != null)
            {
                busidexUser.UserAccount = account;

                if (address == null)
                {
                    busidexUser.Address = new UserAddress
                    {
                        UserId = id,
                        UserAddressId = 0
                    };
                }
                else
                {
                    busidexUser.Address = address;
                }

                sql = $"usp_GetSettingByUserId";
                var results5 = await ExecuteSqlCommandAsync<Setting>(sql, sqlParams);
                busidexUser.Settings = results5.FirstOrDefault();

            }
            
            return busidexUser;
        }

        /*
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
        */

        private async Task<List<AccountType>> getActivePlans()
        {
            var sql = $"usp_GetActivePlans";
            var result = await ExecuteSqlCommandAsync<AccountType>(sql, new List<SqlParameter>());
            return result.ToList();
        }

        /*
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
        */
    }
}