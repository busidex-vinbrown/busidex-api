using System;
using System.Collections.Generic;
using Busidex.Api.DataAccess.DTO;
using Busidex.Api.Models;
using AccountType = Busidex.Api.DataAccess.DTO.AccountType;

namespace Busidex.Api.DataServices.Interfaces
{
    public interface IAccountRepository
    {
        List<Tag> GetUserAccountTags(long userId); 
        long AddUserAccountTag(long userAccountId, long tagId);
        void UpdateUserAccount(long userId, int accountTypeId);
        bool UpdateUserName(long userId, string newUserName);
        void SaveUserAccountToken(long userId, Guid token);
        void SaveUserAccountCode(long userId, string code);
        UserAccount GetUserAccountByToken(Guid token);
        UserAccount GetUserAccountByCode(string code);
        UserAccount GetUserAccountByUserId(long userId);
        UserAccount GetUserAccountByPhoneNumber(string phoneNumber);
        UserAccount AddUserAccount(UserAccount account);
        BusidexUser GetBusidexUserById(long userId);
        List<UserTerm> GetUserTerms(long userId);
        void AcceptUserTerms(long userId);
        bool ActivateUserAccount(string code);
        bool ActivateUserAccount(long userId);
        bool SaveBusidexUser(BusidexUser user);
        IEnumerable<Page> GetAllSitePages();
        IEnumerable<UserCard> GetMyBusidex(long userId, bool includeImages);
        void SignOut();
        IEnumerable<AccountType> GetActivePlans();
        void SaveCommunication(Communication communication);
        EmailTemplate GetEmailTemplate(EmailTemplateCode code);
        UserAccount GetUserAccountByEmail(string email);
        List<Suggestion> GetAllSuggestions();
        void AddNewSuggestion(Suggestion suggestion);
        void UpdateSuggestionVoteCount(int suggestionId);
        BusidexUser GetUserByEmail(string email);
        BusidexUser GetUserByUserName(string userName);
        BusidexUser GetUserByDisplayName(string userName);
        bool UpdatePassword(string userName, string password);
        void SaveUserAccountDeactivateToken(long userId, string token);
        UserAccount GetUserAccountByDeactivateToken(string token);
        void DeleteUserAccount(long userId);
        bool UpdateDisplayName(long userAccountId, string name);
        bool UpdateEmail(long userId, string email);
        void UpdateOnboardingComplete(long userId, bool complete);

        void AddUserDeviceType(long userId, DeviceType device);
    }
}
