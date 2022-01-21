using Busidex.Api.DataAccess.DTO;
using Busidex.Api.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
/*
namespace Busidex.Api.DataAccess
{
    public class BusidexDataContext : IBusidexDataContext, IDisposable
    {
        private readonly string _constr;
        public BusidexDataContext(string connectionString)
        {
            _constr = connectionString;
        }

        public int ActivateUserAccount(long userAccountId)
        {
            using (var conn = new SqlConnection(_constr))
            {
                conn.Open();

                // 1.  create a command object identifying the stored procedure
                SqlCommand cmd = new SqlCommand("CustOrderHist", conn);

                // 2. set the command object so it knows to execute a stored procedure
                cmd.CommandType = CommandType.StoredProcedure;

                // 3. add parameter to command, which will be passed to the stored procedure
                cmd.Parameters.Add(new SqlParameter("@CustomerID", custId));

                // execute the command
                using (SqlDataReader rdr = cmd.ExecuteReader())
                {
                    // iterate through results, printing each to console
                    while (rdr.Read())
                    {
                        Console.WriteLine("Product: {0,-35} Total: {1,2}", rdr["ProductName"], rdr["Total"]);
                    }
                }
            }
        }

        public void AddAddress(long cardId, DTO.CardAddress address)
        {
            throw new NotImplementedException();
        }

        public long AddCard(DTO.Card card)
        {
            throw new NotImplementedException();
        }

        public Setting AddDefaultUserSetting(BusidexUser user)
        {
            throw new NotImplementedException();
        }

        public void AddEventActivity(EventActivity activity)
        {
            throw new NotImplementedException();
        }

        public long AddGroup(Group group)
        {
            throw new NotImplementedException();
        }

        public void AddNewSuggestion(Suggestion suggestion)
        {
            throw new NotImplementedException();
        }

        public long AddPhoneNumber(PhoneNumber phoneNumber)
        {
            throw new NotImplementedException();
        }

        public long AddUserGroupCard(UserGroupCard groupCard)
        {
            throw new NotImplementedException();
        }

        public long AddUserGroupCards(string cardIds, long groupId, long userId)
        {
            throw new NotImplementedException();
        }

        public void DeleteAddress(long cardAddressId)
        {
            throw new NotImplementedException();
        }

        public void DeleteCard(long id)
        {
            throw new NotImplementedException();
        }

        public void DeleteGroup(long id)
        {
            throw new NotImplementedException();
        }

        public void DeleteUserCard(UserCard uc, long userId)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DTO.AccountType> GetActivePlans()
        {
            throw new NotImplementedException();
        }

        public List<DTO.Card> GetAllCards()
        {
            throw new NotImplementedException();
        }

        public List<EventSource> GetAllEventSources()
        {
            throw new NotImplementedException();
        }

        public List<PhoneNumberType> GetAllPhoneNumberTypes()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Page> GetAllSitePages()
        {
            throw new NotImplementedException();
        }

        public List<StateCode> GetAllStateCodes()
        {
            throw new NotImplementedException();
        }

        public List<Suggestion> GetAllSuggestions()
        {
            throw new NotImplementedException();
        }

        public List<UnownedCard> GetAllUnownedCards()
        {
            throw new NotImplementedException();
        }

        public Group GetBusiGroupById(long groupId)
        {
            throw new NotImplementedException();
        }

        public List<UserGroupCard> GetBusiGroupCards(long groupId, bool includeImages)
        {
            throw new NotImplementedException();
        }

        public int GetCardCount()
        {
            throw new NotImplementedException();
        }

        public List<CardDetailModel> GetCardsByOwnerId(long ownerId)
        {
            throw new NotImplementedException();
        }

        public Communication GetCommunicationByActivationToken(string token)
        {
            throw new NotImplementedException();
        }

        public string GetCustomContentById(int id)
        {
            throw new NotImplementedException();
        }

        public List<DTO.Card> GetDuplicateCardsByEmail(long cardId, string email)
        {
            throw new NotImplementedException();
        }

        public List<EventActivity> GetEventActivities(long cardId, byte month)
        {
            throw new NotImplementedException();
        }

        public List<Group> GetMyBusiGroups(long userId)
        {
            throw new NotImplementedException();
        }

        public PhoneNumber GetPhoneNumberById(long id)
        {
            throw new NotImplementedException();
        }

        public List<SystemSettingDto> GetSystemSettings()
        {
            throw new NotImplementedException();
        }

        public List<UnownedCard> GetUnownedCards()
        {
            throw new NotImplementedException();
        }

        public UserAccount GetUserAccountByCode(string code)
        {
            throw new NotImplementedException();
        }

        public UserAccount GetUserAccountByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public UserAccount GetUserAccountByToken(Guid token)
        {
            throw new NotImplementedException();
        }

        public Setting GetUserSetting(BusidexUser user)
        {
            throw new NotImplementedException();
        }

        public bool IsACardOwner(long ownerId)
        {
            throw new NotImplementedException();
        }

        public void RelateCards(long ownerId, long relatedCardId)
        {
            throw new NotImplementedException();
        }

        public long? RemoveUserGroupCards(string cardIds, long groupId, long userId)
        {
            throw new NotImplementedException();
        }

        public void SaveApplicationError(string error, string innerException, string stackTrace, long userId)
        {
            throw new NotImplementedException();
        }

        public bool SaveBusidexUser(BusidexUser user)
        {
            throw new NotImplementedException();
        }

        public bool SaveCardOwnerToken(long cardId, Guid token)
        {
            throw new NotImplementedException();
        }

        public Setting SaveSetting(Setting setting)
        {
            throw new NotImplementedException();
        }

        public void SaveSharedCards(List<SharedCard> sharedCards)
        {
            throw new NotImplementedException();
        }

        public void SaveUserAccountCode(long userId, string code)
        {
            throw new NotImplementedException();
        }

        public void SaveUserAccountToken(long userId, Guid token)
        {
            throw new NotImplementedException();
        }

        public bool SaveUserPassword(string userName, string password)
        {
            throw new NotImplementedException();
        }

        public List<CardImage> SyncData(long id)
        {
            throw new NotImplementedException();
        }

        public void UnRelateCards(long ownerId, long relatedCardId)
        {
            throw new NotImplementedException();
        }

        public void UpdateAddress(DTO.CardAddress address)
        {
            throw new NotImplementedException();
        }

        public void UpdateCard(DTO.Card card)
        {
            throw new NotImplementedException();
        }

        public void UpdateCardBasicInfo(long cardId, string name, string company, string phone, string email)
        {
            throw new NotImplementedException();
        }

        public void UpdateCardFileId(long cardId, Guid frontFileId, Guid backFileId)
        {
            throw new NotImplementedException();
        }

        public long UpdateGroup(Group group)
        {
            throw new NotImplementedException();
        }

        public void UpdateMobileView(long id, bool isMobileView)
        {
            throw new NotImplementedException();
        }

        public void UpdatePhonenumber(PhoneNumber phoneNumber)
        {
            throw new NotImplementedException();
        }

        public void UpdateSetting(Setting setting)
        {
            throw new NotImplementedException();
        }

        public void UpdateSuggestionVoteCount(int suggestionId)
        {
            throw new NotImplementedException();
        }

        public void UpdateUserAccount(long userId, int accountTypeId)
        {
            throw new NotImplementedException();
        }

        public void UpdateUserCard(long userCardId, string notes)
        {
            throw new NotImplementedException();
        }

        public void UpdateUserGroupCard(long userCardId, string notes)
        {
            throw new NotImplementedException();
        }

        public bool UpdateUserName(long userId, string newUserName)
        {
            throw new NotImplementedException();
        }
    }
}

*/