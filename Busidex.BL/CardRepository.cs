using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using Busidex.BL.Interfaces;
using Busidex.DAL;
using Busidex.Providers;

namespace Busidex.BL
{
    public class CardRepository : RepositoryBase, ICardRepository
    {
        public CardRepository(IBusidexDataContext busidexDal)
            : base(busidexDal)
        {
        }

        public void DeleteUserCard(UserCard uc, long userId)
        {
            if (uc != null)
            {
                BusidexDAL.DeleteUserCard(uc, userId);
                Bcp.UpdateCache(BusidexCacheProvider.CachKeys.MyBusidex, null);
            }
        }

        public void AddToMyBusidex(long id, long userId)
        {
            var userCard = new UserCard(id, userId);

            BusidexDAL.AddUserCard(userCard);
            Bcp.UpdateCache(BusidexCacheProvider.CachKeys.MyBusidex, null);
        }

        public void AddSendersCardToMyBusidex(string token, long userId)
        {
            Communication communication = BusidexDAL.GetCommunicationByActivationToken(token);
            if (communication != null)
            {
                List<Card> card = BusidexDAL.GetCardsByOwnerId(communication.SentById);
                if (card != null)
                {
                    foreach (var c in card)
                    {
                        var userCard = new UserCard(c.CardId, userId);
                        BusidexDAL.AddUserCard(userCard);
                    }
                    Bcp.UpdateCache(BusidexCacheProvider.CachKeys.MyBusidex, null);    
                }
            }
        }

        public UserCard GetUserCard(long id, long userId)
        {
            return BusidexDAL.GetUserCard(id, userId);

        }

        //public void UpdateLizzidex(int coffee, int thing)
        //{
        //    BusidexDAL.UpdateLizzidex(coffee, thing);
        //}

        public CardDetailModel GetCardDetail(Card card)
        {
            return new CardDetailModel(card);
        }

        public void SaveCardNotes(long id, string notes)
        {
            BusidexDAL.UpdateUserCard(id, notes);
            Bcp.UpdateCache(BusidexCacheProvider.CachKeys.MyBusidex, null);
        }

        public void SaveGroupCardNotes(long id, string notes)
        {
            BusidexDAL.UpdateUserGroupCard(id, notes);
        }

        public Card GetCardById(long cardId)
        {
            return BusidexDAL.GetCardById(cardId);
        }

        public AddOrEditCardModel GetAddOrEditModel(AddOrEditCardModel model)
        {
            Card card = BusidexDAL.GetCardById(model.CardId);

            model.PhoneNumbers = new List<PhoneNumber>();
            model.PhoneNumbers.AddRange(card.PhoneNumbers.Where(pn => pn.Deleted == false));
            model.PhoneNumberTypes = BusidexDAL.GetAllPhoneNumberTypes();
            model.Addresses = card.Addresses ?? new List<CardAddress>();
            model.Tags = card.Tags ?? new List<Tag>();
            //model.StateCodes = GetAllStateCodes();

            return model;
        }

        public AddOrEditCardModel GetAddOrEditModel(long cardId, BusidexUser bu, string action)
        {
            var card = cardId == 1 ? new Card() : BusidexDAL.GetCardById(cardId);
            var appSettings = new AppSettingsReader();
            card = card ?? new Card();
            if (card.Addresses.Count == 0)
            {
                card.Addresses.Add(new CardAddress());
            }
            var uc = GetUserCard(card.CardId, bu.UserId);
            var model = new AddOrEditCardModel
                            {
                                //CanHaveRelatedCards =
                                //    bu.UserAccount != null && bu.UserAccount.AccountTypeId >= ACCT_TYPE_HAS_RELATIONS && bu.UserAccount.AccountTypeId != ACCT_TYPE_BETA,
                                PhoneNumberTypes = new List<PhoneNumberType>(),//.GetAllPhoneNumberTypes(),
                                FileSizeLimit = Convert.ToString(appSettings.GetValue("FileSizeLimit", typeof(string))),
                                FileSizeInfoContent = string.Empty,// BusidexDAL.GetCustomContentById(1),
                                MyBusidex = new List<UserCard>(),// GetMyBusidex(bu.UserId),
                                ActionMethod = action,
                                BackImage = card.BackImage,
                                BackOrientation = card.BackOrientation,
                                BackType = card.BackType != null ? card.BackType.Replace(".", "") : string.Empty,
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
                                FrontType = card.FrontType != null ? card.FrontType.Replace(".", "") : string.Empty,
                                PhoneNumbers = card.PhoneNumbers.Where(pn => pn.Deleted == false).ToList(),
                                HasFrontImage = card.FrontImage != null && card.FrontImage.Length > 0 && cardId > 1,
                                HasBackImage = card.BackImage != null && card.BackImage.Length > 0 && cardId > 1,
                                FrontFileId = card.FrontFileId,
                                BackFileId = card.BackFileId,
                                Tags = card.Tags ?? new List<Tag>(),
                                Addresses = card.Addresses,
                               // StateCodes = new List<StateCode>(),// GetAllStateCodes(),
                                Markup = card.Markup,
                                Display = card.Display,
                                Notes = uc != null ? uc.Notes : string.Empty
                            };
            if (card.OwnerId.HasValue)
            {
                model.IsMyCard = card.OwnerId == bu.UserId;
            } 

            if (action == "Edit")
            {
                model.IsMyCard = card.OwnerId == bu.UserId;
            }

            if (model.PhoneNumbers.Count == 0)
            {
                model.PhoneNumbers.Add(new PhoneNumber
                {
                    Number = string.Empty,
                    PhoneNumberTypeId = 1,
                    Extension = string.Empty
                });
            }

            return model;
        }

        public List<Card> GetDuplicateCardsByEmail(long cardId, string email)
        {
            return BusidexDAL.GetDuplicateCardsByEmail(cardId, email);
        }

        public SearchResultModel Search(SearchResultModel model, long? userId, Tuple<double, double> altAddress)
        {

            var searchModel = new SearchResultModel();
            var user = Bcp.GetFromCache(BusidexCacheProvider.CachKeys.CurrentUser) as BusidexUser;

            if (userId.GetValueOrDefault() > 0 && user == null)
            {
                user = BusidexDAL.GetBusidexUserById(userId.GetValueOrDefault());
                Bcp.UpdateCache(BusidexCacheProvider.CachKeys.CurrentUser, user);
            }
            
            double? latitude = null, longitude = null;

            // use the alternate address or the address associated with the current user.
            if (altAddress != null)
            {
                latitude = altAddress.Item1;
                longitude = altAddress.Item2;
            }
            else if (user != null && user.Address != null)
            {
                latitude = user.Address.Latitude;
                longitude = user.Address.Longitude;
            }
            if (latitude == 0) latitude = null;
            if (longitude == 0) longitude = null;

            var cards = new List<Card>();
            string s = model.SearchText;
            IEqualityComparer<Card> comparer = new CardComparer();
            if (!string.IsNullOrEmpty(s) && !string.IsNullOrWhiteSpace(s))
            {
                IEnumerable<string> searchCriteria = ExtractParameters(s).ToList();
                bool searchableOnly = userId.GetValueOrDefault() == 0;
                cards.AddRange(BusidexDAL.SearchCards(string.Join(",", searchCriteria), latitude, longitude, model.Distance, searchableOnly));
                //var taskSearchCards = Task.Factory.StartNew(() =>
                //    {
                //        //var ctxA = new BusidexDataContext();
                //        var cardsA = new List<Card>();
                //        Parallel.ForEach(searchCriteria, criteria =>
                //        {
                //            if (!string.IsNullOrEmpty(criteria) && !string.IsNullOrWhiteSpace(criteria))
                //            {
                //                var listA = BusidexDAL.SearchCards(criteria, latitude, longitude, model.Distance);
                //                cardsA.AddRange(listA);
                //            }
                //        });
                //        var listB = BusidexDAL.SearchCards(s, latitude, longitude, model.Distance);
                //        cardsA.AddRange(listB);
                //        return cardsA;
                //    });
                #region Not Used

                /*
                #region Cards By Email

                var taskCardsByEmail =
                    Task.Factory.StartNew(() =>
                                              {
                                                  var ctxA = new BusidexDataContext();
                                                  var cardsA = new List<Card>();
                                                  Parallel.ForEach(searchCriteria, criteria =>
                                                                                       {
                                                                                           if (!string.IsNullOrEmpty(criteria) && !string.IsNullOrWhiteSpace(criteria))
                                                                                           {
                                                                                               var list = ctxA.GetCardsByEmail(criteria, latitude, longitude, model.Distance);
                                                                                               cardsA.AddRange(list);
                                                                                           }
                                                                                       });
                                                  return cardsA;
                                              });

                #endregion

                #region Cards By Name Keyword

                var taskCardsByNameKeyword =
                    Task.Factory.StartNew(() =>
                                              {
                                                  var ctxB = new BusidexDataContext();
                                                  var cardsB = new List<Card>();
                                                  Parallel.ForEach(searchCriteria, criteria =>
                                                                                       {
                                                                                           if (!string.IsNullOrEmpty(criteria) && !string.IsNullOrWhiteSpace(criteria))
                                                                                           {
                                                                                               var list = ctxB.GetCardsByNameKeyword(criteria, latitude, longitude, model.Distance);
                                                                                               cardsB.AddRange(list);
                                                                                           }
                                                                                       });
                                                  return cardsB;
                                              });

                #endregion

                #region Cards By Company Name

                var taskCardsByCompanyName =
                    Task.Factory.StartNew(() =>
                                              {
                                                  var ctxC = new BusidexDataContext();
                                                  var cardsC = new List<Card>();
                                                  if (!string.IsNullOrEmpty(s) && !string.IsNullOrWhiteSpace(s))
                                                  {
                                                      var list = ctxC.GetCardsByCompanyName(s, latitude, longitude, model.Distance);
                                                      cardsC.AddRange(list);
                                                  }
                                                  return cardsC;
                                              });

                #endregion

                #region Cards By Phone Number

                var taskCardsByPhoneNumber =
                    Task.Factory.StartNew(() =>
                                              {
                                                  var ctxD = new BusidexDataContext();
                                                  var cardsD = new List<Card>();
                                                  Parallel.ForEach(searchCriteria, criteria =>
                                                                                       {
                                                                                           if (!string.IsNullOrEmpty(criteria) && !string.IsNullOrWhiteSpace(criteria))
                                                                                           {
                                                                                               var list = ctxD.GetCardsByPhoneNumber(criteria, latitude, longitude, model.Distance);
                                                                                               cardsD.AddRange(list);
                                                                                           }
                                                                                       });
                                                  return cardsD;
                                              });

                #endregion

                #region Cards By Title

                var taskCardsByTitle =
                    Task.Factory.StartNew(() =>
                                              {
                                                  var ctxE = new BusidexDataContext();
                                                  var cardsE = new List<Card>();

                                                  Parallel.ForEach(searchCriteria, criteria =>
                                                                                       {
                                                                                           if (!string.IsNullOrEmpty(criteria) && !string.IsNullOrWhiteSpace(criteria))
                                                                                           {
                                                                                               var list = ctxE.GetCardsByTitle(criteria, latitude, longitude, model.Distance);
                                                                                               cardsE.AddRange(list);
                                                                                           }
                                                                                       });

                                                  return cardsE;
                                              });

                #endregion

                #region Cards By Tag

                var taskCardsByTag =
                    Task.Factory.StartNew(() =>
                                              {
                                                  var ctxF = new BusidexDataContext();
                                                  var cardsF = new List<Card>();
                                                  if (!string.IsNullOrEmpty(s) && !string.IsNullOrWhiteSpace(s))
                                                  {
                                                      var list = ctxF.GetCardsByTag(s, latitude, longitude, model.Distance);
                                                      cardsF.AddRange(list);
                                                  }

                                                  return cardsF;
                                              });

                #endregion

                */
                #endregion

                #region Run Tasks

                //var tasks = new Task[1];
                //tasks[0] = taskSearchCards;
                //tasks[0] = taskCardsByEmail;
                //tasks[1] = taskCardsByNameKeyword;
                //tasks[2] = taskCardsByCompanyName;
                //tasks[3] = taskCardsByPhoneNumber;
                //tasks[4] = taskCardsByTitle;
                //tasks[5] = taskCardsByTag;

                //Task.WaitAll(tasks);

                #endregion
                //cards.AddRange(taskSearchCards.Result);
                //cards.AddRange(taskCardsByEmail.Result);
                //cards.AddRange(taskCardsByNameKeyword.Result);
                //cards.AddRange(taskCardsByCompanyName.Result);
                //cards.AddRange(taskCardsByPhoneNumber.Result);
                //cards.AddRange(taskCardsByTitle.Result);
                //cards.AddRange(taskCardsByTag.Result);
            }

            //set up other model attributes
            searchModel.Criteria = s;
            searchModel.Distance = model.Distance;
            searchModel.SearchLocation = model.SearchLocation;
            searchModel.SearchAddress = model.SearchAddress;

            var myBusidex = userId.GetValueOrDefault() > 0 ? GetMyBusidex(userId.GetValueOrDefault()) : new List<UserCard>();
            var distinctCards = cards.Where(_c => _c.Searchable || myBusidex.Any(uc => uc.CardId == _c.CardId))
                                     .Distinct(comparer);

            searchModel.Results = distinctCards.Select(_c => new CardDetailModel(_c))
                                       .ToList();

            // update flag for each card in user's MyBusidex
            searchModel.Results.ForEach(_c => _c.ExistsInMyBusidex = myBusidex.Any(b => b.CardId == _c.CardId));

            searchModel.HasResults = cards.Any();
            searchModel.Display = ViewType.List;

            return searchModel;
        }

        private IEnumerable<string> ExtractParameters(string s)
        {
            var parameters = new List<string>();
            if (Regex.Matches(s, "\"").Count == 2)
            {

            }
            else
            {
                parameters.AddRange(s.Split(' ').Where(_s => _s.Length >= 3));
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
            var myBusidex = Bcp.GetFromCache(BusidexCacheProvider.CachKeys.MyBusidex) as List<UserCard>;
            if (myBusidex == null || myBusidex.Count == 0)
            {
                myBusidex = BusidexDAL.GetMyBusidex(userId, includeImages); 
                Bcp.UpdateCache(BusidexCacheProvider.CachKeys.MyBusidex, myBusidex);

                return myBusidex;
            }

            return Bcp.GetFromCache(BusidexCacheProvider.CachKeys.MyBusidex) as List<UserCard>;
        }

        public List<UserGroupCard> GetBusiGroupCards(long groupId, bool includeImages = false)
        {
            return BusidexDAL.GetBusiGroupCards(groupId, includeImages);
        }

        public DAL.Group GetBusiGroupById(long groupId)
        {
            return BusidexDAL.GetBusiGroupById(groupId);
        }

        public List<DAL.Group> GetMyBusiGroups(long userId)
        {
            var myBusgroups = Bcp.GetFromCache(BusidexCacheProvider.CachKeys.MyBusiGroups) as List<DAL.Group>;
            if (myBusgroups == null || myBusgroups.Count == 0)
            {
                myBusgroups = BusidexDAL.GetMyBusiGroups(userId); 
                Bcp.UpdateCache(BusidexCacheProvider.CachKeys.MyBusiGroups, myBusgroups);

                return myBusgroups;
            }

            return Bcp.GetFromCache(BusidexCacheProvider.CachKeys.MyBusiGroups) as List<DAL.Group>;
        }

        public long AddGroup(DAL.Group group)
        {
            Bcp.UpdateCache(BusidexCacheProvider.CachKeys.MyBusiGroups, null); // invalidate the cache
            return BusidexDAL.AddGroup(group);
        }

        public void DeleteGroup(long id)
        {
            BusidexDAL.DeleteGroup(id);
            Bcp.UpdateCache(BusidexCacheProvider.CachKeys.MyBusiGroups, null);
        }

        public long UpdateGroup(DAL.Group group)
        {
            Bcp.UpdateCache(BusidexCacheProvider.CachKeys.MyBusiGroups, null); // invalidate the cache
            return BusidexDAL.UpdateGroup(group);
        }

        public long AddUserGroupCard(UserGroupCard groupCard)
        {
            return BusidexDAL.AddUserGroupCard(groupCard);
        }

        public void AddUserGroupCards(string cardIds, long groupId, long userId)
        {
            BusidexDAL.AddUserGroupCards(cardIds, groupId, userId);
        }

        public void RemoveUserGroupCards(string cardIds, long groupId, long userId)
        {
            BusidexDAL.RemoveUserGroupCards(cardIds, groupId, userId);
        }

        public AddOrUpdateCardErrors EditCard(Card cardModel, bool isMyCard, long userId, string notes)
        {
            var modelErrors = CheckForCardModelErrors(cardModel, isMyCard);

            if (modelErrors.ErrorCollection.Count == 0)
            {
                #region Phone Numbers

                var newPhoneNumbers = new List<PhoneNumber>();
                newPhoneNumbers.AddRange(cardModel.PhoneNumbers.Where(p => p.PhoneNumberId == 0));

                foreach (var existingPhoneNumber in cardModel.PhoneNumbers.Where(p => p.PhoneNumberId > 0))
                {
                    PhoneNumber phoneNumber = BusidexDAL.GetPhoneNumberById(existingPhoneNumber.PhoneNumberId);

                    if (phoneNumber == null) continue;

                    phoneNumber.Number = existingPhoneNumber.Number;
                    phoneNumber.Extension = existingPhoneNumber.Extension;
                    phoneNumber.PhoneNumberTypeId = existingPhoneNumber.PhoneNumberTypeId;
                    phoneNumber.Deleted = existingPhoneNumber.Deleted;
                    phoneNumber.Updated = DateTime.UtcNow;

                    BusidexDAL.UpdatePhonenumber(phoneNumber);
                }

                // Add the new phone numbers
                foreach (var phoneNumber in newPhoneNumbers)
                {
                    phoneNumber.CardId = cardModel.CardId;
                    BusidexDAL.AddPhoneNumber(phoneNumber);
                }

                #endregion

                #region Tags
                foreach (var tag in cardModel.Tags)
                {
                    if (tag.TagId == 0)
                    {
                        BusidexDAL.AddTag(cardModel.CardId, tag);
                    }
                    else
                    {
                        // delete the tag
                        if (tag.TagId > 0 && tag.Deleted)
                        {
                            BusidexDAL.DeleteTag(cardModel.CardId, tag.TagId);
                        }
                    }
                }
                #endregion

                #region Addresses
                foreach (var address in cardModel.Addresses)
                {
                    if (address.CardAddressId == 0)
                    {
                        BusidexDAL.AddAddress(cardModel.CardId, address);
                    }
                    else
                    {
                        // update or delete the address
                        if (address.CardAddressId > 0)
                        {
                            if (address.Deleted)
                            {
                                BusidexDAL.DeleteAddress(address.CardAddressId);
                            }
                            else
                            {
                                BusidexDAL.UpdateAddress(address);
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
                BusidexDAL.UpdateCard(cardModel);

                // Update card notes
                UserCard uc = BusidexDAL.GetUserCard(cardModel.CardId, userId);
                if (uc != null)
                {
                    BusidexDAL.UpdateUserCard(uc.UserCardId, notes);
                }
                // update MyBusidex cache
                Bcp.UpdateCache(BusidexCacheProvider.CachKeys.MyBusidex, null);
                var newBusidex = BusidexDAL.GetMyBusidex(userId, false);
                Bcp.UpdateCache(BusidexCacheProvider.CachKeys.MyBusidex, newBusidex);
            }

            return modelErrors;
        }

        private AddOrUpdateCardErrors CheckForCardModelErrors(Card cardModel, bool isMyCard)
        {
            var modelErrors = new AddOrUpdateCardErrors();

            #region Required if this is the card owner

            const string ERROR_PHONE_NUMBER = "Each card must have a phone number";
            const string ERROR_EMAIL = "Each card must have an email";
            const string ERROR_NAME = "Name is required";
            const string ERROR_COMPANY = "Company is required";
            const string ERROR_DUPLICATE_CARD = "One or more cards with the same email were found";

            if (isMyCard) // requirements for card owner
            {
                if (cardModel.PhoneNumbers.Count == 0)
                {
                    modelErrors.ErrorCollection.Add("MissingPhone", ERROR_PHONE_NUMBER);
                }
                if (string.IsNullOrEmpty(cardModel.Email))
                {
                    modelErrors.ErrorCollection.Add("MissingEmail", ERROR_EMAIL);
                }
                if (string.IsNullOrEmpty(cardModel.Name))
                {
                    modelErrors.ErrorCollection.Add("MissingName", ERROR_NAME);
                }
                if (string.IsNullOrEmpty(cardModel.CompanyName))
                {
                    modelErrors.ErrorCollection.Add("MissingCompany", ERROR_COMPANY);
                }
            }
            else  // 
            {
                if (string.IsNullOrEmpty(cardModel.Email))
                {
                    modelErrors.ErrorCollection.Add("MissingEmail", ERROR_EMAIL);
                }
                if (cardModel.PhoneNumbers.Count == 0)
                {
                    modelErrors.ErrorCollection.Add("MissingPhone", ERROR_PHONE_NUMBER);
                }
            }
            #endregion

            #region Check for duplicate email
            modelErrors.ExistingCards = modelErrors.ExistingCards ?? new List<Card>();
            modelErrors.ExistingCards.AddRange(GetDuplicateCardsByEmail(cardModel.CardId, cardModel.Email));
            if (modelErrors.ExistingCards.Any())
            {
                modelErrors.ErrorCollection.Add("DuplicateEmail", ERROR_DUPLICATE_CARD);
            }
            #endregion
            return modelErrors;
        }

        public AddOrUpdateCardErrors AddCard(Card card, bool isMyCard, long userId, string notes, out long cardId)
        {

            var modelErrors = CheckForCardModelErrors(card, isMyCard);

            if (modelErrors.ErrorCollection.Count == 0)
            {

                // Is this user the owner?
                if (isMyCard)
                {
                    card.OwnerId = userId;
                    card.Searchable = true;
                }

                // Add the new card
                //var card = Card.Clone(cardModel);

                cardId = BusidexDAL.AddCard(card);
                card.CardId = cardId;


                // Add phone numbers
                foreach (var phoneNumber in card.PhoneNumbers)
                {
                    phoneNumber.CardId = cardId;
                    BusidexDAL.AddPhoneNumber(phoneNumber);
                }

                // Add Tags
                foreach (var tag in card.Tags)
                {
                    BusidexDAL.AddTag(cardId, tag);
                }

                // Add Addresses
                foreach (var address in card.Addresses)
                {
                    if (!address.Deleted)
                    {
                        BusidexDAL.AddAddress(cardId, address);
                    }
                }

                // Add the card to MyBusidex
                var busidex = GetMyBusidex(userId);
                if (card.CardId == 0 || busidex.FirstOrDefault(c => c.CardId == card.CardId) == null)
                {
                    var userCard = new UserCard(card, userId) { CardId = cardId, Created = DateTime.Now, Notes = notes };
                    BusidexDAL.AddUserCard(userCard);
                }

                // update MyBusidex cache
                Bcp.UpdateCache(BusidexCacheProvider.CachKeys.MyBusidex, null);
                var newBusidex = BusidexDAL.GetMyBusidex(userId, false);
                Bcp.UpdateCache(BusidexCacheProvider.CachKeys.MyBusidex, newBusidex);
            }
            else
            {
                cardId = -1;
            }

            return modelErrors;
        }


        public CardDetailModel GetCardByToken(string token)
        {
            Card card = BusidexDAL.GetCardByToken(token);
            CardDetailModel model = null;
            if (card != null)
            {
                model = new CardDetailModel(card);
            }

            return model;
        }

        public bool SaveCardOwner(long cardId, long ownerId)
        {
            return BusidexDAL.SaveCardOwner(cardId, ownerId);
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
            var isCardOwner = Bcp.GetFromCache(BusidexCacheProvider.CachKeys.IsACardOwner);
            if (isCardOwner == null)
            {
                isCardOwner = BusidexDAL.IsACardOwner(ownerId);
                Bcp.UpdateCache(BusidexCacheProvider.CachKeys.IsACardOwner, isCardOwner);
            }
            return (bool)isCardOwner;
        }

        public List<CardDetailModel> GetCardsByOwnerId(long ownerId)
        {

            List<Card> ownerCards = BusidexDAL.GetCardsByOwnerId(ownerId);
            var returnList = ownerCards.Count > 0
                                 ? (from item in ownerCards
                                    select new CardDetailModel(item)).ToList()
                                 : new List<CardDetailModel>();
            return returnList;
        }

        public void DeleteCard(long id, long userId)
        {
            BusidexDAL.DeleteCard(id);

            var userCard = BusidexDAL.GetUserCard(id, userId);
            if (userCard != null)
            {
                BusidexDAL.DeleteUserCard(userCard, userId);
            }

            // update MyBusidex cache
            Bcp.UpdateCache(BusidexCacheProvider.CachKeys.MyBusidex, null);
            var newBusidex = BusidexDAL.GetMyBusidex(userId, false);
            Bcp.UpdateCache(BusidexCacheProvider.CachKeys.MyBusidex, newBusidex);

        }

        public void InvalidateBusidexCache()
        {
            Bcp.UpdateCache(BusidexCacheProvider.CachKeys.MyBusidex, null);
        }
        public void SaveSharedCards(List<SharedCard> sharedCards)
        {
            BusidexDAL.SaveSharedCards(sharedCards);
        }

        public List<SharedCard> GetSharedCards(long userId)
        {
            var cards = Bcp.GetFromCache(BusidexCacheProvider.CachKeys.SharedCards) as List<SharedCard>;
            if (cards == null)
            {
                cards = BusidexDAL.GetSharedCards(userId);
                Bcp.UpdateCache(BusidexCacheProvider.CachKeys.SharedCards, cards);
            }
            return cards;
        }

        public void AcceptSharedCard(long cardId, long userId)
        {
            BusidexDAL.AcceptSharedCard(cardId, userId);
        }

        public void DeclineSharedCard(long cardId, long userId)
        {
            BusidexDAL.DeclineSharedCard(cardId, userId);
        }

        public void UpdateCardFileId(long cardId, Guid frontFileId, Guid backFileId)
        {
            BusidexDAL.UpdateCardFileId(cardId, frontFileId, backFileId);
        }

        public List<Card> GetAllCards()
        {
            return BusidexDAL.GetAllCards();
        }

        public List<Card> GetUnownedCards()
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

        public void UpdateMobileView(long id, bool isMobileView)
        {
            BusidexDAL.UpdateMobileView(id, isMobileView);
        }
    }
}
