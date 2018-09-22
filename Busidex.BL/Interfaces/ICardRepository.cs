using System;
using System.Collections.Generic;
using Busidex.DAL;

namespace Busidex.BL.Interfaces
{
    public interface ICardRepository
    {
        AddOrEditCardModel GetAddOrEditModel(long cardId, BusidexUser bu, string action);
        AddOrEditCardModel GetAddOrEditModel(AddOrEditCardModel model);

        SearchResultModel Search(SearchResultModel model, long? userId, Tuple<double, double> altAddress);
        List<UserCard> GetMyBusidex(long userId, bool includeImages);
        List<UserGroupCard> GetBusiGroupCards(long groupId, bool includeImages = false);
        List<Group> GetMyBusiGroups(long userId);
        Group GetBusiGroupById(long groupId);
        long AddUserGroupCard(UserGroupCard groupCard);
        void AddUserGroupCards(string cardIds, long groupId, long userId);
        void RemoveUserGroupCards(string cardIds, long groupId, long userId);
        long AddGroup(Group group);
        long UpdateGroup(Group group);
        void DeleteGroup(long id);
        List<CardImage> SyncData(long id);
        //List<UserCard> GetMyBusidex(long userId, bool includeImages);
        void SaveGroupCardNotes(long id, string notes);
        //AddOrUpdateCardErrors AddOrUpdateCard(HttpContextBase ctx, bool edit, AddOrEditCardModel cardModel, long userId,
        //    string[] sCategories, string[] cardRelations, List<UserCard> myBusidex, out long cardId);

        //void UpdateLizzidex(int coffee, int thing);
        CardDetailModel GetCardDetail(Card card);
        BusidexUser GetBusidexUserById(long userId);
        bool SaveCardOwnerToken(long cardId, Guid token);
        bool SaveCardOwner(long cardId, long ownerId);
        CardDetailModel GetCardByToken(string token);
        Card GetCardById(long cardId);
        List<CardDetailModel> GetCardsByOwnerId(long ownerId);
        void DeleteCard(long id, long userId);
        void SaveCardNotes(long id, string notes);
        UserCard GetUserCard(long id, long userId);
        void DeleteUserCard(UserCard uc, long userId);
        void AddToMyBusidex(long cardId, long userId);
        void AddSendersCardToMyBusidex(string token, long userId);
        void SaveSharedCards(List<SharedCard> sharedCards);
        List<SharedCard> GetSharedCards(long userId);
        void AcceptSharedCard(long cardId, long userId);
        void DeclineSharedCard(long cardId, long userId);
        void UpdateCardFileId(long cardId, Guid frontFileId, Guid backFileId);
        List<Card> GetAllCards();
        List<Card> GetUnownedCards();
        bool IsACardOwner(long ownerId);
        AddOrUpdateCardErrors AddCard(Card card, bool isMyCard, long userId, string notes, out long cardId);
        AddOrUpdateCardErrors EditCard(Card cardModel, bool isMyCard, long userId, string notes);
        void InvalidateBusidexCache();
        List<StateCode> GetAllStateCodes();
        List<Card> GetDuplicateCardsByEmail(long cardId, string email);
        void RelateCards(long ownerId, long relatedCardId);
        void UnRelateCards(long ownerId, long relatedCardId);
        void UpdateCardBasicInfo(long cardId, string name, string company, string phone, string email);
        void UpdateMobileView(long id, bool isMobileView);
    }
}
