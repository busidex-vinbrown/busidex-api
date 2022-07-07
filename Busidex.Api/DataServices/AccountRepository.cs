using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Busidex.Api.DataAccess;
using Busidex.Api.DataAccess.DTO;
using Busidex.Api.DataServices.Interfaces;
using Busidex.Api.Models;
using AccountType = Busidex.Api.DataAccess.DTO.AccountType;

namespace Busidex.Api.DataServices
{
    public class AccountRepository : RepositoryBase, IAccountRepository
    {

        public AccountRepository(IBusidexDataContext busidexDal)
            : base(busidexDal)
        {
        }

        public List<Tag> GetUserAccountTags(long userId)
        {
            return _dao.GetUserAccountTags(userId);
        }

        public long AddUserAccountTag(long userAccountId, long tagId)
        {
            return _dao.AddUserAccountTag(userAccountId, tagId);       
        }

        public void UpdateUserAccount(long userId, int accountTypeId)
        {
            BusidexDAL.UpdateUserAccount(userId, accountTypeId);    
        }

        public async Task<bool> UpdateUserName(long userId, string newUserName)
        {
            try
            {
                BusidexDAL.UpdateUserName(userId, newUserName);
            }
            catch (Exception ex)
            {
                await SaveApplicationError(ex, userId);
                return false;
            }
            return true;
        }

        public bool UpdateEmail(long userId, string email)
        {
            return _dao.UpdateEmail(userId, email);
        }

        public async Task<bool> UpdatePassword(string userName, string password)
        {
            try
            {
                BusidexDAL.SaveUserPassword(userName, password);
            }
            catch (Exception ex)
            {
                await SaveApplicationError(ex, 0);
                return false;
            }
            return true;
        }

        public BusidexUser GetUserByEmail(string email)
        {
            return _dao.GetUserByEmail(email);
        }

        public BusidexUser GetUserByUserName(string userName)
        {
            return _dao.GetUserByUserName(userName);
        }

        public BusidexUser GetUserByDisplayName(string userName)
        {
            return _dao.GetUserByDisplayName(userName);
        }

        public void SaveUserAccountToken(long userId, Guid token)
        {
            if (userId > 0)
            {
                BusidexDAL.SaveUserAccountToken(userId, token);
            }
        }

        public void SaveUserAccountCode(long userId, string code)
        {
            if (userId > 0)
            {
                BusidexDAL.SaveUserAccountCode(userId, code);
            }
        }

        public bool UpdateDisplayName(long userAccountId, string name)
        {
            return _dao.UpdateDisplayName(userAccountId, name);
        }

        public UserAccount GetUserAccountByEmail(string email)
        {
            return BusidexDAL.GetUserAccountByEmail(email);
        }

        public bool SaveBusidexUser(BusidexUser user)
        {
            return BusidexDAL.SaveBusidexUser(user);
        }

        public UserAccount GetUserAccountByCode(string code)
        {
            return BusidexDAL.GetUserAccountByCode(code);
        }

        public UserAccount GetUserAccountByToken(Guid token)
        {
            return BusidexDAL.GetUserAccountByToken(token);
        }

        public UserAccount GetUserAccountByUserId(long userId)
        {
            return _dao.GetUserAccountByUserId(userId);
        }

        public UserAccount GetUserAccountByPhoneNumber(string phoneNumber)
        {
            return _dao.GetUserAccountByPhoneNumber(phoneNumber);
        }

        public bool ActivateUserAccount(string code)
        {
            UserAccount ua;
            const int GUID_LENGTH = 36;
            if (code.Length == GUID_LENGTH)
            {
                var token = new Guid(code);
                ua = BusidexDAL.GetUserAccountByToken(token);
            }
            else
            {
                ua = BusidexDAL.GetUserAccountByCode(code);
            }

            if (ua == null)
            {
                return false;
            }

            ua.Active = true;
            var rowsUpdated = BusidexDAL.ActivateUserAccount(ua.UserAccountId);
            return rowsUpdated > 0;
        }

        public bool ActivateUserAccount(long userId)
        {
            UserAccount ua = _dao.GetUserAccountByUserId(userId);
           
            if (ua == null)
            {
                return false;
            }

            ua.Active = true;
            var rowsUpdated = BusidexDAL.ActivateUserAccount(ua.UserAccountId);
            return rowsUpdated > 0;
        }

        public UserAccount AddUserAccount(UserAccount account)
        {
            return _dao.AddUserAccount(account);
        }

        public IEnumerable<Page> GetAllSitePages()
        {
            return BusidexDAL.GetAllSitePages();
        }

        public IEnumerable<UserCard> GetMyBusidex(long userId, bool includeImages = false)
        {
            return _dao.GetMyBusidex(userId, includeImages);
        }

        public void SignOut()
        {
            //Bcp.UpdateCache(BusidexCacheProvider.CachKeys.MyBusidex, null);
        }

        public IEnumerable<AccountType> GetActivePlans()
        {
            return BusidexDAL.GetActivePlans();
        }

        public EmailTemplate GetEmailTemplate(EmailTemplateCode code)
        {
            return _dao.GetEmailTemplate(code);
        }

        public void SaveCommunication(Communication communication)
        {
            _dao.SaveCommunication(communication);
        }

        public List<Suggestion> GetAllSuggestions()
        {
            var list = BusidexDAL.GetAllSuggestions();
            return list.OrderByDescending(s => s.Votes).ToList();
        }

        public void AddNewSuggestion(Suggestion suggestion)
        {
            BusidexDAL.AddNewSuggestion(suggestion);
        }

        public void UpdateSuggestionVoteCount(int suggestionId)
        {
            BusidexDAL.UpdateSuggestionVoteCount(suggestionId);
        }

        public void SaveUserAccountDeactivateToken(long userId, string token)
        {
            _dao.SaveUserAccountDeactivateToken(userId, token);
        }

        public UserAccount GetUserAccountByDeactivateToken(string token)
        {
            return _dao.GetUserAccountByDeactivateToken(token);
        }

        public void DeleteUserAccount(long userId)
        {
            _dao.DeleteUserAccount(userId);
        }

        public void UpdateOnboardingComplete(long userId, bool complete)
        {
            _dao.UpdateOnboardingComplete(userId, complete);
        }

        public void AddUserDeviceType(long userId, DeviceType device)
        {
            _dao.AddUserDeviceType(userId, device);
            ;
        }   
    }
}
