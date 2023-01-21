using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using Busidex.Api.DataAccess.DTO;
using Busidex.Api.Models;

namespace Busidex.Api.DataServices.Interfaces
{
    public interface ICardRepository
    {
        List<SEOCardResult> GetSeoCardResult();
        EmailTemplate GetSharedCardEmailPreview(SharedCard model);
        Task SendSharedCardInvitation(SharedCard model);
        Task SendSharedCard(SharedCard model);
        List<Communication> GetCommunications(string[] emails, long userId);
        Task AddSharedCardsToQueue(string connectionString, List<SharedCard> sharedCardList);
        Task AddSharedCardToQueue(string connectionString, SharedCard sharedCard);
        CardDetailModel GetFeaturedCard();
        List<EventSource> GetAllEventSources();
        void AddEventActivity(EventActivity activity);
        List<EventActivity> GetEventActivities(long cardId, byte month);
        Task CardToFile(long cardId, bool replaceFront, bool replaceBack, byte[] frontImage, Guid frontFileId, string frontType, byte[] backImage, Guid backFileId, string backType, long userId);
        AddOrEditCardModel GetAddOrEditModel(long cardId, BusidexUser bu, string action);
        AddOrEditCardModel GetAddOrEditModel(AddOrEditCardModel model);

        SearchResultModel Search(SearchResultModel model, long? userId, Tuple<double, double> altAddress);
        List<UserCard> GetMyBusidex(long userId, bool includeImages);
        List<GroupCard> GetBusiGroupCards(long groupId, bool includeImages = false);
        List<Group> GetMyBusiGroups(long userId);
        Group GetBusiGroupById(long groupId);
        Group GetGroupByName(string groupName);
        long AddUserGroupCard(UserGroupCard groupCard);
        void AddUserGroupCards(string cardIds, long groupId, long userId);
        void AddGroupCards(long groupId, string cardIds);
        void RemoveGroupCards(long groupId, string cardIds, long userId);
        void AddGroup(Group group, string cardIds);
        long UpdateGroup(Group group);
        void DeleteGroup(long id);
        List<CardImage> SyncData(long id);

        SharedCard GetSharedCard(long cardId, long sendFrom, long shareWith);
        List<SharedCard> Get30DaySharedCards();

        void SaveGroupCardNotes(long id, string notes);
       
        CardDetailModel GetCardDetail(DataAccess.DTO.Card card);
        BusidexUser GetBusidexUserById(long userId);
        bool SaveCardOwnerToken(long cardId, Guid token);
        Task SaveCardOwner(string connectionString, string json);
        CardDetailModel GetCardByToken(string token);
        CardDetailModel GetCardByEmail(string email);
        List<CardDetailModel> GetCardsByPhoneNumber(long userId, string phoneNumber);
        Card GetCardById(long cardId, long userId = 0);
        void SaveCardStub();
        List<CardDetailModel> GetCardsByOwnerId(long ownerId);
        void DeleteCard(long id, long userId);
        void SaveCardNotes(long id, string notes);
        UserCard GetUserCard(long id, long userId);
        UserCard GetUserCardLite(long id, long userId);

        void DeleteUserCard(UserCard uc, long userId);
        Task AddToMyBusidex(long cardId, long userId);
        Task AddSendersCardToMyBusidex(string token, long userId);
        void SaveSharedCard(SharedCard sharedCard);
        void SaveSharedCards(List<SharedCard> sharedCards);
        List<SharedCard> GetSharedCards(long userId);
        Task AcceptSharedCard(long cardId, long userId);
        void DeclineSharedCard(long cardId, long userId);
        Task UpdateCardFileId(long cardId, Guid frontFileId, string frontType, Guid backFileId, string backType);
        List<Card> GetAllCards();
        List<UnownedCard> GetUnownedCards();
        bool IsACardOwner(long ownerId);
        Task<long> AddCard(DataAccess.DTO.Card card, bool isMyCard, long userId, string notes);
        Task<AddOrUpdateCardErrors> EditCard(Card cardModel, bool isMyCard, long userId, string notes);
        void UpdateCardLinks(long cardId, List<DataAccess.ExternalLink> links);
        void InvalidateBusidexCache();
        List<StateCode> GetAllStateCodes();
        List<Card> GetDuplicateCardsByEmail(long cardId, string email);
        void RelateCards(long ownerId, long relatedCardId);
        void UnRelateCards(long ownerId, long relatedCardId);
        void UpdateCardBasicInfo(long cardId, string name, string company, string phone, string email);
        Task UpdateCardOrientation(long cardId, string frontOrientation, string backOrientation);
        void UpdateMobileView(long id, bool isMobileView);
        long GetCardCount();
        AddOrUpdateCardErrors CheckForCardModelErrors(Card cardModel, bool isMyCard);
        Task UploadCardUpdateToBlobStorage(AddOrEditCardModel model, string cardUpdateStorageConnectionString, string cardRef);
        void AddCardToQueue(string connectionString, string cardUpdateRef);
        Task SaveApplicationError(string error, string innerException, string stackTrace, long userId);
        Task SaveApplicationError(Exception ex, long userId);
        Image ScaleImage(Image image, string orientation);
        void ClearBusidexCache();
        SearchResultModel SearchBySystemTag(string systag, long? userId);
        SearchResultModel SearchByGroupName(string groupName, long? userId);
        List<EventTag> GetEventTags();
        void UpdateUserCardStatus(long userCardId, UserCardAddStatus status);
        void AddSystemTagToCard(long cardid, string tag);
        OrgCardDetailModel GetOrganizationCardByOwnerId(long ownerId);
        Task SendCardUpdatedEmails();
        Task SendOwnerNotificationOfSharedCard(SharedCard model);
    }
}
