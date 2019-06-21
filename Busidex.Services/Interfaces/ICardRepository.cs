using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Drawing;
using System.Net.Mime;
using Busidex.DomainModels;
using Busidex.DomainModels.DTO;

namespace Busidex.Services.Interfaces
{
    public interface ICardRepository
    {
        List<SEOCardResult> GetSeoCardResult();
        EmailTemplate GetSharedCardEmailPreview(SharedCard model);
        void SendSharedCardInvitation(SharedCard model);
        void SendSharedCard(SharedCard model);
        List<Communication> GetCommunications(string[] emails, long userId);
        void AddSharedCardsToQueue(List<SharedCard> sharedCardList);
        void AddSharedCardToQueue(SharedCard sharedCard);
        CardDetailModel GetFeaturedCard();
        List<EventSource> GetAllEventSources();
        void AddEventActivity(EventActivity activity);
        List<EventActivity> GetEventActivities(long cardId, byte month);
        void CardToFile(long cardId, bool replaceFront, bool replaceBack, Binary frontImage, Guid frontFileId, string frontType, Binary backImage, Guid backFileId, string backType, long userId);
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
       
        CardDetailModel GetCardDetail(DomainModels.Card card);
        BusidexUser GetBusidexUserById(long userId);
        bool SaveCardOwnerToken(long cardId, Guid token);
        bool SaveCardOwner(long cardId, long ownerId);
        CardDetailModel GetCardByToken(string token);
        CardDetailModel GetCardByEmail(string email);
        List<CardDetailModel> GetCardsByPhoneNumber(long userId, string phoneNumber);
        Card GetCardById(long cardId, long userId = 0);
        List<CardDetailModel> GetCardsByOwnerId(long ownerId);
        void DeleteCard(long id, long userId);
        void SaveCardNotes(long id, string notes);
        UserCard GetUserCard(long id, long userId);
        void DeleteUserCard(UserCard uc, long userId);
        void AddToMyBusidex(long cardId, long userId);
        void AddSendersCardToMyBusidex(string token, long userId);
        void SaveSharedCard(SharedCard sharedCard);
        void SaveSharedCards(List<SharedCard> sharedCards);
        List<SharedCard> GetSharedCards(long userId);
        void AcceptSharedCard(long cardId, long userId);
        void DeclineSharedCard(long cardId, long userId);
        void UpdateCardFileId(long cardId, Guid frontFileId, string frontType, Guid backFileId, string backType);
        List<Card> GetAllCards();
        List<UnownedCard> GetUnownedCards();
        bool IsACardOwner(long ownerId);
        AddOrUpdateCardErrors AddCard(Card card, bool isMyCard, long userId, string notes, out long cardId);
        AddOrUpdateCardErrors EditCard(Card cardModel, bool isMyCard, long userId, string notes);
        void InvalidateBusidexCache();
        List<StateCode> GetAllStateCodes();
        List<Card> GetDuplicateCardsByEmail(long cardId, string email);
        void RelateCards(long ownerId, long relatedCardId);
        void UnRelateCards(long ownerId, long relatedCardId);
        void UpdateCardBasicInfo(long cardId, string name, string company, string phone, string email);
        void UpdateCardOrientation(long cardId, string frontOrientation, string backOrientation);
        void UpdateMobileView(long id, bool isMobileView);
        long GetCardCount();
        AddOrUpdateCardErrors CheckForCardModelErrors(Card cardModel, bool isMyCard);
        void AddCardToQueue(AddOrEditCardModel model);
        void SaveApplicationError(string error, string innerException, string stackTrace, long userId);
        void SaveApplicationError(Exception ex, long userId);
        Image ScaleImage(Image image, string orientation);
        void ClearBusidexCache();
        SearchResultModel SearchBySystemTag(string systag, long? userId);
        SearchResultModel SearchByGroupName(string groupName, long? userId);
        List<EventTag> GetEventTags();
        void UpdateUserCardStatus(long userCardId, UserCardAddStatus status);
        void AddSystemTagToCard(long cardid, string tag);
        OrgCardDetailModel GetOrganizationCardByOwnerId(long ownerId);
        void SendCardUpdatedEmails();
    }
}
