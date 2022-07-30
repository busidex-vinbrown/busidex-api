using Busidex.DataAccess;
using Busidex.DomainModels;
using Busidex.DomainModels.Standard.DTO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using Card = Busidex.DomainModels.Standard.DTO.Card;

namespace Busidex.DataServices
{
    public class CardRepository : RepositoryBase, ICardRepository
    {
        public CardRepository(IBusidexDataContext busidexDal, string connectionString = "")
            : base(busidexDal, connectionString)
        {
        }

        public async Task AcceptSharedCard(long cardId, long userId)
        {
            await _dao.AcceptSharedCard(cardId, userId);
        }

        public async Task<long> AddCard(Card card, bool isMyCard, long userId, string notes)
        {
            if (isMyCard)
            {
                card.OwnerId = userId;
                card.Searchable = true;
            }

            // Add the new card
            var cardId = await _dao.AddCard(card);
            card.CardId = cardId;


            // Add phone numbers
            foreach (var phoneNumber in card.PhoneNumbers)
            {
                phoneNumber.CardId = cardId;
                await _dao.AddPhoneNumber(phoneNumber);
            }

            // Add Tags
            foreach (var tag in card.Tags)
            {
                //BusidexDAL.AddTag(cardId, tag);
                _dao.AddTag(cardId, tag);
            }

            // Add Addresses
            foreach (var address in card.Addresses)
            {
                if (!address.Deleted)
                {
                    await _dao.AddAddress(cardId, address);
                }
            }

            // Add the card to MyBusidex
            var userCard = new UserCard(card, userId) { CardId = cardId, Created = DateTime.Now, Notes = notes };
            await _dao.AddUserCard(userCard);

            return cardId;
        }

        public void AddCardToQueue(string connectionString, string cardUpdateRef)
        {
            const string QUEUE_NAME = "card-update";
            Azure.Storage.Queues.QueueClient queueClient = new Azure.Storage.Queues.QueueClient(connectionString, QUEUE_NAME);
            queueClient.CreateIfNotExists();

            var bytes = ASCIIEncoding.ASCII.GetBytes(cardUpdateRef);
            queueClient.SendMessage(System.Convert.ToBase64String(bytes));
        }

        public void AddGroup(Group group, string cardIds)
        {
            throw new NotImplementedException();
        }

        public void AddGroupCards(long groupId, string cardIds)
        {
            throw new NotImplementedException();
        }

        public Task AddSendersCardToMyBusidex(string token, long userId)
        {
            throw new NotImplementedException();
        }

        public Task AddSharedCardsToQueue(string connectionString, List<SharedCard> sharedCardList)
        {
            throw new NotImplementedException();
        }

        public Task AddSharedCardToQueue(string connectionString, SharedCard sharedCard)
        {
            throw new NotImplementedException();
        }

        public void AddSystemTagToCard(long cardid, string tag)
        {
            throw new NotImplementedException();
        }

        public Task AddToMyBusidex(long cardId, long userId)
        {
            throw new NotImplementedException();
        }

        public long AddUserGroupCard(UserGroupCard groupCard)
        {
            throw new NotImplementedException();
        }

        public void AddUserGroupCards(string cardIds, long groupId, long userId)
        {
            throw new NotImplementedException();
        }

        public Task CardToFile(long cardId, bool replaceFront, bool replaceBack, byte[] frontImage, Guid frontFileId, string frontType, byte[] backImage, Guid backFileId, string backType, long userId)
        {
            throw new NotImplementedException();
        }

        public AddOrUpdateCardErrors CheckForCardModelErrors(Card cardModel, bool isMyCard)
        {
            throw new NotImplementedException();
        }

        public void ClearBusidexCache()
        {
            throw new NotImplementedException();
        }

        public void DeclineSharedCard(long cardId, long userId)
        {
            throw new NotImplementedException();
        }

        public void DeleteCard(long id, long userId)
        {
            throw new NotImplementedException();
        }

        public void DeleteGroup(long id)
        {
            throw new NotImplementedException();
        }

        public void DeleteUserCard(UserCard uc, long userId)
        {
            throw new NotImplementedException();
        }

        public Task<AddOrUpdateCardErrors> EditCard(Card cardModel, bool isMyCard, long userId, string notes)
        {
            throw new NotImplementedException();
        }

        public List<SharedCard> Get30DaySharedCards()
        {
            throw new NotImplementedException();
        }

        public AddOrEditCardModel GetAddOrEditModel(long cardId, BusidexUser bu, string action)
        {
            throw new NotImplementedException();
        }

        public AddOrEditCardModel GetAddOrEditModel(AddOrEditCardModel model)
        {
            throw new NotImplementedException();
        }

        public List<Card> GetAllCards()
        {
            throw new NotImplementedException();
        }

        public List<StateCode> GetAllStateCodes()
        {
            throw new NotImplementedException();
        }

        public Group GetBusiGroupById(long groupId)
        {
            throw new NotImplementedException();
        }

        public List<GroupCard> GetBusiGroupCards(long groupId, bool includeImages = false)
        {
            throw new NotImplementedException();
        }

        public CardDetailModel GetCardByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public Card GetCardById(long cardId, long userId = 0)
        {
            throw new NotImplementedException();
        }

        public CardDetailModel GetCardByToken(string token)
        {
            throw new NotImplementedException();
        }

        public long GetCardCount()
        {
            throw new NotImplementedException();
        }

        public CardDetailModel GetCardDetail(DomainModels.DTO.Card card)
        {
            throw new NotImplementedException();
        }

        public List<CardDetailModel> GetCardsByOwnerId(long ownerId)
        {
            throw new NotImplementedException();
        }

        public List<CardDetailModel> GetCardsByPhoneNumber(long userId, string phoneNumber)
        {
            throw new NotImplementedException();
        }

        public List<Communication> GetCommunications(string[] emails, long userId)
        {
            throw new NotImplementedException();
        }

        public List<Card> GetDuplicateCardsByEmail(long cardId, string email)
        {
            throw new NotImplementedException();
        }

        public List<EventTag> GetEventTags()
        {
            throw new NotImplementedException();
        }

        public CardDetailModel GetFeaturedCard()
        {
            throw new NotImplementedException();
        }

        public Group GetGroupByName(string groupName)
        {
            throw new NotImplementedException();
        }

        public List<UserCard> GetMyBusidex(long userId, bool includeImages)
        {
            throw new NotImplementedException();
        }

        public List<Group> GetMyBusiGroups(long userId)
        {
            throw new NotImplementedException();
        }

        public OrgCardDetailModel GetOrganizationCardByOwnerId(long ownerId)
        {
            throw new NotImplementedException();
        }

        public List<SEOCardResult> GetSeoCardResult()
        {
            throw new NotImplementedException();
        }

        public SharedCard GetSharedCard(long cardId, long sendFrom, long shareWith)
        {
            throw new NotImplementedException();
        }

        public EmailTemplate GetSharedCardEmailPreview(SharedCard model)
        {
            throw new NotImplementedException();
        }

        public List<SharedCard> GetSharedCards(long userId)
        {
            throw new NotImplementedException();
        }

        public List<UnownedCard> GetUnownedCards()
        {
            throw new NotImplementedException();
        }

        public UserCard GetUserCard(long id, long userId)
        {
            throw new NotImplementedException();
        }

        public UserCard GetUserCardLite(long id, long userId)
        {
            throw new NotImplementedException();
        }

        public void InvalidateBusidexCache()
        {
            throw new NotImplementedException();
        }

        public bool IsACardOwner(long ownerId)
        {
            throw new NotImplementedException();
        }

        public void RelateCards(long ownerId, long relatedCardId)
        {
            throw new NotImplementedException();
        }

        public void RemoveGroupCards(long groupId, string cardIds, long userId)
        {
            throw new NotImplementedException();
        }

        public void SaveCardNotes(long id, string notes)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SaveCardOwner(long cardId, long ownerId)
        {
            throw new NotImplementedException();
        }

        public bool SaveCardOwnerToken(long cardId, Guid token)
        {
            throw new NotImplementedException();
        }

        public void SaveGroupCardNotes(long id, string notes)
        {
            throw new NotImplementedException();
        }

        public void SaveSharedCard(SharedCard sharedCard)
        {
            throw new NotImplementedException();
        }

        public void SaveSharedCards(List<SharedCard> sharedCards)
        {
            throw new NotImplementedException();
        }

        public Image ScaleImage(Image image, string orientation)
        {
            throw new NotImplementedException();
        }

        public SearchResultModel Search(SearchResultModel model, long? userId, Tuple<double, double> altAddress)
        {
            throw new NotImplementedException();
        }

        public SearchResultModel SearchByGroupName(string groupName, long? userId)
        {
            throw new NotImplementedException();
        }

        public SearchResultModel SearchBySystemTag(string systag, long? userId)
        {
            throw new NotImplementedException();
        }

        public Task SendCardUpdatedEmails()
        {
            throw new NotImplementedException();
        }

        public Task SendOwnerNotificationOfSharedCard(SharedCard model)
        {
            throw new NotImplementedException();
        }

        public Task SendSharedCard(SharedCard model)
        {
            throw new NotImplementedException();
        }

        public Task SendSharedCardInvitation(SharedCard model)
        {
            throw new NotImplementedException();
        }

        public List<CardImage> SyncData(long id)
        {
            throw new NotImplementedException();
        }

        public void UnRelateCards(long ownerId, long relatedCardId)
        {
            throw new NotImplementedException();
        }

        public void UpdateCardBasicInfo(long cardId, string name, string company, string phone, string email)
        {
            throw new NotImplementedException();
        }

        public Task UpdateCardFileId(long cardId, Guid frontFileId, string frontType, Guid backFileId, string backType)
        {
            throw new NotImplementedException();
        }

        public void UpdateCardLinks(long cardId, List<DomainModels.DTO.ExternalLink> links)
        {
            throw new NotImplementedException();
        }

        public Task UpdateCardOrientation(long cardId, string frontOrientation, string backOrientation)
        {
            throw new NotImplementedException();
        }

        public long UpdateGroup(Group group)
        {
            throw new NotImplementedException();
        }

        public void UpdateMobileView(long id, bool isMobileView)
        {
            throw new NotImplementedException();
        }

        public void UpdateUserCardStatus(long userCardId, UserCardAddStatus status)
        {
            throw new NotImplementedException();
        }

        public Task UploadCardUpdateToBlobStorage(AddOrEditCardModel model, string cardUpdateStorageConnectionString, string cardRef)
        {
            throw new NotImplementedException();
        }
    }
}

