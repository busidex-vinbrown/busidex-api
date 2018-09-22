using System;
using System.Collections.Generic;
using System.Linq;
using Busidex.BL.Interfaces;
using Busidex.DAL;
using Busidex.Providers;

namespace Busidex.BL
{
    public class AccountRepository : RepositoryBase, IAccountRepository
    {
        public AccountRepository(IBusidexDataContext busidexDal)
            : base(busidexDal)
        {
        }

        public void SaveUserAccountToken(long userId, Guid token)
        {
            if (userId > 0)
            {
                BusidexDAL.SaveUserAccountToken(userId, token);
            }
        }

        public UserAccount GetUserAccountByEmail(string email)
        {
            return BusidexDAL.GetUserAccountByEmail(email);
        }

        public bool SaveBusidexUser(BusidexUser user)
        {
            return BusidexDAL.SaveBusidexUser(user);
        }

        public UserAccount GetUserAccountByToken(Guid token)
        {
            return BusidexDAL.GetUserAccountByToken(token);
        }

        public UserAccount GetUserAccountByUserId(long userId)
        {
            return BusidexDAL.GetUserAccountByUserId(userId);
        }

        public bool ActivateUserAccount(Guid token)
        {
            UserAccount ua = BusidexDAL.GetUserAccountByToken(token);
            if (ua != null)
            {
                ua.Active = true;
                var rowsUpdated = BusidexDAL.ActivateUserAccount(ua.UserAccountId);
                return rowsUpdated > 0;
            }
            return false;
        }

        public UserAccount AddUserAccount(UserAccount account)
        {
            return BusidexDAL.AddUserAccount(account);
        }

        public IEnumerable<Page> GetAllSitePages()
        {
            return BusidexDAL.GetAllSitePages();
        }

        public IEnumerable<UserCard> GetMyBusidex(long userId, bool includeImages = false)
        {
            if (Bcp.GetFromCache(BusidexCacheProvider.CachKeys.MyBusidex) == null)
            {
                Bcp.UpdateCache(BusidexCacheProvider.CachKeys.MyBusidex, BusidexDAL.GetMyBusidex(userId, includeImages));
            }
            return Bcp.GetFromCache(BusidexCacheProvider.CachKeys.MyBusidex) as List<UserCard>;
        }

        public void SignOut()
        {
            Bcp.UpdateCache(BusidexCacheProvider.CachKeys.MyBusidex, null);
        }

        public IEnumerable<AccountType> GetActivePlans()
        {
            return BusidexDAL.GetActivePlans();
        }

        public EmailTemplate GetEmailTemplate(EmailTemplateCode code)
        {
            return BusidexDAL.GetEmailTemplate(code);
        }

        public void SaveCommunication(Communication communication)
        {
            BusidexDAL.SaveCommunication(communication);
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
    }
}
