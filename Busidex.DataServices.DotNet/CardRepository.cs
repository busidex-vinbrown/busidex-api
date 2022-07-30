using Busidex.DomainModels.DotNet.DTO;
using Newtonsoft.Json;
using System.Drawing;
using System.Text;

namespace Busidex.DataServices.DotNet
{
    public class CardRepository : RepositoryBase, ICardRepository
    {
        public CardRepository(string connectionString = "")
            : base(connectionString)
        {
        }

        //public async Task AcceptSharedCard(long cardId, long userId)
        //{
        //    await _dao.AcceptSharedCard(cardId, userId);
        //}

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
                await _dao.AddTag(cardId, tag);
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

        public async Task AddEventActivity(EventActivity activity)
        {
            await _dao.AddEventActivity(activity);
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

        public async Task CardToFile(long cardId, bool replaceFront, bool replaceBack, byte[] frontImage, Guid frontFileId, string frontType, byte[] backImage, Guid backFileId, string backType, long userId)
        {
            var mimeTypes = new Dictionary<string, string>
            {
                {"jpg", "image/jpeg"},
                {"jpeg", "image/jpeg"},
                {"png", "image/x-png"},
                {"gif", "image/gif"},
                {"bmp", "image/bmp"}
            };

            const long DEMO_CARD_ID = 1;

            if (cardId <= DEMO_CARD_ID)
            {
                return;
            }

            try
            {
                var card = await GetCardById(cardId, userId);
                //var defaultCard = GetCardById(DEMO_CARD_ID, userId);

                if (card == null) return;

                #region Update file name in DB

                card.FrontFileId = frontFileId;// card.FrontFileId ?? Guid.NewGuid();
                card.BackFileId = backFileId;// card.BackFileId ?? Guid.NewGuid();

                await UpdateCardFileId(cardId, card.FrontFileId.GetValueOrDefault(), frontType, card.BackFileId.GetValueOrDefault(), backType);

                #endregion

                #region Save card to file system

                var connStr = Environment.GetEnvironmentVariable("CardImageStorageConnection");
                var container = Environment.GetEnvironmentVariable("CardImageBlobStorageContainer");
                var containerClient = new Azure.Storage.Blobs.BlobContainerClient(connStr, container);
                
                if (replaceFront && frontImage != null)
                {
                    var uniqueFrontBlobName = $"{card.FrontFileId}.{frontType}".ToLower();
                    var blobClient = containerClient.GetBlobClient(uniqueFrontBlobName);                    
                    bool deleted = await blobClient.DeleteIfExistsAsync();                    
                    await blobClient.UploadAsync(new MemoryStream(frontImage.ToArray()));
                    await SaveThumbnail(frontImage.ToArray(), card.FrontOrientation, card.FrontFileId.GetValueOrDefault().ToString(), frontType);
                }

                if (replaceBack && backImage != null && backImage != null)
                {
                    var uniqueBackBlobName = $"{card.BackFileId}.{backType}".ToLower();
                    var blobClient = containerClient.GetBlobClient(uniqueBackBlobName);
                    bool deleted = await blobClient.DeleteIfExistsAsync();
                    await blobClient.UploadAsync(new MemoryStream(backImage.ToArray()));
                    await SaveThumbnail(backImage.ToArray(), card.BackOrientation, card.BackFileId.GetValueOrDefault().ToString(), backType);
                }

                #endregion
            }
            catch (Exception ex)
            {
                await SaveApplicationError(ex.Message, ex.InnerException?.Message ?? string.Empty, ex.StackTrace, 0);
            }
        }

        private async Task SaveThumbnail(byte[] cardImage, string orientation, string fileName, string fileType)
        {
            var connStr = Environment.GetEnvironmentVariable("CardImageStorageConnection");
            var container = Environment.GetEnvironmentVariable("mobile-images");
            var containerClient = new Azure.Storage.Blobs.BlobContainerClient(connStr, container);
            string uniqueBlobName = $"{fileName}.{fileType}";
            var blobClient = containerClient.GetBlobClient(uniqueBlobName);

            Image img = new Bitmap(new MemoryStream(cardImage));
            var scaledImage = ScaleImage(img, orientation);

            using (var memoryStream = new MemoryStream())
            {
                scaledImage.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);

                memoryStream.Seek(0, SeekOrigin.Begin);
                await blobClient.UploadAsync(memoryStream);
            }
        }

        public async Task<List<EventSource>> GetAllEventSources()
        {
            return await _dao.GetAllEventSources();
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

        public async Task<AddOrUpdateCardErrors> EditCard(Card cardModel, bool isMyCard, long userId, string notes)
        {
            var modelErrors = new AddOrUpdateCardErrors();
            #region Phone Numbers

            var newPhoneNumbers = new List<PhoneNumber>();
            newPhoneNumbers.AddRange(cardModel.PhoneNumbers?.Where(p => p.PhoneNumberId == 0));

            foreach (var existingPhoneNumber in cardModel.PhoneNumbers.Where(p => p.PhoneNumberId > 0))
            {
                var phoneNumber = await _dao.GetPhoneNumberById(existingPhoneNumber.PhoneNumberId);

                if (phoneNumber == null) continue;

                phoneNumber.Number = existingPhoneNumber.Number;
                phoneNumber.Extension = existingPhoneNumber.Extension;
                phoneNumber.PhoneNumberTypeId = existingPhoneNumber.PhoneNumberTypeId;
                phoneNumber.Deleted = existingPhoneNumber.Deleted;
                phoneNumber.Updated = DateTime.UtcNow;

                await _dao.UpdatePhoneNumber(phoneNumber);
            }

            // Add the new phone numbers
            foreach (var phoneNumber in newPhoneNumbers)
            {
                phoneNumber.CardId = cardModel.CardId;
                await _dao.AddPhoneNumber(phoneNumber);
            }

            //var existingNumbers = _dao.GetCardPhoneNumbers(cardModel.CardId.ToString());
            //foreach (var number in existingNumbers)
            //{
            //    if (cardModel.PhoneNumbers.All(
            //        p =>
            //            !p.Number.Equals(number.Number) && !p.Extension.Equals(number.Extension) &&
            //            p.PhoneNumberTypeId != number.PhoneNumberTypeId))
            //    {
            //        _dao.DeletePhoneNumber(number.PhoneNumberId);
            //    }

            //}
            #endregion

            #region Tags

            var tags = await _dao.GetCardTags(cardModel.CardId);
            foreach (var tag in tags)
            {
                // if it's not found, it's been removed
                if (cardModel.Tags.All(t => !string.Equals(t.Text, tag.Text, StringComparison.CurrentCultureIgnoreCase)))
                {
                    await _dao.DeleteTag(cardModel.CardId, tag.TagId);
                }
            }

            foreach (var tag in cardModel.Tags)
            {
                if (tag.TagId == 0)
                {
                    await _dao.AddTag(cardModel.CardId, tag);
                }
            }


            #endregion

            #region Addresses
            foreach (var address in cardModel.Addresses)
            {
                if (address.CardAddressId == 0 && address.ToString().Trim() != ",")
                {
                    await _dao.AddAddress(cardModel.CardId, address);
                }
                else
                {
                    // update or delete the address
                    if (address.CardAddressId > 0)
                    {
                        if (address.Deleted)
                        {
                            await _dao.DeleteAddress(address.CardAddressId);
                        }
                        else
                        {
                            await _dao.UpdateAddress(address);
                        }
                    }
                }
            }
            #endregion

            #region Update card owner and Searchable

            if (isMyCard)
            {
                cardModel.OwnerId = userId;
                cardModel.Searchable = true;
            }
            else
            {
                cardModel.OwnerId = null;
                cardModel.Searchable = false;
            }

            #endregion

            // Update the card
            await _dao.UpdateCard(cardModel);

            // Update card notes
            var uc = await _dao.GetUserCard(cardModel.CardId, userId);
            if (uc != null)
            {
                await _dao.UpdateUserCard(uc.UserCardId, notes);
            }

            return modelErrors;
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

        public async Task<Card?> GetCardById(long cardId, long userId = 0)
        {
            return await _dao.GetCardById(cardId, userId);
        }

        public CardDetailModel GetCardByToken(string token)
        {
            throw new NotImplementedException();
        }

        public long GetCardCount()
        {
            throw new NotImplementedException();
        }

        public CardDetailModel GetCardDetail(Card card)
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

        public async Task<List<Communication>> GetCommunications(string[] emails, long userId)
        {
            return await _dao.GetCommunications(emails, userId);
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

        public async Task SaveCardOwner(long cardId, long ownerId)
        {
            await _dao.SaveCardOwner(cardId, ownerId);
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

        public Image? ScaleImage(Image image, string orientation)
        {
            if (image == null) return null; ;

            int maxHeight = orientation == "H" ? 250 : 340;
            int maxWidth = orientation == "H" ? 400 : 232;

            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            var newImage = new Bitmap(newWidth, newHeight);
            Graphics.FromImage(newImage).DrawImage(image, 0, 0, newWidth, newHeight);

            return newImage;
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

        public async Task SendOwnerNotificationOfSharedCard(SharedCard model)
        {
            var card = await GetCardById(model.CardId);
            var template = await _dao.GetEmailTemplate(EmailTemplateCode.SharedCardOwner);
            var account = await _dao.GetUserAccountByUserId(model.SendFrom);
            var ownerAccount = await _dao.GetUserAccountByUserId(card.OwnerId.GetValueOrDefault());
            if (ownerAccount != null && model.SendFrom != card.OwnerId)
            {
                template.Populate(model, account, card);

                var communication = new Communication
                {
                    DateSent = DateTime.UtcNow,
                    Email = ownerAccount.BusidexUser.Email,
                    EmailTemplate = template,
                    EmailTemplateId = template.EmailTemplateId,
                    SentById = model.SendFrom,
                    OwnerToken = null,
                    Failed = false,
                    UserId = 0
                };
                try
                {
                    await SendEmail(communication);
                }
                catch (Exception ex)
                {
                    await SaveApplicationError(ex, 0);
                    communication.Failed = true;
                }
                finally
                {
                    await _dao.SaveCommunication(communication);
                }
            }
        }

        public async Task SendSharedCard(SharedCard model)
        {
            var card = await GetCardById(model.CardId);

            if (card != null)
            {
                var template = await _dao.GetEmailTemplate(EmailTemplateCode.SharedCard);
                var account = await _dao.GetUserAccountByUserId(model.SendFrom);
                var communication = GetCommunication(model, card, account, template);

                try
                {
                    await SendEmail(communication);

                    if (model.SendFrom != card.OwnerId.GetValueOrDefault())
                    {
                        await SendOwnerNotificationOfSharedCard(model);
                    }
                }
                catch (Exception ex)
                {
                    await SaveApplicationError(ex, 0);
                    communication.Failed = true;
                }
                finally
                {
                    await _dao.SaveCommunication(communication);
                }
            }
        }

        public async Task SendSharedCardInvitation(SharedCard model)
        {
            var card = await GetCardById(model.CardId);

            if (card != null)
            {
                var template = await _dao.GetEmailTemplate(EmailTemplateCode.Invitation);
                var account = await _dao.GetUserAccountByUserId(model.SendFrom);
                var communication = GetCommunication(model, card, account, template);

                try
                {
                    await SendEmail(communication);

                    if (model.SendFrom != card.OwnerId.GetValueOrDefault())
                    {
                        await SendOwnerNotificationOfSharedCard(model);
                    }
                }
                catch (Exception ex)
                {
                    await SaveApplicationError(ex, 0);
                    communication.Failed = true;
                }
                finally
                {
                    await _dao.SaveCommunication(communication);
                }
            }
        }

        private static Communication GetCommunication(SharedCard model, Card card, UserAccount account, EmailTemplate template)
        {
            template.Populate(model, account, card);

            var communication = new Communication
            {
                EmailTemplate = template,
                Email = model.Email,
                Body = template.Body,
                DateSent = DateTime.UtcNow,
                UserId = 0,
                SentById = card.OwnerId.GetValueOrDefault(),
                OwnerToken = null,
                Failed = false,
                EmailTemplateId = template.EmailTemplateId
            };
            return communication;
        }

        private async Task SendEmail(Communication communication)
        {
            try
            {
                const string QUEUE_NAME = "email";
                var connectionString = Environment.GetEnvironmentVariable("BusidexQueuesConnectionString");
                var queueClient = new Azure.Storage.Queues.QueueClient(connectionString, QUEUE_NAME);
                queueClient.CreateIfNotExists();
                var json = JsonConvert.SerializeObject(communication);
                var msg = Convert.ToBase64String(ASCIIEncoding.UTF8.GetBytes(json));
                await queueClient.SendMessageAsync(msg);
            }
            catch (Exception ex)
            {
                await SaveApplicationError(ex, 0);
            }
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

        public async Task UpdateCardFileId(long cardId, Guid frontFileId, string frontType, Guid backFileId, string backType)
        {
            await _dao.UpdateCardFileId(cardId, frontFileId, frontType, backFileId, backType);
        }

        public async Task UpdateCardLinks(long cardId, List<ExternalLink> links)
        {
            //await _dao.UpdateCardLinks(cardId, links);
            throw new NotImplementedException();
        }

        public async Task UpdateCardOrientation(long cardId, string frontOrientation, string backOrientation)
        {
            await _dao.UpdateCardOrientation(cardId, frontOrientation, backOrientation);
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

        BusidexUser ICardRepository.GetBusidexUserById(long userId)
        {
            throw new NotImplementedException();
        }
    }
}

