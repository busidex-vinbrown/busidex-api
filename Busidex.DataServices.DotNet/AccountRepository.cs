
using Busidex.DomainModels.DotNet;
using Busidex.DomainModels.DotNet.DTO;

namespace Busidex.DataServices.DotNet
{
    public class AccountRepository : RepositoryBase, IAccountRepository
    {

        public AccountRepository(string connectionString)
            : base(connectionString)
        {
        }

        public List<Tag> GetUserAccountTags(long userId)
        {
            throw new NotImplementedException();
            //return _dao.GetUserAccountTags(userId);
        }

        public long AddUserAccountTag(long userAccountId, long tagId)
        {
            throw new NotImplementedException();
            //return _dao.AddUserAccountTag(userAccountId, tagId);
        }

        public void UpdateUserAccount(long userId, int accountTypeId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateUserName(long userId, string newUserName)
        {
            throw new NotImplementedException();
        }

        public bool UpdateEmail(long userId, string email)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdatePassword(string userName, string password)
        {
            throw new NotImplementedException();
        }

        public BusidexUser GetUserByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public BusidexUser GetUserByUserName(string userName)
        {
            throw new NotImplementedException();
        }

        public BusidexUser GetUserByDisplayName(string userName)
        {
            throw new NotImplementedException();
        }

        public void SaveUserAccountToken(long userId, Guid token)
        {
            throw new NotImplementedException();
        }

        public void SaveUserAccountCode(long userId, string code)
        {
            throw new NotImplementedException();
        }

        public bool UpdateDisplayName(long userAccountId, string name)
        {
            throw new NotImplementedException();
        }

        public UserAccount GetUserAccountByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public bool SaveBusidexUser(BusidexUser user)
        {
            throw new NotImplementedException();
        }

        public UserAccount GetUserAccountByCode(string code)
        {
            throw new NotImplementedException();
        }

        public UserAccount GetUserAccountByToken(Guid token)
        {
            throw new NotImplementedException();
        }

        public async Task<UserAccount> GetUserAccountByUserId(long userId)
        {
            return await _dao.GetUserAccountByUserId(userId);
        }

        public UserAccount GetUserAccountByPhoneNumber(string phoneNumber)
        {
            throw new NotImplementedException();
        }

        public bool ActivateUserAccount(string code)
        {
            /*UserAccount ua;
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
            */
            throw new NotImplementedException();
        }

        public bool ActivateUserAccount(long userId)
        {
            /*
            UserAccount ua = _dao.GetUserAccountByUserId(userId);

            if (ua == null)
            {
                return false;
            }

            ua.Active = true;
            var rowsUpdated = BusidexDAL.ActivateUserAccount(ua.UserAccountId);
            return rowsUpdated > 0;
            */
            throw new NotImplementedException();
        }

        public UserAccount AddUserAccount(UserAccount account)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Page> GetAllSitePages()
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public EmailTemplate GetEmailTemplate(EmailTemplateCode code)
        {
            throw new NotImplementedException();
        }

        public async Task SaveCommunication(Communication communication)
        {
            await _dao.SaveCommunication(communication);
        }

        public List<Suggestion> GetAllSuggestions()
        {
            throw new NotImplementedException();
        }

        public void AddNewSuggestion(Suggestion suggestion)
        {
            throw new NotImplementedException();
        }

        public void UpdateSuggestionVoteCount(int suggestionId)
        {
            throw new NotImplementedException();
        }

        public void SaveUserAccountDeactivateToken(long userId, string token)
        {
            throw new NotImplementedException();
        }

        public UserAccount GetUserAccountByDeactivateToken(string token)
        {
            throw new NotImplementedException();
        }

        public void DeleteUserAccount(long userId)
        {
            throw new NotImplementedException();
        }

        public void UpdateOnboardingComplete(long userId, bool complete)
        {
            throw new NotImplementedException();
        }

        public void AddUserDeviceType(long userId, DeviceType device)
        {
            throw new NotImplementedException();
        }
    }
}
