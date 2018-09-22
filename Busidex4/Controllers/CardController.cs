using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.Mvc;
using Busidex.BL.Interfaces;
using Busidex.DAL;
using System.IO;
using Busidex.Providers;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;
using Newtonsoft.Json;

namespace Busidex4.Controllers
{


    [Authorize]
    public class CardController : BaseController
    {
        const int DEMO_CARD_ID = 1;

        private readonly Dictionary<string, string> _mimeTypes = new Dictionary<string, string>();

        private enum CardContent
        {
            ErrorCardSelectCardImage,
            ErrorCardIndicateOwner,
            ErrorCardOwned
        }

        public CardController(ICardRepository cardRepository, IAccountRepository accountRepository)
            : base(cardRepository, accountRepository)
        {

        }

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            InitMimeTypes();
        }

        private void InitMimeTypes()
        {
            _mimeTypes.Add("jpg", "image/jpeg");
            _mimeTypes.Add("jpeg", "image/jpeg");
            _mimeTypes.Add("png", "image/x-png");
            _mimeTypes.Add("gif", "image/gif");
            _mimeTypes.Add("bmp", "image/bmp");
        }

        private string ImagePreview(int idx)
        {
            string image = string.Empty;
            var httpPostedFileBase = (Request.Files.Count == 1)
                ? Request.Files[0]
                : Request.Files[idx];

            if (httpPostedFileBase != null)
            {
                var fileLen = httpPostedFileBase.InputStream.Length;
                var input = new byte[fileLen];

                // Initialize the stream.
                var myStream = httpPostedFileBase.InputStream;

                // Read the file into the byte array.
                myStream.Read(input, 0, (int)fileLen);
                image = Convert.ToBase64String(input);
            }
            return image;
        }

        [ErrorLogger]
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Add()
        {
            MembershipUser user = Membership.GetUser();
            if (user != null && user.ProviderUserKey != null)
            {
                BusidexUser bu = _cardRepository.GetBusidexUserById((long)user.ProviderUserKey);

                AddOrEditCardModel model = _cardRepository.GetAddOrEditModel(DEMO_CARD_ID, bu, "Add");

                model.MyEmail = bu.Email;
                model.CreatedBy = bu.UserId;

                return View(model);
            }
            return null;
        }

        [AllowAnonymous]
        public void MobileAdd(long mId, string email, string name)
        {
            const string NULL_REQUEST = "Null Request";
            const string FILE_COUNT_ZERO = "Files.count = 0";
            const string FRONT_IMAGE_NULL = "frontImage.InputStream is null";
            const string FRONT_IMAGE_LENGTH_ZERO = "frontImage.InputStream.Length is 0";
            const string FILE_EXTENSION_MISSING = "no file extension";
            const string NO_FILE_UPLOADED = "No file uploaded";

            try
            {
                var model = new AddOrEditCardModel
                                {
                                    ActionMethod = "Add",
                                    Name = email,
                                    CreatedBy = mId,
                                    BackOrientation = "H",
                                    IsMyCard = true,
                                    Email = email,
                                    PhoneNumbers = new List<PhoneNumber>(),
                                    Tags = new List<Tag>(),
                                    Addresses = new List<CardAddress>()
                                };

                if (Request == null)
                {
                    throw new ArgumentNullException(NULL_REQUEST);
                }
                if (Request.Files.Count == 0)
                {
                    throw new ArgumentNullException(FILE_COUNT_ZERO);
                }

                HttpPostedFileBase frontImage = Request.Files["file"];
                if (frontImage != null && frontImage.ContentLength > 0)
                {
                    if (frontImage.InputStream == null)
                    {
                        throw new ArgumentNullException(FRONT_IMAGE_NULL);
                    }

                    if (frontImage.InputStream.Length == 0)
                    {
                        throw new ArgumentNullException(FRONT_IMAGE_LENGTH_ZERO);
                    }

                    var frontBuf = new byte[frontImage.InputStream.Length];

                    frontImage.InputStream.Read(frontBuf, 0, (int)frontImage.InputStream.Length);
                    var extension = Path.GetExtension(frontImage.FileName);
                    string frontType;
                    if (extension != null)
                    {
                        frontType = extension.ToLower().Trim();
                    }
                    else
                    {
                        throw new ArgumentNullException(FILE_EXTENSION_MISSING);
                    }
                    model.FrontFileId = null;
                    model.LoadImage(frontBuf, frontType, true);
                    model.FrontType = "jpg";

                    long cardId;
                    var myBusidex = _cardRepository.GetMyBusidex(mId, false);
                    //_cardRepository.AddCard(model, mId, myBusidex, out cardId);

                    //CardToFile(cardId, true, model.BackImage != null);
                }
                else
                {
                    throw new ArgumentNullException(NO_FILE_UPLOADED);
                }
            }
            catch (Exception ex)
            {
                var error = ex;

                string message = error.Message;
                string inner = error.InnerException != null ? error.InnerException.Message : "";
                string stack = error.StackTrace;
                var membershipUser = Membership.GetUser();
                long userId = 0;
                if (membershipUser != null)
                {
                    var providerUserKey = membershipUser.ProviderUserKey;
                    if (providerUserKey != null)
                    {
                        userId = (long)providerUserKey;
                    }
                }
                var bdc = new BusidexDataContext();
                bdc.SaveApplicationError(message, inner, stack, userId);
            }

        }

        [ErrorLogger]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Add(AddOrEditCardModel model, int idx = 0)
        {
            if (Request.Form["showImagePreview"] == "true")
            {
                string img = ImagePreview(idx);
                return new ImagePreviewResult(img);
            }

            MembershipUser user = Membership.GetUser();
            long cardId = DEMO_CARD_ID;
            if (user != null && user.ProviderUserKey != null)
            {
                var bu = _cardRepository.GetBusidexUserById((long)user.ProviderUserKey);

                AddOrEditCardModel addedModel;
                if (Request.Form["SelectExistingCard"] != null)
                {
                    long.TryParse(Request.Form["SelectExistingCard"], out cardId);
                    addedModel = _cardRepository.GetAddOrEditModel(cardId, bu, "Add");
                }
                else
                {
                    model.Tags = model.Tags ?? new List<Tag>();
                    addedModel = AddOrEdit(model);
                }

                if (addedModel != null)
                {
                    cardId = addedModel.CardId;
                    if (addedModel.ModelErrors != null)
                    {
                        var errors = addedModel.ModelErrors as AddOrUpdateCardErrors ?? new AddOrUpdateCardErrors();
                        List<Card> existingCards = errors.ExistingCards;
                        var isMyCard = addedModel.IsMyCard;
                        addedModel = _cardRepository.GetAddOrEditModel(cardId, bu, "Add");
                        addedModel.IsMyCard = isMyCard;
                        addedModel.ExistingCards = existingCards;
                        addedModel.ModelErrors = errors;

                        foreach (var error in addedModel.ModelErrors.ErrorCollection.Keys)
                        {
                            ModelState.AddModelError(error, addedModel.ModelErrors.ErrorCollection[error]);
                        }
                    }
                    if (addedModel.CardId == DEMO_CARD_ID || !ModelState.IsValid)
                    {
                        return View(addedModel);
                    }
                    // Save the user's card to the web server
                    if (addedModel.Display == DisplayType.IMG)
                    {
                        CardToFile(cardId, addedModel.FrontImage != null, addedModel.BackImage != null);
                    }
                    _cardRepository.InvalidateBusidexCache();

                }
                else
                {
                    addedModel = _cardRepository.GetAddOrEditModel(cardId, bu, "Add");
                    return View(addedModel);
                }
            }

            return RedirectToAction("Mine", "Busidex");
        }

        [ErrorLogger]
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Edit(long id)
        {
            var user = Membership.GetUser();
            if (user != null && user.ProviderUserKey != null)
            {
                var userId = (long)user.ProviderUserKey;
                BusidexUser bu = _cardRepository.GetBusidexUserById(userId);

                AddOrEditCardModel model = _cardRepository.GetAddOrEditModel(id, bu, "Edit");
                UserCard uc = _cardRepository.GetUserCard(model.CardId, userId);
                model.Notes = uc.Notes;

                return View(model);
            }
            return null;
        }

        [ErrorLogger]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Edit(AddOrEditCardModel model, int idx = 0)
        {

            if (Request.Form["showImagePreview"] == "true")
            {
                string img = ImagePreview(idx);
                return new ImagePreviewResult(img);
            }

            AddOrEditCardModel editedModel = null;
            MembershipUser user = Membership.GetUser();
            if (user != null && user.ProviderUserKey != null)
            {
                var bu = _cardRepository.GetBusidexUserById((long)user.ProviderUserKey);

                if (Request.Form["SelectExistingCard"] != null)
                {
                    long cardId;
                    long.TryParse(Request.Form["SelectExistingCard"], out cardId);
                    editedModel = _cardRepository.GetAddOrEditModel(cardId, bu, "Edit");
                }
                else
                {
                    editedModel = AddOrEdit(model) ?? new AddOrEditCardModel();
                }
            }
            editedModel = _cardRepository.GetAddOrEditModel(editedModel);
            model.ActionMethod = "Edit";
            //var cardId = editedModel.CardId;

            if (model.ModelErrors != null && model.ModelErrors.ErrorCollection.Count > 0)
            {
                //MembershipUser user = Membership.GetUser();
                //var bu = _cardRepository.GetBusidexUserById((long)user.ProviderUserKey);

                var errors = editedModel.ModelErrors as AddOrUpdateCardErrors ?? new AddOrUpdateCardErrors();
                List<Card> existingCards = errors.ExistingCards;
                var isMyCard = editedModel.IsMyCard;
                //editedModel = _cardRepository.GetAddOrEditModel(cardId, bu, "Edit");
                editedModel.IsMyCard = isMyCard;
                editedModel.ExistingCards = existingCards;
                editedModel.ModelErrors = model.ModelErrors;

                foreach (var error in editedModel.ModelErrors.ErrorCollection.Keys)
                {
                    ModelState.AddModelError(error, editedModel.ModelErrors.ErrorCollection[error]);
                }
                return View(editedModel);
            }

            if (editedModel.Display == DisplayType.IMG)
            {
                CardToFile(editedModel.CardId, editedModel.FrontFileId == null, editedModel.BackFileId == null);
            }
            return RedirectToAction("Mine", "Busidex");
        }

        [ErrorLogger]
        public ActionResult Delete(long id)
        {
            var membershipUser = Membership.GetUser();

            if (membershipUser != null)
            {
                var providerUserKey = membershipUser.ProviderUserKey;
                if (providerUserKey != null)
                {
                    var userId = (long)providerUserKey;
                    _cardRepository.DeleteCard(id, userId);
                }
            }


            return RedirectToAction("MyCards");
        }

        [ErrorLogger]
        public ActionResult MyCards()
        {
            var myCards = new List<CardDetailModel>();
            var membershipUser = Membership.GetUser();

            if (membershipUser != null)
            {
                var providerUserKey = membershipUser.ProviderUserKey;
                if (providerUserKey != null)
                {
                    var userId = (long)providerUserKey;
                    myCards = _cardRepository.GetCardsByOwnerId(userId);
                }
            }
            return View(myCards);
        }

        [ErrorLogger]
        public ActionResult SendOwnerEmail()
        {
            var token = Guid.NewGuid();
            var template = _accountRepository.GetEmailTemplate(EmailTemplateCode.ConfirmOwner);
            string email = Request.Form["email"];
            long cardId = long.Parse(Request.Form["cardId"]);
            var sentBy = _accountRepository.GetBusidexUserById(GetUserId());
            
            template.Body = template.Body.Replace("###", token.ToString());

            var communication = new Communication
            {
                EmailTemplate = template,
                Email = email,
                DateSent = DateTime.UtcNow,
                UserId = 0,
                SentById = sentBy.UserId,
                OwnerToken = token,
                Failed = false,
                EmailTemplateId = template.EmailTemplateId
            };

            try
            {
                SendEmail(communication);
            }
            catch (Exception)
            {
                communication.Failed = true;
            }
            finally
            {
                _accountRepository.SaveCommunication(communication);

                _cardRepository.SaveCardOwnerToken(cardId, token);
            }

            return RedirectToAction("Invite", new { id = cardId });
        }

        private AddOrEditCardModel AddOrEdit(AddOrEditCardModel model)
        {
            var membershipUser = Membership.GetUser();

            if (membershipUser != null)
            {
                var providerUserKey = membershipUser.ProviderUserKey;
                if (providerUserKey != null)
                {
                    var userId = (long)providerUserKey;

                    bool edit = model.CardId > DEMO_CARD_ID;

                    try
                    {
                        if (ModelState.IsValid)
                        {
                            List<UserCard> myBusidex = _cardRepository.GetMyBusidex(userId, false);

                            Card card = (edit ? _cardRepository.GetCardById(model.CardId) : null) ?? new Card();
                            card.PhoneNumbers = card.PhoneNumbers ?? new List<PhoneNumber>();

                            bool hasFronImage = card.CardId > 1 && card.FrontImage != null && card.FrontImage.Length > 0;
                            bool hasBackImage = card.CardId > 1 && card.BackImage != null && card.BackImage.Length > 0;
                            model.HasFrontImage = hasFronImage;

                            model.FrontFileId = card.FrontFileId;
                            model.BackFileId = card.BackFileId;
                            model.OwnerId = card.OwnerId;
                            model.OwnerToken = card.OwnerToken;

                            bool replaceFrontImage = false;
                            bool replaceBackImage = false;

                            if (!edit)
                            {
                                model.CreatedBy = userId;
                            }

                            #region Get Front / Back images
                            byte[] frontBuf = null, backBuf = null;
                            string frontType = "";
                            string backType = "";
                            // get front image                            
                            HttpPostedFileBase frontImage = Request.Files["CardFrontImage"];
                            if (frontImage != null && frontImage.ContentLength > 0)
                            {
                                frontBuf = new byte[frontImage.InputStream.Length];
                                frontImage.InputStream.Read(frontBuf, 0, (int)frontImage.InputStream.Length);
                                var extension = Path.GetExtension(frontImage.FileName);
                                if (extension != null)
                                {
                                    frontType = extension.ToLower().Trim();
                                }
                                model.FrontFileId = null;
                                replaceFrontImage = true;
                            }

                            HttpPostedFileBase backImage = Request.Files["CardBackImage"];
                            if (backImage != null && backImage.ContentLength > 0)
                            {
                                backBuf = new byte[backImage.InputStream.Length];
                                backImage.InputStream.Read(backBuf, 0, (int)backImage.InputStream.Length);
                                var extension = Path.GetExtension(backImage.FileName);
                                if (extension != null)
                                {
                                    backType = extension.ToLower().Trim();
                                }
                                model.BackFileId = null;
                                replaceBackImage = true;
                            }


                            if (frontBuf != null && (frontBuf.Length > 0))
                            {
                                model.LoadImage(frontBuf, frontType, true);
                            }
                            else
                            {
                                if (!hasFronImage && string.IsNullOrEmpty(card.Markup))
                                {
                                    string key = CardContent.ErrorCardSelectCardImage.ToString();
                                    ModelState.AddModelError("FileError", ContentProvider.GetContent(key));
                                }
                                else
                                {
                                    model.FrontImage = card.FrontImage;
                                    model.FrontOrientation = card.FrontOrientation;
                                    model.FrontType = card.FrontType;
                                }
                            }

                            // get back image (if necessary)
                            if (backBuf != null && (backBuf.Length > 0))
                            {
                                model.LoadImage(backBuf, backType, false);
                            }
                            else
                            {
                                if (hasBackImage)
                                {
                                    model.BackImage = card.BackImage;
                                    model.BackOrientation = card.BackOrientation;
                                    model.BackType = card.BackType;
                                }
                            }
                            #endregion

                            if (TryUpdateModel(model))
                            {
                                if (model.IsMyCard == null)
                                {
                                    string key = CardContent.ErrorCardIndicateOwner.ToString();
                                    ModelState.AddModelError("FileError", ContentProvider.GetContent(key));
                                }
                                // clean up phone number field
                                #region Phone Numbers

                                if (Request.Form["PhoneNumberId"] != null)
                                {
                                    string[] sPhoneNumberIds = Request.Form["PhoneNumberId"].Split(',');
                                    string[] sPhoneNumbers = Request.Form["Number"].Split(',');
                                    string[] sExtensions = Request.Form["Extension"].Split(',');
                                    string[] sPhoneNumberTypes = Request.Form["PhoneNumberTypeId"].Split(',');
                                    string[] sPhoneDeleteds = Request.Form["PhoneDeleted"].Split(',');

                                    model.PhoneNumbers = GetPhoneNumbersFromForm(card, sPhoneNumberIds, sPhoneNumbers, sExtensions, sPhoneNumberTypes, sPhoneDeleteds);
                                }
                                #endregion

                                #region Tags

                                if (Request.Form["tagText"] != null)
                                {
                                    string[] sTags = Request.Form["tagText"].Split(',');
                                    sTags = sTags.Where(tag => !string.IsNullOrEmpty(tag)).ToArray();

                                    model.Tags = GetCardTagsFromForm(card, sTags);
                                }
                                #endregion

                                #region Addresses
                                if (Request.Form["CardAddressId"] != null)
                                {
                                    string[] sCardAddressIds = Request.Form["CardAddressId"].Split(',');
                                    string[] sAddressOnes = Request.Form["Address1"].Split(',');
                                    string[] sAddressTwos = Request.Form["Address2"].Split(',');
                                    string[] sCities = Request.Form["City"].Split(',');
                                    string[] sStates = Request.Form["State"].Split(',');
                                    string[] sZips = Request.Form["ZipCode"].Split(',');
                                    string[] sRegions = Request.Form["Region"].Split(',');
                                    string[] sCountries = Request.Form["Country"].Split(',');
                                    string[] sDeleted = Request.Form["AddressDeleted"].Split(',');

                                    //model.Addresses = GetCardAddressesFromForm(card, sCardAddressIds, sAddressOnes,
                                    //                                           sAddressTwos, sCities, sStates, sZips,
                                    //                                           sRegions, sCountries, sDeleted);
                                }
                                #endregion

                                #region Add the card
                                var cardId = model.CardId;
                                //model.ModelErrors = edit
                                //                        ? _cardRepository.EditCard(model, userId, myBusidex)
                                //                        : _cardRepository.AddCard(model, userId, myBusidex, out cardId);
                                #endregion

                                model.CardId = cardId;
                                if (replaceBackImage)
                                {
                                    model.BackFileId = null;
                                }
                                if (replaceFrontImage)
                                {
                                    model.FrontFileId = null;
                                }
                            }
                        }

                    }
                    catch (Exception e)
                    {
                        ModelState.AddModelError("UnknownError", e.Message);
                    }
                }
            }
            return model;
        }

        private List<PhoneNumber> GetPhoneNumbersFromForm(Card card,
                                                          string[] sPhoneNumberIds,
                                                          string[] sPhoneNumbers,
                                                          string[] sExtensions,
                                                          string[] sPhoneNumberTypes,
                                                          string[] sPhoneDeleteds)
        {
            var phoneNumbers = new List<PhoneNumber>();

            for (int i = 0; i < sPhoneNumberIds.Length; i++)
            {
                string sPhoneNumber = sPhoneNumberIds[i];
                bool deleted = !string.IsNullOrEmpty(sPhoneDeleteds[i]) &&
                               bool.Parse(sPhoneDeleteds[i]);

                long phoneNumberId = string.IsNullOrEmpty(sPhoneNumber) ? 0 : long.Parse(sPhoneNumber);

                if (phoneNumberId > 0 && card.PhoneNumbers.Any(p => p.PhoneNumberId == phoneNumberId))
                {
                    var phoneNumber = card.PhoneNumbers.Single(p => p.PhoneNumberId == phoneNumberId);
                    phoneNumber.Number = sPhoneNumbers[i];
                    phoneNumber.Extension = sExtensions[i];
                    phoneNumber.PhoneNumberTypeId = int.Parse(sPhoneNumberTypes[i]);
                    phoneNumber.Deleted = deleted;
                    if (!string.IsNullOrEmpty(phoneNumber.Number))
                    {
                        phoneNumbers.Add(phoneNumber);
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(sPhoneNumbers[i]))
                    {
                        var phoneNumber = new PhoneNumber
                                          {
                                              PhoneNumberId = phoneNumberId,
                                              Number = sPhoneNumbers[i],
                                              Extension = sExtensions[i],
                                              PhoneNumberTypeId = int.Parse(sPhoneNumberTypes[i]),
                                              Deleted = deleted,
                                              CardId = card.CardId,
                                              Created = DateTime.UtcNow
                                          };
                        phoneNumbers.Add(phoneNumber);
                    }
                }
            }

            return phoneNumbers;
        }

        private List<Tag> GetCardTagsFromForm(Card card, string[] sTags)
        {
            var tags = new List<Tag>();

            if (sTags.Length > 0)
            {
                // check for added tags
                var newTags = sTags.Where(t => !card.Tags.Any(tt => tt.Text.Equals(t))).ToList();
                tags.AddRange(newTags.Select(t => new Tag { Text = t }));

                // check for removed tags
                tags.AddRange(from tag in card.Tags
                              where sTags.All(t => t != tag.Text)
                              select new Tag { Deleted = true, TagId = tag.TagId, Text = tag.Text });
            }
            else
            {
                // all tags have been removed
                tags.AddRange(card.Tags.Select(tag => new Tag { Deleted = true, TagId = tag.TagId, Text = tag.Text }));
            }

            return tags;
        }

        //private List<CardAddress> GetCardAddressesFromForm(Card card, string[] sCardAddressIds, string[] sAddressOnes,
        //                                                   string[] sAddressTwos, string[] sCities, string[] sStates, string[] sZips,
        //                                                   string[] sRegions, string[] sCountries, string[] sDeleted)
        //{
        //    var addresses = new List<CardAddress>();

        //    for (var i = 0; i < sCardAddressIds.Length; i++)
        //    {
        //        string sCardAddressId = sCardAddressIds[i];

        //        long cardAddressId = string.IsNullOrEmpty(sCardAddressId) ? 0 : long.Parse(sCardAddressId);
        //        CardAddress address;
        //        if (cardAddressId > 0 && card.Addresses.Any(a => a.CardAddressId == cardAddressId))
        //        {
        //            address = card.Addresses.Single(a => a.CardAddressId == cardAddressId);
        //            address.Address1 = sAddressOnes[i];
        //            address.Address2 = sAddressTwos[i];
        //            address.City = sCities[i];
        //            address.State = sStates[i];
        //            address.ZipCode = sZips[i];
        //            address.Region = sRegions[i];
        //            address.Country = sCountries[i];
        //            address.Deleted = !string.IsNullOrEmpty(sDeleted[i]) && bool.Parse(sDeleted[i]);
        //        }
        //        else
        //        {
        //            address = new CardAddress
        //                                 {
        //                                     Address1 = sAddressOnes[i],
        //                                     Address2 = sAddressTwos[i],
        //                                     City = sCities[i],
        //                                     State = sStates[i],
        //                                     ZipCode = sZips[i],
        //                                     Region = sRegions[i],
        //                                     Country = sCountries[i]
        //                                 };

        //        }

        //        TryToGetCoordinatesFromAddress(address);

        //        if (!string.IsNullOrEmpty(address.ToString()) || address.Deleted)
        //        {
        //            addresses.Add(address);
        //        }
        //    }
        //    return addresses;
        //}

        private void TryToGetCoordinatesFromAddress(CardAddress address)
        {
            string key = ConfigurationManager.AppSettings["BingMapsKey"];

            string url = string.Format(BING_URL, address, key);
            var response = MakeRequest(url);
            if (response != null)
            {
                var coordinates = ProcessResponse(response);

                // if we got double zeros, take one more shot by trying just address 2
                if (coordinates.Item1.Equals(0) && coordinates.Item2.Equals(0))
                {
                    var tmpAddress = new CardAddress
                    {
                        Address1 = string.Empty,
                        Address2 = address.Address2,
                        City = address.City,
                        State = address.State,
                        ZipCode = address.ZipCode,
                        Region = address.Region,
                        Country = address.Country
                    };
                    url = string.Format(BING_URL, tmpAddress, key);
                    response = MakeRequest(url);
                    coordinates = ProcessResponse(response);
                }
                // if we got double zeros, try again by trying just address 1
                if (coordinates.Item1.Equals(0) && coordinates.Item2.Equals(0))
                {
                    var tmpAddress = new CardAddress
                    {
                        Address1 = address.Address1,
                        Address2 = string.Empty,
                        City = address.City,
                        State = address.State,
                        ZipCode = address.ZipCode,
                        Region = address.Region,
                        Country = address.Country
                    };
                    url = string.Format(BING_URL, tmpAddress, key);
                    response = MakeRequest(url);
                    coordinates = ProcessResponse(response);
                }

                // if we got double zeros, try getting just the City
                if (coordinates.Item1.Equals(0) && coordinates.Item2.Equals(0))
                {
                    var tmpAddress = new CardAddress
                    {
                        Address1 = string.Empty,
                        Address2 = string.Empty,
                        City = address.City,
                        State = address.State,
                        ZipCode = address.ZipCode,
                        Region = address.Region,
                        Country = address.Country
                    };
                    url = string.Format(BING_URL, tmpAddress, key);
                    response = MakeRequest(url);
                    coordinates = ProcessResponse(response);
                }

                // if we got double zeros, take one more shot by trying just state
                if (coordinates.Item1.Equals(0) && coordinates.Item2.Equals(0))
                {
                    var tmpAddress = new CardAddress
                    {
                        Address1 = string.Empty,
                        Address2 = string.Empty,
                        City = string.Empty,
                        State = address.State,
                        ZipCode = address.ZipCode,
                        Region = address.Region,
                        Country = address.Country
                    };
                    url = string.Format(BING_URL, tmpAddress, key);
                    response = MakeRequest(url);
                    coordinates = ProcessResponse(response);
                }
                address.Latitude = coordinates.Item1;
                address.Longitude = coordinates.Item2;
            }
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public long GetFileSize(int rand)
        {
            byte[] fileData = null;
            var testFile = Request.Files[0];
            if (testFile != null)
            {
                var postedFileBase = Request.Files[0];
                if (postedFileBase != null)
                    using (var binaryReader = new BinaryReader(postedFileBase.InputStream))
                    {
                        var httpPostedFileBase = postedFileBase;
                        fileData = binaryReader.ReadBytes(httpPostedFileBase.ContentLength);
                    }
            }

            return fileData != null ? fileData.Length : 0;
        }

        [ErrorLogger]
        [AcceptVerbs(HttpVerbs.Post)]
        public void AddToMyBusidex(long id)
        {
            var membershipUser = Membership.GetUser();
            if (membershipUser != null)
            {
                var providerUserKey = membershipUser.ProviderUserKey;
                if (providerUserKey != null)
                {
                    var userId = (long)providerUserKey;
                    if (HttpContext != null) _cardRepository.AddToMyBusidex(id, userId);
                }
            }
        }

        [ErrorLogger]
        [AllowAnonymous]
        public void AddToMyBusidexMobile(long userId, long cardId)
        {
            if (HttpContext != null) _cardRepository.AddToMyBusidex(cardId, userId);
        }

        [ErrorLogger]
        [AcceptVerbs(HttpVerbs.Get)]
        public void DeleteUserCard(long id)
        {
            var membershipUser = Membership.GetUser();
            if (membershipUser != null)
            {
                var providerUserKey = membershipUser.ProviderUserKey;
                if (providerUserKey != null)
                {
                    var userId = (long)providerUserKey;
                    UserCard uc = _cardRepository.GetUserCard(id, userId);
                    if (uc != null)
                    {
                        _cardRepository.DeleteUserCard(uc, userId);
                    }
                }
            }
        }

        [ErrorLogger]
        [AllowAnonymous]
        public ActionResult Details(long? id = 0)
        {
            if (id.GetValueOrDefault() == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            Card c = _cardRepository.GetCardById(id.GetValueOrDefault());
            var user = Membership.GetUser();
            if (!c.Searchable)
            {
                if (user == null) // user is not logged in and card is not searchable
                {
                    return RedirectToAction("Index", "Home");
                }
                if (user.ProviderUserKey != null)
                {
                    var myBusidex = _cardRepository.GetMyBusidex((long)user.ProviderUserKey, false);
                    if (myBusidex.All(b => b.CardId != id)) // user is logged in, but this isn't one of their cards
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
            }
            var model = new CardDetailModel(c);
            return View(model);
        }

        [ErrorLogger]
        public ActionResult SaveCardOwner(string token)
        {
            CardDetailModel model = _cardRepository.GetCardByToken(token);
            model.IsUserLoggedIn = false;

            var membershipUser = Membership.GetUser();
            if (membershipUser != null)
            {
                model.IsUserLoggedIn = true;

                if (membershipUser.ProviderUserKey != null)
                {
                    var userId = (long)membershipUser.ProviderUserKey;
                    _cardRepository.SaveCardOwner(model.CardId, userId);
                }
            }

            return RedirectToAction("MyCards");
        }

        [ErrorLogger]
        public ActionResult ConfirmMyCard(string token)
        {
            CardDetailModel model = _cardRepository.GetCardByToken(token);
            model.IsUserLoggedIn = false;

            var membershipUser = Membership.GetUser();
            if (membershipUser != null)
            {
                model.IsUserLoggedIn = true;

                if (membershipUser.ProviderUserKey != null)
                {
                    var userId = (long)membershipUser.ProviderUserKey;
                    var ownerId = model.OwnerId.GetValueOrDefault();
                    model.IsMyCard = ownerId == userId;

                    if (!model.IsMyCard)
                    {
                        if (ownerId > 0)
                        {
                            // This card belongs to someone else. Strange. How did they get this link?
                            string key = CardContent.ErrorCardOwned.ToString();
                            model.ModelErrors.ErrorCollection.Add("PreviousCardOwner", ContentProvider.GetContent(key));
                        }
                    }
                    else
                    {
                        // This card is already owned by this person. Did they click the Confirm link twice? 
                        return RedirectToAction("Edit", new { id = model.CardId });
                    }
                }

                //TODO: In the future, check user.UserAccount.AccountType to see if they have a paid account
                //busidex_User user = _accountRepository.GetBusidexUserById(userId);
            }

            return View(model);
        }

        #region Search

        [ErrorLogger]
        [HttpGet]
        [AllowAnonymous]
        public ActionResult Search()
        {
            var searchModel = new SearchResultModel
            {
                Display = Busidex.DAL.ViewType.List,
                Results = new List<CardDetailModel>()
            };
            return View(searchModel);
        }

        [ErrorLogger]
        public ActionResult Invite(long id)
        {
            Card model = _cardRepository.GetCardById(id);
            return View(model);
        }

        [ErrorLogger]
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Search(SearchResultModel model)
        {

            GetBaseViewData();

            var user = Membership.GetUser();
            long userId = 0;
            if (user != null && user.ProviderUserKey != null)
            {
                userId = (long)user.ProviderUserKey;
            }
            Tuple<double, double> altAddress = null;
            //string key = ConfigurationManager.AppSettings["BingMapsKey"];
            //string url = string.Format(BING_URL, model.SearchAddress, key);
            //var response = MakeRequest(url);
            //if (response != null)
            //{
            //    var coordinates = ProcessResponse(response);
            //    altAddress = new Tuple<double, double>(coordinates.Item1, coordinates.Item2);
            //}

            SearchResultModel searchModel = _cardRepository.Search(model, userId, altAddress);
            searchModel.IsLoggedIn = User.Identity.IsAuthenticated;

            var allCards = searchModel.Results.Select(card => card).ToList();
            List<string> allTags = (from cards in allCards
                                    from tag in cards.Tags
                                    select tag.Text).ToList();

            var tags = (from tag in allTags
                        group tag by tag into t
                        select new { key = t.First(), Value = t.Count() })
                .ToDictionary(t => t.key, t => t.Value);

            searchModel.TagCloud = tags;

            return View(searchModel);
        }

        [AllowAnonymous]
        public string SearchJSON(string criteria, long userId)
        {
            Response.AppendHeader("Access-Control-Allow-Origin", "*");

            var model = new SearchResultModel { SearchText = criteria };
            var results = _cardRepository.Search(model, userId, null);

            if (results.HasResults)
            {
                var cards = (from card in results.Results
                             select new MobileCardSmall
                                        {
                                            FrontFileId = card.FrontFileId != null ? card.FrontFileId.ToString() + "." + card.FrontFileType : string.Empty,
                                            Name = card.Name ?? string.Empty,
                                            Email = card.Email ?? string.Empty,
                                            TagList = string.IsNullOrEmpty(card.TagList) ? "." : card.TagList.ToLower(),
                                            Company = card.CompanyName.Trim(),
                                            PhoneNumbers = new List<string>(from p in card.PhoneNumbers select !string.IsNullOrEmpty(p.Number) ? p.Number : string.Empty),
                                            OwnerId = card.OwnerId
                                        }).ToList()
                                        .OrderByDescending(c => c.OwnerId.GetValueOrDefault()).ThenBy(c => c.Company).ThenBy(c => c.Name).ToList().ToList();

                return JsonConvert.SerializeObject(cards);
            }

            return string.Empty;

        }
        #endregion

        [ErrorLogger]
        public ActionResult Create(long? id = -1)
        {
            var user = Membership.GetUser();
            var model = new AddOrEditCardModel
                        {
                            Markup = string.Empty, 
                            Display = DisplayType.HTM
                        };

            if (id.GetValueOrDefault() > 1)
            {
                if (user != null && user.ProviderUserKey != null)
                {
                    var userId = (long)user.ProviderUserKey;
                    BusidexUser bu = _cardRepository.GetBusidexUserById(userId);

                    model = _cardRepository.GetAddOrEditModel(id.GetValueOrDefault(), bu, "Edit");
                    if (string.IsNullOrEmpty(model.Markup))
                    {
                        model.Markup = string.Empty;
                    }
                }
            }
            return View(model);
        }

        [ErrorLogger]
        public long SaveCardMarkup(long cardId, string markup, string company, string name, string title, string email, string phone1, string phone2)
        {
            var html = Server.HtmlDecode(markup);
            if (!string.IsNullOrEmpty(html))
            {

                html = html.Trim();

                var membershipUser = Membership.GetUser();

                if (membershipUser != null)
                {
                    var providerUserKey = membershipUser.ProviderUserKey;
                    if (providerUserKey != null)
                    {
                        var userId = (long)providerUserKey;
                        var bu = _accountRepository.GetBusidexUserById(userId);
                        var model = cardId > 1 ? _cardRepository.GetAddOrEditModel(cardId, bu, "Edit") : new AddOrEditCardModel();

                        List<UserCard> myBusidex = _cardRepository.GetMyBusidex(userId, false);

                        model.CardId = cardId;
                        model.Display = DisplayType.HTM;
                        model.Markup = html;
                        model.CompanyName = company;
                        model.Name = name;
                        model.Title = title;
                        model.Email = email;
                        model.PhoneNumbers = model.PhoneNumbers ?? new List<PhoneNumber>();
                        model.Addresses = model.Addresses ?? new List<CardAddress>();
                        model.FrontOrientation = model.BackOrientation = "H";
                        model.OwnerId = userId;
                        model.IsMyCard = true;
                        model.Searchable = true;
                        model.CreatedBy = userId;
                        model.Created = DateTime.Now;

                        #region Phone 1

                        if (!string.IsNullOrEmpty(phone1))
                        {
                            if (model.PhoneNumbers.Count == 0)
                            {
                                model.PhoneNumbers.Add(new PhoneNumber
                                                           {
                                                               Number = phone1,
                                                               PhoneNumberTypeId = 1
                                                           });
                            }
                            else
                            {
                                model.PhoneNumbers[0].Number = phone1;
                            }
                        }
                        #endregion

                        #region Phone 2

                        if (!string.IsNullOrEmpty(phone2) && model.PhoneNumbers.All(p => p.Number != phone2))
                        {
                            if (model.PhoneNumbers.Count == 1)
                            {
                                model.PhoneNumbers.Add(new PhoneNumber
                                                           {
                                                               Number = phone2,
                                                               PhoneNumberTypeId = 1
                                                           });
                            }
                            else if (model.PhoneNumbers.Count > 1)
                            {
                                model.PhoneNumbers[1].Number = phone2;
                            }
                        }
                        #endregion
                        model.Tags = model.Tags ?? new List<Tag>();

                        if (model.CardId > 1)
                        {
                           // _cardRepository.EditCard(model, userId);
                        }
                        else
                        {
                           // _cardRepository.AddCard(model, userId, myBusidex, out cardId);
                        }
                    }
                }
            }
            return cardId;
        }

        [ErrorLogger]
        public ActionResult AddSharedCardsToMyBusidex()
        {
            var acceptedIds = Request.Form["cardIdAccept"];
            var declinedIds = Request.Form["cardIdDecline"];
            string[] sAcceptedCardIds = acceptedIds != null ? acceptedIds.Split(',') : new string[0];
            var acceptedCardIds = sAcceptedCardIds.Select(long.Parse).ToList();
            string[] sDeclinedCardIds = declinedIds != null ? declinedIds.Split(',') : new string[0];
            var declinedCardIds = sDeclinedCardIds.Select(long.Parse).ToList();
            var user = Membership.GetUser();
            if (user != null && user.ProviderUserKey != null)
            {
                var userId = (long)user.ProviderUserKey;

                foreach (var cardId in acceptedCardIds)
                {
                    _cardRepository.AcceptSharedCard(cardId, userId);
                }

                foreach (var cardId in declinedCardIds)
                {
                    _cardRepository.DeclineSharedCard(cardId, userId);
                }
                _cardRepository.InvalidateBusidexCache();
            }
            return RedirectToAction("Mine", "Busidex");
        }

        private Image ScaleImage(Image image, int maxWidth, int maxHeight)
        {
            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            var newImage = new Bitmap(newWidth, newHeight);
            Graphics.FromImage(newImage).DrawImage(image, 0, 0, newWidth, newHeight);
            return newImage;
        }

        private void SaveThumbnail(byte[] cardImage, string orientation, string fileName, string fileType)
        {
            var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["StorageConnection"].ConnectionString);
            var blobStorage = storageAccount.CreateCloudBlobClient();
            var container = blobStorage.GetContainerReference("http://busidexcdn.blob.core.windows.net/mobile-images");

            string uniqueBlobName = string.Format("{0}.{1}", fileName, fileType);
            var frontImageBlob = container.GetBlockBlobReference(uniqueBlobName);

            int maxHeight = orientation == "H" ? 95 : 130;
            int maxWidth = orientation == "H" ? 130 : 95;
            Image img = new Bitmap(new MemoryStream(cardImage));
            var scaledImage = ScaleImage(img, maxWidth, maxHeight);

            using (var memoryStream = new MemoryStream())
            {
                scaledImage.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);

                memoryStream.Seek(0, SeekOrigin.Begin);
                frontImageBlob.UploadFromStream(memoryStream);
            }

        }

        protected void CardToFile(long cardId, bool replaceFront = false, bool replaceBack = false)
        {

            var card = _cardRepository.GetCardById(cardId);
            var defaultCard = _cardRepository.GetCardById(DEMO_CARD_ID);

            if (card != null)
            {
                #region Update file name in DB

                var frontGuid = card.FrontFileId ?? defaultCard.FrontFileId.GetValueOrDefault();
                if (replaceFront) frontGuid = Guid.NewGuid();

                card.FrontFileId = frontGuid;

                var backGuid = card.BackFileId ?? defaultCard.BackFileId.GetValueOrDefault();
                if (replaceBack)
                {
                    backGuid = Guid.NewGuid();
                    card.BackFileId = backGuid;
                }

                if (card.BackImage == null)
                {
                    backGuid = defaultCard.BackFileId.GetValueOrDefault();
                }

                _cardRepository.UpdateCardFileId(cardId, frontGuid, backGuid);

                #endregion

                #region Save card to file system

                var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["StorageConnection"].ConnectionString);
                var blobStorage = storageAccount.CreateCloudBlobClient();
                var container = blobStorage.GetContainerReference(ConfigurationManager.AppSettings["BlobStorageContainer"]);

                string uniqueBlobName = string.Format("{0}.{1}", card.FrontFileId, card.FrontType);
                var frontImageBlob = container.GetBlockBlobReference(uniqueBlobName);
                frontImageBlob.Properties.ContentType = _mimeTypes[card.FrontType];
                frontImageBlob.Properties.CacheControl = "public, max-age=31536000";
                frontImageBlob.UploadFromStream(new MemoryStream(card.FrontImage.ToArray()));

                SaveThumbnail(card.FrontImage.ToArray(), card.FrontOrientation, card.FrontFileId.GetValueOrDefault().ToString(), card.FrontType);

                if (replaceBack && card.BackImage != null)
                {
                    uniqueBlobName = string.Format("{0}.{1}", card.BackFileId, card.BackType);
                    var backImageBlob = container.GetBlockBlobReference(uniqueBlobName);
                    backImageBlob.Properties.ContentType = _mimeTypes[card.BackType];
                    backImageBlob.Properties.CacheControl = "public, max-age=31536000";
                    backImageBlob.UploadFromStream(new MemoryStream(card.BackImage.ToArray()));

                    SaveThumbnail(card.FrontImage.ToArray(), card.FrontOrientation, card.BackFileId.GetValueOrDefault().ToString(), card.FrontType);
                }

                #endregion
            }
        }


        //private void SendConfirmationEmail(long cardId, string email) {
        //    var section = (NameValueCollection)ConfigurationManager.GetSection("emailInfo");

        //    var token = Guid.NewGuid();
        //    _cardRepository.SaveCardOwnerToken(cardId, token);

        //    string body = section["RegistrationBody"].Replace("###", token.ToString());
        //    string subject = section["RegistrationSubject"];

        //    var to = new MailAddress(email);
        //    var message = new MailMessage();
        //    message.To.Add(to);
        //    message.IsBodyHtml = true;
        //    message.Subject = subject;
        //    message.Body = body;

        //    var smtp = new SmtpClient();

        //    smtp.Send(message);
        //}


    }

}
