using System;
using System.Collections.Generic;

namespace Busidex.DAL
{
    public interface IBusidexDataContext
    {
        IEnumerable<AccountType> GetActivePlans();
        List<PhoneNumberType> GetAllPhoneNumberTypes();
        IEnumerable<Page> GetAllSitePages();
        BusidexUser GetBusidexUserById(long id);
        Card GetCardById(long id);
        int GetCardCount();
        IEnumerable<Card> GetCardsByEmail(string email, double? latitude, double? longitude, int? distance);
        IEnumerable<Card> GetCardsByNameKeyword(string text, double? latitude, double? longitude, int? distance);
        IEnumerable<Card> GetCardsByCompanyName(string name, double? latitude, double? longitude, int? distance);
        IEnumerable<Card> GetCardsByPhoneNumber(string phone, double? latitude, double? longitude, int? distance);
        IEnumerable<Card> GetCardsByTitle(string title, double? latitude, double? longitude, int? distance);
        IEnumerable<Card> SearchCards(string criteria, double? latitude, double? longitude, int? distance, bool searchableOnly);
        PhoneNumber GetPhoneNumberById(long id);
        string GetCustomContentById(int id);
        List<UserCard> GetMyBusidex(long id, bool includeImages);
        UserCard GetUserCard(long cardId, long userId);
        void RelateCards(long ownerId, long relatedCardId);
        void UnRelateCards(long ownerId, long relatedCardId);
        List<Card> GetAllUnownedCards();

        Setting GetUserSetting(BusidexUser user);
        Setting AddDefaultUserSetting(BusidexUser user);
        Setting SaveSetting(Setting setting);
        void UpdateSetting(Setting setting);

        List<BusidexUser> GetAllBusidexUsers();

        void SaveUserAccountToken(long userId, Guid token);
        bool SaveCardOwnerToken(long cardId, Guid token);

        bool SaveBusidexUser(BusidexUser user);

        UserAccount GetUserAccountByToken(Guid token);
        UserAccount GetUserAccountByUserId(long userId);
        UserAccount GetUserAccountByEmail(string email);
        UserAccount AddUserAccount(UserAccount ua);
        int ActivateUserAccount(long userAccountId);

        Card GetCardByToken(string token);
        bool SaveCardOwner(long cardId, long ownerId);
        List<Card> GetCardsByOwnerId(long ownerId);
        bool IsACardOwner(long ownerId);

        void DeleteUserCard(UserCard uc, long userId);
        UserCard GetUserCardById(long id);
        List<CardImage> SyncData(long id);

        void SaveApplicationError(string error, string innerException, string stackTrace, long userId);

        void SaveCommunication(Communication communication);

        Communication GetCommunicationByActivationToken(string token);
        void SaveSharedCards(List<SharedCard> sharedCards);
        List<SharedCard> GetSharedCards(long userId);
        void AcceptSharedCard(long cardId, long userId);
        void DeclineSharedCard(long cardId, long userId);

        void UpdateCardFileId(long cardId, Guid frontFileId, Guid backFileId);
        List<Card> GetAllCards();
        List<Card> GetUnownedCards();
        long AddCard(Card card);
        void UpdateCard(Card card);
        void DeleteCard(long id);

        EmailTemplate GetEmailTemplate(EmailTemplateCode code);

        long AddPhoneNumber(PhoneNumber phoneNumber);
        void UpdatePhonenumber(PhoneNumber phoneNumber);

        long AddUserCard(UserCard userCard);
        void UpdateUserCard(long userCardId, string notes);
        void UpdateUserGroupCard(long userCardId, string notes);
        void AddTag(long cardId, Tag tag);
        void DeleteTag(long cardId, long tagId);

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
