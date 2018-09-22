using System;
using System.Collections.Generic;
using Busidex.DAL;

namespace Busidex.BL.Interfaces
{
    public interface IAccountRepository
    {

        void SaveUserAccountToken(long userId, Guid token);
        UserAccount GetUserAccountByToken(Guid token);
        UserAccount GetUserAccountByUserId(long userId);
        UserAccount AddUserAccount(UserAccount account);
        BusidexUser GetBusidexUserById(long userId);
        bool ActivateUserAccount(Guid token);
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
    }
}
