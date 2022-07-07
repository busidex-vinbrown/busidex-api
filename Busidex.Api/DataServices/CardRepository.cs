using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using Busidex.Api.DataAccess;
using Busidex.Api.DataAccess.DTO;
using Busidex.Api.DataServices.Interfaces;
using Busidex.Api.Models;
using Newtonsoft.Json;
using Group = Busidex.Api.DataAccess.DTO.Group;
using System.IO.Compression;
using System.Text;
using System.Xml.Serialization;
using CloudStorageAccount = Microsoft.WindowsAzure.Storage.CloudStorageAccount;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Threading.Tasks;

namespace Busidex.Api.DataServices
{
    public class CardRepository : RepositoryBase, ICardRepository
    {
        public CardRepository(IBusidexDataContext busidexDal, string connectionString = "")
            : base(busidexDal, connectionString)
        {
        }

        public List<Communication> GetCommunications(string[] emails, long userId)
        {
            return _dao.GetCommunications(emails, userId);   
        } 

        public CardDetailModel GetFeaturedCard()
        {
            return _dao.GetFeaturedCard();
        }

        public void DeleteUserCard(UserCard uc, long userId)
        {
            if (uc != null)
            {
                BusidexDAL.DeleteUserCard(uc, userId);
            }
        }

        public async Task AddToMyBusidex(long id, long userId)
        {
            var userCard = new UserCard(id, userId);
            await _dao.AddUserCard(userCard);

            // Send an email notification to the card owner
            return;

            //var template = _dao.GetEmailTemplate(EmailTemplateCode.CardAdded);
            //var card = GetCardById(id);
            //var account = _dao.GetUserAccountByUserId(userId);
            //if (account != null)
            //{
            //    template.Populate(card, account);

            //    var sendTo = _dao.GetUserAccountByUserId(card.OwnerId.GetValueOrDefault());
            //    var communication = new Communication
            //    {
            //        EmailTemplate = template,
            //        EmailTemplateId = template.EmailTemplateId,
            //        Email = sendTo.BusidexUser.Email,
            //        Body = template.Body,
            //        DateSent = DateTime.Now,
            //        SentById = userId,
            //        Failed = false,
            //        UserId = 0
            //    };

            //    SendEmail(communication);
            //    _dao.SaveCommunication(communication);

            //}

        }

        public async Task AddSendersCardToMyBusidex(string token, long userId)
        {
            Communication communication = BusidexDAL.GetCommunicationByActivationToken(token);
            if (communication != null)
            {
                List<CardDetailModel> card = BusidexDAL.GetCardsByOwnerId(communication.SentById);
                if (card != null)
                {
                    foreach (var c in card)
                    {
                        var userCard = new UserCard(c.CardId, userId);
                        await _dao.AddUserCard(userCard);
                    }
                }
            }
        }

        public UserCard GetUserCardLite(long id, long userId)
        {
            return BusidexDAL.GetUserCard(id, userId);
        }

        public UserCard GetUserCard(long id, long userId)
        {
            var userCard = _dao.GetUserCard(id, userId);
            if (userCard == null)
            {
                userCard = new UserCard
                {
                    Card = new DataAccess.DTO.Card()
                };
            }
            else
            {
                userCard.Card = _dao.GetCardById(userCard.CardId);
            }
            return userCard;

        }

        public void UpdateUserCardStatus(long userCardId, UserCardAddStatus status)
        {
            _dao.UpdateUserCardStatus(userCardId, status);
        }

        public CardDetailModel GetCardDetail(DataAccess.DTO.Card card)
        {
            return new CardDetailModel(card);
        }

        public void SaveCardNotes(long id, string notes)
        {
            BusidexDAL.UpdateUserCard(id, notes);
        }

        public void SaveGroupCardNotes(long id, string notes)
        {
            BusidexDAL.UpdateUserGroupCard(id, notes);
        }

        public DataAccess.DTO.Card GetCardById(long cardId, long userId = 0)
        {
            return _dao.GetCardById(cardId, userId);
        }

        public AddOrEditCardModel GetAddOrEditModel(AddOrEditCardModel model)
        {
			DataAccess.DTO.Card card = _dao.GetCardById(model.CardId);

            model.PhoneNumbers = new List<PhoneNumber>();
            model.PhoneNumbers.AddRange(card.PhoneNumbers.Where(pn => pn.Deleted == false));
            model.PhoneNumberTypes = BusidexDAL.GetAllPhoneNumberTypes();
            model.Addresses = card.Addresses ?? new List<DataAccess.DTO.CardAddress>();
            model.Tags = card.Tags ?? new List<Tag>();

            return model;
        }

        public AddOrEditCardModel GetAddOrEditModel(long cardId, BusidexUser bu, string action)
        {
            var card = cardId == 1 ? new DataAccess.DTO.Card() : _dao.GetCardById(cardId, bu.UserId);
            var appSettings = new AppSettingsReader();
            card = card ?? new DataAccess.DTO.Card();
            if (card.Addresses.Count == 0)
            {
                card.Addresses.Add(new DataAccess.DTO.CardAddress());
            }
            var uc = GetUserCard(card.CardId, bu.UserId);
            var links = _dao.GetExternalLinks(card.CardId);
            var model = new AddOrEditCardModel
                            {
                                PhoneNumberTypes = new List<PhoneNumberType>(),
                                FileSizeLimit = Convert.ToString(appSettings.GetValue("FileSizeLimit", typeof(string))),
                                FileSizeInfoContent = string.Empty,
                                MyBusidex = new List<UserCard>(),
                                ActionMethod = action,
                                BackImage = card.BackImage,
                                BackOrientation = card.BackOrientation,
                                BackType = card.BackType?.Replace(".", "") ?? string.Empty,
                                BusinessId = card.BusinessId,
                                Email = card.Email,
                                Name = card.Name,
                                Title = card.Title,
                                Url = card.Url,
                                CompanyName = card.CompanyName,
                                CardId = card.CardId,
                                Created = card.Created,
                                CreatedBy = card.CreatedBy ?? new long(),
                                Updated = card.Updated,
                                OwnerId = card.OwnerId,
                                IsMyCard = card.OwnerId == bu.UserId,
                                OwnerToken = card.OwnerToken,
                                FrontImage = card.FrontImage,
                                FrontOrientation = card.FrontOrientation,
                                FrontType = card.FrontType?.Replace(".", "") ?? string.Empty,
                                PhoneNumbers = card.PhoneNumbers.Where(pn => pn.Deleted == false).ToList(),
                                HasFrontImage = card.FrontImage != null && card.FrontImage.Length > 0 && cardId > 1,
                                HasBackImage = card.BackImage != null && card.BackImage.Length > 0 && cardId > 1 && card.BackFileId != Guid.Empty && card.BackFileId.HasValue,
                                FrontFileId = card.FrontFileId,
                                BackFileId = card.BackFileId,
                                Tags = card.Tags ?? new List<Tag>(),
                                Addresses = card.Addresses,
                                ExternalLinks = links,
                                Markup = card.Markup,
                                Visibility = card.Visibility,
                                Display = card.Display,
                                Notes = uc != null ? uc.Notes : string.Empty,
                                CardType = CardType.Professional
                            };
            if (card.OwnerId.HasValue)
            {
                model.IsMyCard = card.OwnerId == bu.UserId;
            } 

            if (action == "Edit")
            {
                model.IsMyCard = card.OwnerId == bu.UserId;
            }

            //if (model.PhoneNumbers.Count == 0)
            //{
            //    model.PhoneNumbers.Add(new PhoneNumber
            //    {
            //        Number = string.Empty,
            //        PhoneNumberTypeId = 1,
            //        Extension = string.Empty
            //    });
            //}

            return model;
        }

        public List<DataAccess.DTO.Card> GetDuplicateCardsByEmail(long cardId, string email)
        {
            return BusidexDAL.GetDuplicateCardsByEmail(cardId, email);
        }

        public List<EventTag> GetEventTags()
        {
            return _dao.GetEventTags();
        } 

        public SearchResultModel SearchBySystemTag(string systag, long? userId)
        {
            var searchModel = new SearchResultModel();

            var cards = _dao.SearchBySystemTag(systag, userId);
            searchModel.Results = cards.Select(c => new CardDetailModel(c)).ToList();

            return searchModel;
        }

        public SearchResultModel SearchByGroupName(string groupName, long? userId)
        {
            var searchModel = new SearchResultModel();

            var cards = _dao.SearchByGroupName(groupName, userId);
            searchModel.Results = cards.Select(c => new CardDetailModel(c)).ToList();

            return searchModel;
        }

        public SearchResultModel Search(SearchResultModel model, long? userId, Tuple<double, double> altAddress)
        {

            var searchModel = new SearchResultModel();

            var user = _dao.GetBusidexUserById(userId.GetValueOrDefault());
            
            double? latitude = null, longitude = null;

            // use the alternate address or the address associated with the current user.
            if (altAddress != null)
            {
                latitude = altAddress.Item1;
                longitude = altAddress.Item2;
            }
            else if (user?.Address != null)
            {
                latitude = user.Address.Latitude;
                longitude = user.Address.Longitude;
            }
            if (latitude == 0) latitude = null;
            if (longitude == 0) longitude = null;
            if (model.Distance == 0)
            {
                latitude = longitude = null;
            }
            var cards = new List<DataAccess.DTO.Card>();
            string s = model.SearchText;
            IEqualityComparer<DataAccess.DTO.Card> comparer = new CardComparer();
            if (!string.IsNullOrEmpty(s) && !string.IsNullOrWhiteSpace(s))
            {
                IEnumerable<string> searchCriteria = ExtractParameters(s).ToList();
                bool searchableOnly = userId.GetValueOrDefault() == 0;
                cards.AddRange(
                    _dao.SearchCards(string.Join(",", searchCriteria), latitude, longitude, model.Distance, searchableOnly, model.CardType, userId)
                );
            }

            //set up other model attributes
            searchModel.Criteria = s;
            searchModel.Distance = model.Distance;
            searchModel.SearchLocation = model.SearchLocation;
            searchModel.SearchAddress = model.SearchAddress;

            var distinctCards = cards.Where(c => c.Searchable).Distinct(comparer);

            var filteredCards = FilterSearchResults(distinctCards.ToList(), new List<UserCard>(), userId.GetValueOrDefault());

            searchModel.Results = filteredCards.Select(c => new CardDetailModel(c)).ToList();

            searchModel.HasResults = cards.Any();
            searchModel.Display = ViewType.List;

            return searchModel;
        }

        private IEnumerable<DataAccess.DTO.Card> FilterSearchResults(List<DataAccess.DTO.Card> cards, IEnumerable<UserCard> mybusidex, long userId)
        {
            var results = new List<DataAccess.DTO.Card>();
            const short PUBLIC = 1;
            const short SEMI_PUBLIC = 2;
            const short PRIVATE = 3;

            // Add all the public cards
            results.AddRange(cards.Where(c => c.Visibility == PUBLIC || c.OwnerId == userId));
            
            if (userId > 0)
            {
                // Add any Semi-Private cards that are allowed
                results.AddRange(cards.Where(c => c.Visibility == SEMI_PUBLIC && (mybusidex.Any(uc=>uc.SharedById.HasValue && uc.CardId == c.CardId))));

                // Add any Private cards that are allowed
                results.AddRange(cards.Where(c => c.Visibility == PRIVATE && (mybusidex.Any(uc => uc.SharedById == c.OwnerId) )));
            }
            return results;
        }

        private IEnumerable<string> ExtractParameters(string s)
        {
            var parameters = new List<string>();
            if (Regex.Matches(s, "\"").Count == 2)
            {

            }
            else
            {
                parameters.AddRange(s.Split(' ').Where(ss => ss.Length >= 3));
            }

            return parameters.ToArray();
        }

        public void RelateCards(long ownerId, long relatedCardId)
        {
            BusidexDAL.RelateCards(ownerId, relatedCardId);
            InvalidateBusidexCache();
        }

        public void UnRelateCards(long ownerId, long relatedCardId)
        {
            BusidexDAL.UnRelateCards(ownerId, relatedCardId);
            InvalidateBusidexCache();
        }

        public List<CardImage> SyncData(long id)
        {
            return BusidexDAL.SyncData(id);
        }

        public List<UserCard> GetMyBusidex(long userId, bool includeImages = false)
        {
            var myBusidex = _dao.GetMyBusidex(userId, includeImages); 

            return myBusidex;
        }

        public List<GroupCard> GetBusiGroupCards(long groupId, bool includeImages = false)
        {
            return _dao.GetGroupCards(groupId);//.GetBusiGroupCards(groupId, includeImages);
        }

        public Group GetBusiGroupById(long groupId)
        {
            return _dao.GetGroupById(groupId);
        }

        public Group GetGroupByName(string groupName)
        {
            return _dao.GetGroupByName(groupName);
        }

        public List<Group> GetMyBusiGroups(long userId)
        {
            const int GROUP_TYPE_PERSONAL = 1;
            var myBusgroups = _dao.GetGroupsByOwnerId(userId, GROUP_TYPE_PERSONAL); 

            return myBusgroups;
        }

        public void AddGroup(Group group, string cardIds)
        {
            _dao.AddGroup(group, cardIds);
        }

        public void DeleteGroup(long id)
        {
            _dao.DeleteGroup(id);
        }

        public long UpdateGroup(Group group)
        {
            return BusidexDAL.UpdateGroup(group);
        }

        public long AddUserGroupCard(UserGroupCard groupCard)
        {
            return BusidexDAL.AddUserGroupCard(groupCard);
        }

        public void AddGroupCards(long groupId, string cardIds)
        {
            _dao.AddGroupCards(groupId, cardIds);
        }

        public void AddUserGroupCards(string cardIds, long groupId, long userId)
        {
            BusidexDAL.AddUserGroupCards(cardIds, groupId, userId);
        }

        public void RemoveGroupCards(long groupId, string cardIds,  long userId)
        {
            _dao.RemoveGroupCards(groupId, cardIds, userId);
        }

        public void UpdateCardLinks(long cardId, List<DataAccess.ExternalLink> links)
        {
            _dao.UpdateCardLinks(cardId, links);
        }

        public async Task<AddOrUpdateCardErrors> EditCard(DataAccess.DTO.Card cardModel, bool isMyCard, long userId, string notes)
        {
            var modelErrors = new AddOrUpdateCardErrors();// CheckForCardModelErrors(cardModel, isMyCard);

            if (modelErrors.ErrorCollection.Count == 0)
            {
                #region Phone Numbers

                var newPhoneNumbers = new List<PhoneNumber>();
                newPhoneNumbers.AddRange(cardModel.PhoneNumbers.Where(p => p.PhoneNumberId == 0));

                foreach (var existingPhoneNumber in cardModel.PhoneNumbers.Where(p => p.PhoneNumberId > 0))
                {
                    var phoneNumber = _dao.GetPhoneNumberById(existingPhoneNumber.PhoneNumberId);

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

                var tags = _dao.GetCardTags(cardModel.CardId);// _dao.GetCardTags(cardModel.CardId);
                foreach (var tag in tags)
                {
                    // if it's not found, it's been removed
                    if (cardModel.Tags.All(t => !string.Equals(t.Text, tag.Text, StringComparison.CurrentCultureIgnoreCase)))
                    {
                        _dao.DeleteTag(cardModel.CardId, tag.TagId);
                    }
                }

                foreach (var tag in cardModel.Tags)
                {
                    if (tag.TagId == 0)
                    {
                        _dao.AddTag(cardModel.CardId, tag);
                    }                    
                }
                

                #endregion

                #region Addresses
                foreach (var address in cardModel.Addresses)
                {
                    if (address.CardAddressId == 0)
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
                var uc = _dao.GetUserCard(cardModel.CardId, userId);//BusidexDAL.GetUserCard(cardModel.CardId, userId);
                if (uc != null)
                {
                    await _dao.UpdateUserCard(uc.UserCardId, notes);
                }                
            }

            return modelErrors;
        }

        public async Task SendCardUpdatedEmails()
        {

            var updatedCards = BusidexDAL.GetRecentlyUpdatedCards();

            foreach (var cardId in updatedCards)
            {
                var cardModel = BusidexDAL.GetCardById(cardId, 0);
                if (cardModel == null)
                {
                    continue;
                }

                try
                {
                    await NotifyUsersOfChangedCard(cardModel);
                }
                catch (Exception ex)
                {
                    await SaveApplicationError(ex, 0);
                }
            }
            
        }

        private async Task NotifyUsersOfChangedCard(DataAccess.DTO.Card card)
        {
            List<string> emails = BusidexDAL.GetUsersThatHaveCard(card.CardId);
            var ownerEmail = string.Empty;

            var ownerAccount = BusidexDAL.GetUserAccountByUserId(card.OwnerId.GetValueOrDefault());
            if (ownerAccount != null)
            {
                ownerEmail = ownerAccount.BusidexUser.Email;
            }

            foreach (var email in emails)
            {
                if (!email.Equals(ownerEmail, StringComparison.InvariantCultureIgnoreCase) && !string.IsNullOrEmpty(email))
                {
                    // Get the email template for updated card notification and send the emails.
                    var template = BusidexDAL.GetEmailTemplate(EmailTemplateCode.CardUpdated);
                    if (template != null)
                    {
                        /*
                         0: card name
                         1: front file name
                         2: front height
                         3: front width
                         4: back file name
                         5: back height
                         6: back width
                         7: back dispay (block or none)
                         8: card name
                         9: company name
                         10: title
                         11: email
                         12: url
                         13: address
                         14: phone numbers
                         */

                        string height = card.FrontOrientation == "H" ? "120px" : "220px";
                        string width = card.FrontOrientation == "H" ? "210px" : "140px";

                        string backHeight = card.BackOrientation == "H" ? "120px" : "220px";
                        string backWidth = card.BackOrientation == "H" ? "210px" : "140px";
                        string backDisplay = "block";
                        if (card.BackFileId == null ||
                            card.BackFileId.ToString().ToUpper() == "B66FF0EE-E67A-4BBC-AF3B-920CD0DE56C6" ||
                            card.BackFileId.ToString().ToUpper() == Guid.Empty.ToString())
                        {
                            backDisplay = "none";
                        }
                        string address = string.Empty;
                        if (card.Addresses != null && card.Addresses.Count > 0)
                        {
                            address = card.Addresses[0].ToString();
                        }
                        string phoneNumbers = string.Empty;
                        foreach (var number in card.PhoneNumbers)
                        {
                            phoneNumbers += number + Environment.NewLine;
                        }

                        string name = string.IsNullOrEmpty(card.Name) ? card.CompanyName : card.Name;
                        template.Subject = string.Format(template.Subject, name);

                        template.Body = string.Format(template.Body, name, card.FrontFileId + "." + card.FrontType, height, width,
                            card.BackFileId + "." + card.BackType, backHeight, backWidth, backDisplay, card.Name, card.CompanyName, card.Title,
                            card.Email, card.Url, address, phoneNumbers);

                        var communication = new Communication
                        {
                            EmailTemplate = template,
                            Email = email,
                            Body = template.Body,
                            DateSent = DateTime.UtcNow,
                            UserId = 0,
                            SentById = card.OwnerId.GetValueOrDefault(),
                            OwnerToken = null,
                            Failed = false,
                            EmailTemplateId = template.EmailTemplateId
                        };

                        
                       await SendEmail(communication);
                       BusidexDAL.SaveCommunication(communication);
                    }
                }    
            }
        }

        public AddOrUpdateCardErrors CheckForCardModelErrors(DataAccess.DTO.Card cardModel, bool isMyCard)
        {
            var modelErrors = new AddOrUpdateCardErrors();

            #region Required if this is the card owner

            //const string ERROR_PHONE_NUMBER = "Each card must have a phone number";
            //const string ERROR_PHONE_NUMBER_MISSING_NUMBER = "One or more invalid phone numbers";
            const string ERROR_EMAIL = "Each card must have an email";
            //const string ERROR_NAME = "Name is required";
            //const string ERROR_COMPANY = "Company is required";
            //const string ERROR_DUPLICATE_CARD = "One or more cards with the same email were found";
            const string ERROR_NO_CARD_IMAGE = "Card front image is required";

            if (cardModel.FrontImage == null)
            {
                modelErrors.ErrorCollection.Add("MissingFrontImage", ERROR_NO_CARD_IMAGE);
            }
            if (string.IsNullOrEmpty(cardModel.Email))
            {
                modelErrors.ErrorCollection.Add("MissingEmail", ERROR_EMAIL);
            }

            //if (isMyCard) // requirements for card owner
            //{
            //    if (cardModel.PhoneNumbers.Count == 0)
            //    {
            //        modelErrors.ErrorCollection.Add("MissingPhone", ERROR_PHONE_NUMBER);
            //    }
            //    if (string.IsNullOrEmpty(cardModel.Email))
            //    {
            //        modelErrors.ErrorCollection.Add("MissingEmail", ERROR_EMAIL);
            //    }
            //    if (string.IsNullOrEmpty(cardModel.Name))
            //    {
            //        modelErrors.ErrorCollection.Add("MissingName", ERROR_NAME);
            //    }
            //    if (string.IsNullOrEmpty(cardModel.CompanyName))
            //    {
            //        modelErrors.ErrorCollection.Add("MissingCompany", ERROR_COMPANY);
            //    }
            //}
            //else  // 
            //{
            //    //if (string.IsNullOrEmpty(cardModel.Email))
            //    //{
            //    //    modelErrors.ErrorCollection.Add("MissingEmail", ERROR_EMAIL);
            //    //}
            //    if (cardModel.PhoneNumbers.Count == 0)
            //    {
            //        modelErrors.ErrorCollection.Add("MissingPhone", ERROR_PHONE_NUMBER);
            //    }
                
            //}
            //if (cardModel.PhoneNumbers.Count > 0 && cardModel.PhoneNumbers.Any(p => string.IsNullOrEmpty(p.Number)))
            //{
            //    modelErrors.ErrorCollection.Add("MissingPhone", ERROR_PHONE_NUMBER_MISSING_NUMBER);
            //}
            #endregion

            //#region Check for duplicate email
            //modelErrors.ExistingCards = modelErrors.ExistingCards ?? new List<Card>();
            //modelErrors.ExistingCards.AddRange(GetDuplicateCardsByEmail(cardModel.CardId, cardModel.Email));
            //if (modelErrors.ExistingCards.Any())
            //{
            //    modelErrors.ErrorCollection.Add("DuplicateEmail", ERROR_DUPLICATE_CARD);
            //}
            //#endregion
            return modelErrors;
        }

        public Image ScaleImage(Image image, string orientation)
        {
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

        public void ClearBusidexCache()
        {
            //Bcp.UpdateCache(BusidexCacheProvider.CachKeys.MyBusidex, null);    
        }

        public async Task UploadCardUpdateToBlobStorage(AddOrEditCardModel model, string storageConnectionString, string cardRef)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve reference to a previously created container.
            CloudBlobContainer container = blobClient.GetContainerReference("card-update");

            // Retrieve reference to a blob named "myblob".
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(cardRef);

            // Create or overwrite the "myblob" blob with contents from a local file.
            try
            {
                var json = JsonConvert.SerializeObject(model);
                await blockBlob.UploadFromByteArrayAsync(Encoding.ASCII.GetBytes(json),0, json.Length);
            }
            catch(Exception ex)
            {
                await SaveApplicationError(ex, 0);
            }  
        }

        public void AddCardToQueue(string connectionString, string cardUpdateRef)
        {
            const string QUEUE_NAME = "card-update";
            Azure.Storage.Queues.QueueClient queueClient = new Azure.Storage.Queues.QueueClient(connectionString, QUEUE_NAME);
            queueClient.CreateIfNotExists();

            var bytes = ASCIIEncoding.ASCII.GetBytes(cardUpdateRef);
            queueClient.SendMessage(System.Convert.ToBase64String(bytes));
        }

        public async Task AddSharedCardsToQueue(string connectionString, List<SharedCard> sharedCardList)
        {            
            const string QUEUE_NAME = "shared-card";
            Azure.Storage.Queues.QueueClient queueClient = new Azure.Storage.Queues.QueueClient(connectionString, QUEUE_NAME);
            queueClient.CreateIfNotExists();

            foreach (var sharedCardModel in sharedCardList)
            {
                var json = JsonConvert.SerializeObject(sharedCardModel);
                var bytes = ASCIIEncoding.ASCII.GetBytes(json);
                await queueClient.SendMessageAsync(System.Convert.ToBase64String(bytes));
            }            
        }

        public async Task AddSharedCardToQueue(string connectionString, SharedCard sharedCard)
        {            
            const string QUEUE_NAME = "shared-card";
            Azure.Storage.Queues.QueueClient queueClient = new Azure.Storage.Queues.QueueClient(connectionString, QUEUE_NAME);
            queueClient.CreateIfNotExists();
            
            var json = JsonConvert.SerializeObject(sharedCard);
            var bytes = ASCIIEncoding.ASCII.GetBytes(json);
            await queueClient.SendMessageAsync(System.Convert.ToBase64String(bytes));
        }

        public EmailTemplate GetSharedCardEmailPreview(SharedCard model)
        {
            var card = GetCardById(model.CardId);
            if (card != null)
            {
                var template = _dao.GetEmailTemplate(EmailTemplateCode.Invitation);
                var account = _dao.GetUserAccountByUserId(model.SendFrom);
                template.Populate(model, account, card);
                return template;
            }
            
            return null;
        }

        public async Task SendSharedCard(SharedCard model)
        {
            var card = GetCardById(model.CardId);

            if (card != null)
            {
                var template = _dao.GetEmailTemplate(EmailTemplateCode.SharedCard);
                var account = _dao.GetUserAccountByUserId(model.SendFrom);
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
                    _dao.SaveCommunication(communication);
                }
            }

        }

        public SharedCard GetSharedCard(long cardId, long sendFrom, long shareWith)
        {
            return _dao.GetSharedCard(cardId, sendFrom, shareWith);
        }

        
        public async Task SendOwnerNotificationOfSharedCard(SharedCard model)
        {
            var card = GetCardById(model.CardId);
            var template = _dao.GetEmailTemplate(EmailTemplateCode.SharedCardOwner);
            var account = _dao.GetUserAccountByUserId(model.SendFrom);
            var ownerAccount = _dao.GetUserAccountByUserId(card.OwnerId.GetValueOrDefault());
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
                    _dao.SaveCommunication(communication);
                }
            }
        }

        public async Task SendSharedCardInvitation(SharedCard model)
        {

            var card = GetCardById(model.CardId);

            if (card != null)
            {
                var template = _dao.GetEmailTemplate(EmailTemplateCode.Invitation);
                var account = _dao.GetUserAccountByUserId(model.SendFrom);
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
                    _dao.SaveCommunication(communication);
                }

            }
        }

        private static Communication GetCommunication(SharedCard model, DataAccess.DTO.Card card, UserAccount account, EmailTemplate template)
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
                var connectionString = ConfigurationManager.AppSettings["BusidexQueuesConnectionString"];
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

        public void AddSystemTagToCard(long cardid, string tag)
        {
            _dao.AddSystemTagToCard(cardid, tag);  
        }

        public async Task<long> AddCard(DataAccess.DTO.Card card, bool isMyCard, long userId, string notes)
        {
            // Is this user the owner?
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

        public CardDetailModel GetCardByEmail(string email)
        {
            return _dao.GetCardByEmail(email);
        }

        public List<CardDetailModel> GetCardsByPhoneNumber(long userId, string phoneNumber)
        {
            return _dao.GetCardsByPhoneNumber(userId, phoneNumber);
        }

        public List<SharedCard> Get30DaySharedCards()
        {
            return _dao.Get30DaySharedCards();
        }

        public CardDetailModel GetCardByToken(string token)
        {
			DataAccess.DTO.Card card = _dao.GetCardByToken(token);
            CardDetailModel model = null;
            if (card != null)
            {
                model = new CardDetailModel(card);
            }

            return model;
        }

        public async Task<bool> SaveCardOwner(long cardId, long ownerId)
        {
            return await _dao.SaveCardOwner(cardId, ownerId);
        }

        public bool SaveCardOwnerToken(long cardId, Guid token)
        {
            if (cardId > 0)
            {
                return BusidexDAL.SaveCardOwnerToken(cardId, token);
            }
            return false;
        }

        public bool IsACardOwner(long ownerId)
        {
            var isCardOwner = BusidexDAL.IsACardOwner(ownerId);
            return isCardOwner;
        }

        public OrgCardDetailModel GetOrganizationCardByOwnerId(long ownerId)
        {
            return _dao.GetOrganizationCardByOwnerId(ownerId);
        }

        public List<CardDetailModel> GetCardsByOwnerId(long ownerId)
        {

            List<CardDetailModel> ownerCards = BusidexDAL.GetCardsByOwnerId(ownerId);
            
            var returnList = ownerCards.Count > 0
                                 ? (from item in ownerCards
                                    select item).ToList()
                                 : new List<CardDetailModel>();
            return returnList;
        }

        public void DeleteCard(long id, long userId)
        {
            BusidexDAL.DeleteCard(id);

            var userCard = GetUserCard(id, userId);// BusidexDAL.GetUserCard(id, userId);
            if (userCard != null)
            {
                BusidexDAL.DeleteUserCard(userCard, userId);
            }
        }

        public void InvalidateBusidexCache()
        {
            //Bcp.UpdateCache(BusidexCacheProvider.CachKeys.MyBusidex, null);
        }
        public void SaveSharedCards(List<SharedCard> sharedCards)
        {
            BusidexDAL.SaveSharedCards(sharedCards);
        }

        public void SaveSharedCard(SharedCard sharedCard)
        {
            _dao.SaveSharedCard(sharedCard);
        }

        public List<SharedCard> GetSharedCards(long userId)
        {
            var cards = _dao.GetSharedCards(userId); 
            return cards;
        }

        public async Task AcceptSharedCard(long cardId, long userId)
        {
            await _dao.AcceptSharedCard(cardId, userId);
        }

        public void DeclineSharedCard(long cardId, long userId)
        {
            _dao.DeclineSharedCard(cardId, userId);
        }

        public async Task UpdateCardFileId(long cardId, Guid frontFileId, string frontType, Guid backFileId, string backType)
        {
            await _dao.UpdateCardFileId(cardId, frontFileId, frontType, backFileId, backType);
        }

        public List<DataAccess.DTO.Card> GetAllCards()
        {
            return BusidexDAL.GetAllCards();
        }

        public List<UnownedCard> GetUnownedCards()
        {
            return BusidexDAL.GetUnownedCards();
        }

        public List<StateCode> GetAllStateCodes()
        {
            return BusidexDAL.GetAllStateCodes();
        }

        public void UpdateCardBasicInfo(long cardId, string name, string company, string phone, string email)
        {
            BusidexDAL.UpdateCardBasicInfo(cardId, name, company, phone, email);
        }

        public async Task UpdateCardOrientation(long cardId, string frontOrientation, string backOrientation)
        {
            await _dao.UpdateCardOrientation(cardId, frontOrientation, backOrientation);
        }
        
        public void UpdateMobileView(long id, bool isMobileView)
        {
            BusidexDAL.UpdateMobileView(id, isMobileView);
        }

        public long GetCardCount()
        {
            return BusidexDAL.GetCardCount();
        }

        public void AddEventActivity(EventActivity activity)
        {
            BusidexDAL.AddEventActivity(activity);    
        }

        public List<EventSource> GetAllEventSources()
        {
            return BusidexDAL.GetAllEventSources();
        }

        public List<EventActivity> GetEventActivities(long cardId, byte month)
        {
            return BusidexDAL.GetEventActivities(cardId, month);
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
                var card = GetCardById(cardId, userId);
                //var defaultCard = GetCardById(DEMO_CARD_ID, userId);

                if (card == null) return;

                #region Update file name in DB

                card.FrontFileId = frontFileId;// card.FrontFileId ?? Guid.NewGuid();
                card.BackFileId = backFileId;// card.BackFileId ?? Guid.NewGuid();

                await UpdateCardFileId(cardId, card.FrontFileId.GetValueOrDefault(), frontType, card.BackFileId.GetValueOrDefault(), backType);

                #endregion

                #region Save card to file system

                var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["CardImageStorageConnection"]);
                var blobStorage = storageAccount.CreateCloudBlobClient();
                var container = blobStorage.GetContainerReference(ConfigurationManager.AppSettings["CardImageBlobStorageContainer"]);

                string uniqueBlobName = $"{card.FrontFileId}.{frontType}".ToLower();
                if (replaceFront && frontImage != null)
                {
                    var frontImageBlob = container.GetBlockBlobReference(uniqueBlobName);
                    frontImageBlob.Properties.ContentType = mimeTypes[frontType];
                    bool deleted = frontImageBlob.DeleteIfExists();
                    frontImageBlob.Properties.CacheControl = "public, max-age=31536000";
                    frontImageBlob.UploadFromStream(new MemoryStream(frontImage.ToArray()));

                    SaveThumbnail(frontImage.ToArray(), card.FrontOrientation, card.FrontFileId.GetValueOrDefault().ToString(), frontType);
                }

                if (replaceBack && card.BackImage != null && backImage != null)
                {
                    uniqueBlobName = $"{card.BackFileId}.{backType}";
                    var backImageBlob = container.GetBlockBlobReference(uniqueBlobName);
                    backImageBlob.Properties.ContentType = mimeTypes[backType];
                    backImageBlob.Properties.CacheControl = "public, max-age=31536000";
                    backImageBlob.UploadFromStream(new MemoryStream(backImage.ToArray()));

                    SaveThumbnail(backImage.ToArray(), card.BackOrientation, card.BackFileId.GetValueOrDefault().ToString(), backType);
                }

                #endregion
            }
            catch (Exception ex)
            {
                await SaveApplicationError(ex.Message, ex.InnerException?.Message ?? string.Empty, ex.StackTrace, 0);
            }
        }



        private void SaveThumbnail(byte[] cardImage, string orientation, string fileName, string fileType)
        {
            var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["CardImageStorageConnection"]);
            var blobStorage = storageAccount.CreateCloudBlobClient();
            var container = blobStorage.GetContainerReference("mobile-images");

            string uniqueBlobName = $"{fileName}.{fileType}";
            var frontImageBlob = container.GetBlockBlobReference(uniqueBlobName);

            Image img = new Bitmap(new MemoryStream(cardImage));
            var scaledImage = ScaleImage(img, orientation);

            using (var memoryStream = new MemoryStream())
            {
                scaledImage.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);

                memoryStream.Seek(0, SeekOrigin.Begin);
                frontImageBlob.UploadFromStream(memoryStream);
            }
        }

        public List<SEOCardResult> GetSeoCardResult()
        {
            return _dao.GetSeoCardResult();
        }
    }




    /// <summary>
    /// Extension methods to compress a string or bojects to zipped array of bytes,
    /// and decompress a byte array containing zipped data to a string or object
    /// again
    /// </summary>
    public static class StringZipExtensions
    {
        #region Compress
        public static byte[] CompressObject(
            this object objectToCompress)
        {
            var bf = new BinaryFormatter();
            var ms = new MemoryStream();
            bf.Serialize(ms, objectToCompress);
            using (var memoryStream = new MemoryStream())
            {
                using (var zipStream = new GZipStream(memoryStream,
                    CompressionMode.Compress))
                {
                    zipStream.Write(ms.ToArray(), 0, (int)ms.Length);
                    zipStream.Close();
                    return (memoryStream.ToArray());
                }
            }
        }
        /// <summary>
        /// Compresses the specified string to a byte array
        /// using the specified encoding.
        /// </summary>
        /// <param name="stringToCompress">The string to compress.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns>bytes array with compressed string</returns>
        public static byte[] Compress(
            this string stringToCompress, 
             Encoding encoding )
         {
             var stringAsBytes = encoding.GetBytes(stringToCompress);
             using (var memoryStream = new MemoryStream())
             {
                 using (var zipStream = new GZipStream(memoryStream,
                     CompressionMode.Compress))
                 {
                     zipStream.Write(stringAsBytes, 0, stringAsBytes.Length);
                     zipStream.Close();
                     return (memoryStream.ToArray());
                 }
             }
         }

         /// <summary>
         /// Compresses the specified string a byte array using default
         /// UTF8 encoding.
         /// </summary>
         /// <param name="stringToCompress">The string to compress.</param>
         /// <returns>bytes array with compressed string</returns>
        public static byte[] Compress( this string stringToCompress )
        {
            return Compress(stringToCompress, new UTF8Encoding());
        }


        /// <summary>
        /// XmlSerializes the object to a compressed byte array
        /// using the specified encoding.
        /// </summary>
        /// <param name="objectToCompress">The object to compress.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns>bytes array with compressed serialized object</returns>
        public static byte[] Compress(this object objectToCompress,
            Encoding encoding)
        {
            //var xmlSerializer = new XmlSerializer(objectToCompress.GetType());
            //using (var stringWriter = new StringWriter())
            //{
            //    xmlSerializer.Serialize(stringWriter, objectToCompress);
            //    return stringWriter.ToString().Compress(encoding);
            //}
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All,
                //TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
                Formatting = Formatting.Indented
            };


            var jsMessage = JsonConvert.SerializeObject(objectToCompress, settings);
            return jsMessage.Compress(encoding);
        }

        /// <summary>
        /// XmlSerializes the object to a compressed byte array using default
        /// UTF8 encoding.
        /// </summary>
        /// <param name="objectToCompress">The object to compress.</param>
        /// <returns>bytes array with compressed serialized object</returns>
        public static byte[] Compress(this object objectToCompress)
        {
            return Compress(objectToCompress, new UTF8Encoding());
        }
        #endregion

        #region Decompress
        /// <summary>
        /// Decompress an array of bytes to a string 
        /// using the specified encoding
        /// </summary>
        /// <param name="compressedString">The compressed string.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns>Decompressed string</returns>
        public static string DecompressToString(
            this byte[] compressedString, 
            Encoding encoding)
        {
            const int BUFFER_SIZE = 1024;
            using (var memoryStream = new MemoryStream(compressedString))
            {
                using (var zipStream = new GZipStream(memoryStream,
                    CompressionMode.Decompress))
                {
                    // Memory stream for storing the decompressed bytes
                    using (var outStream = new MemoryStream())
                    {
                        var buffer = new byte[BUFFER_SIZE];
                        var totalBytes = 0;
                        int readBytes;
                        while ((readBytes = zipStream.Read(buffer,0, BUFFER_SIZE)) > 0)
                        {
                            outStream.Write(buffer, 0, readBytes);
                            totalBytes += readBytes;
                        }
                        return encoding.GetString(
                            outStream.GetBuffer(),0, totalBytes);                   
                    }
                }
            }
        }

        /// <summary>
        /// Decompress an array of bytes to a string using default
        /// UTF8 encoding.
        /// </summary>
        /// <param name="compressedString">The compressed string.</param>
        /// <returns>Decompressed string</returns>
        public static string DecompressToString(this byte[] compressedString )
        {
            return DecompressToString(compressedString, new UTF8Encoding());
        }


        /// <summary>
        /// Decompress an array of bytes into an object via Xml Deserialization
        /// using the specified encoding
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="compressedObject">The compressed string.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns>Decompressed object</returns>
        public static T DecompressToObject<T>(this byte[] compressedObject,
            Encoding encoding)
        {
            var xmlSer = new XmlSerializer(typeof(T));
            return (T)xmlSer.Deserialize(new StringReader(
                compressedObject.DecompressToString(encoding)));
        }

        /// <summary>
        /// Decompress an array of bytes into an object via Xml Deserialization
        /// using the specified encoding
        /// </summary>
        /// <param name="compressedObject">The compressed string.</param>
        /// <returns>Decompressed object</returns>
        public static T DecompressToObject<T>(this byte[] compressedObject )
        {
            return DecompressToObject<T>(compressedObject, new UTF8Encoding());
        }

        #endregion
    }
}

