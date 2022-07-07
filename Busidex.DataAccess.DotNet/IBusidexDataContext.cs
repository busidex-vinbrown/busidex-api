using Busidex.DomainModels.DotNet;
using Busidex.DomainModels.DotNet.DTO;

namespace Busidex.DataAccess.DotNet
{
    public interface IBusidexDataContext
    {
        void UpdateUserAccount(long userId, int accountTypeId);
        List<EventSource> GetAllEventSources();
        void AddEventActivity(EventActivity activity);
        List<EventActivity> GetEventActivities(long cardId, byte month); 
        bool UpdateUserName(long userId, string newUserName);
        IEnumerable<AccountType> GetActivePlans();
        List<PhoneNumberType> GetAllPhoneNumberTypes();
        IEnumerable<Page> GetAllSitePages();
        int GetCardCount();
        //List<Tag> GetCardTags(long cardId);
        PhoneNumber GetPhoneNumberById(long id);
        string GetCustomContentById(int id);
        void RelateCards(long ownerId, long relatedCardId);
        void UnRelateCards(long ownerId, long relatedCardId);
        List<UnownedCard> GetAllUnownedCards();
        Setting GetUserSetting(BusidexUser user);
        Setting AddDefaultUserSetting(BusidexUser user);
        Setting SaveSetting(Setting setting);
        void UpdateSetting(Setting setting);
        bool SaveUserPassword(string userName, string password);
        void SaveUserAccountCode(long userId, string code);
        void SaveUserAccountToken(long userId, Guid token);
        bool SaveCardOwnerToken(long cardId, Guid token);
        bool SaveBusidexUser(BusidexUser user);
        UserAccount GetUserAccountByCode(string code);
        UserAccount GetUserAccountByToken(Guid token);
        UserAccount GetUserAccountByEmail(string email);
        int ActivateUserAccount(long userAccountId);
        List<CardDetailModel> GetCardsByOwnerId(long ownerId);
        bool IsACardOwner(long ownerId);
        void DeleteUserCard(UserCard uc, long userId);
        List<CardImage> SyncData(long id);
        void SaveApplicationError(string error, string innerException, string stackTrace, long userId);
        //void SaveCommunication(Communication communication);
        Communication GetCommunicationByActivationToken(string token);
        void SaveSharedCards(List<SharedCard> sharedCards);
        void UpdateCardFileId(long cardId, Guid frontFileId, Guid backFileId);
        List<Card> GetAllCards();
        List<UnownedCard> GetUnownedCards();
        long AddCard(Card card);
        void UpdateCard(Card card);
        void DeleteCard(long id);
       // EmailTemplate GetEmailTemplate(EmailTemplateCode code);
        long AddPhoneNumber(PhoneNumber phoneNumber);
        void UpdatePhonenumber(PhoneNumber phoneNumber);
        void UpdateUserCard(long userCardId, string notes);
        void UpdateUserGroupCard(long userCardId, string notes);
        //void AddTag(long cardId, Tag tag);
        //void DeleteTag(long cardId, long tagId);
        void AddAddress(long cardId, CardAddress address);
        void UpdateAddress(CardAddress address);
        void DeleteAddress(long cardAddressId);
        List<StateCode> GetAllStateCodes();
        List<Card> GetDuplicateCardsByEmail(long cardId, string email);
        List<Suggestion> GetAllSuggestions();
        void AddNewSuggestion(Suggestion suggestion);
        void UpdateSuggestionVoteCount(int suggestionId);
        List<Group> GetMyBusiGroups(long userId);
        List<UserGroupCard> GetBusiGroupCards(long groupId, bool includeImages);
        Group GetBusiGroupById(long groupId);
        long AddUserGroupCard(UserGroupCard groupCard);
        long AddUserGroupCards(string cardIds, long groupId, long userId);
        long AddGroup(Group group);
        long UpdateGroup(Group group);
        void DeleteGroup(long id);
        long? RemoveUserGroupCards(string cardIds, long groupId, long userId);
        void UpdateCardBasicInfo(long cardId, string name, string company, string phone, string email);
        void UpdateMobileView(long id, bool isMobileView);
    }
}
