using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;
using Busidex.Api.DataServices.Interfaces;
using Busidex.Api.HttpPipeline;
using Busidex.Api.Models;
using Busidex.Api.DataAccess.DTO;
using log4net;
using Microsoft.ServiceBus.Messaging;
using BusidexUser = Busidex.Api.DataAccess.DTO.BusidexUser;
using System.Configuration;

namespace Busidex.Api.Controllers {
	[RequireHttps]
	[EnableCors(origins: "*", headers: "*", methods: "*")]
	[ApiExceptionFilter]
	public class CardController : BaseApiController {

		private static ILog _log;
		private readonly IAccountRepository _accountRepository;
		private static readonly JavaScriptSerializer Serializer = new JavaScriptSerializer();
		const string OLD_DEFAULT_CARD_FILE_ID = "B66FF0EE-E67A-4BBC-AF3B-920CD0DE56C6";
		private readonly string cardUpdateStorageConnectionString;
		public CardController(ICardRepository cardRepository, IAccountRepository accountRepository)
		{
			_cardRepository = cardRepository;
			_accountRepository = accountRepository;
			_log = _log ?? LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
			cardUpdateStorageConnectionString = ConfigurationManager.AppSettings["BusidexQueuesConnectionString"];
		}

		[System.Web.Http.HttpGet]
		public HttpResponseMessage GetCardCount()
		{
			var count = _cardRepository.GetCardCount();

			return new HttpResponseMessage
			{
				Content = new JsonContent(new
				{
					Success = true,
					Count = count
				}),
				StatusCode = HttpStatusCode.OK
			};
		}

		//[System.Web.Http.HttpGet]
		//public HttpResponseMessage GetFeaturedCard()
		//{
		//    try
		//    {
		//        var card = _cardRepository.GetFeaturedCard();

		//        return new HttpResponseMessage
		//        {
		//            Content = new JsonContent(new
		//            {
		//                FeaturedCard = card,
		//                Success = false
		//            }),
		//            StatusCode = HttpStatusCode.OK
		//        };

		//    }
		//    catch (Exception ex)
		//    {
		//        _cardRepository.SaveApplicationError(ex, 0);
		//        return new HttpResponseMessage
		//        {
		//            Content = new JsonContent(new
		//            {
		//                Success = false
		//            }),
		//            StatusCode = HttpStatusCode.InternalServerError
		//        };
		//    }
		//}

		[System.Web.Http.HttpGet]
		public HttpResponseMessage Details(long? id)
		{
			long userId = ValidateUser();

			try
			{
				if (id.GetValueOrDefault() == 0)
				{
					return new HttpResponseMessage
					{
						Content = new JsonContent(new
						{
							Success = false,
							ReasonPhrase = "Id is : " + id.GetValueOrDefault()
						}),
						StatusCode = HttpStatusCode.NotFound,
						ReasonPhrase = "Id is : " + id.GetValueOrDefault()
					};
				}

				var card = _cardRepository.GetCardById(id.GetValueOrDefault(), userId);
				//var myBusidex = _cardRepository.GetMyBusidex(userId, false);


				if (card == null)
				{
					return new HttpResponseMessage
					{
						Content = new JsonContent(new
						{
							Success = false,
							Reason = "Card is null"
						}),
						StatusCode = HttpStatusCode.NotFound,
						ReasonPhrase = "Card is null"
					};
				}

				card.Tags.AddRange(_accountRepository.GetUserAccountTags(card.OwnerId.GetValueOrDefault()));

				var model = new CardDetailModel(card);
				//model.ExistsInMyBusidex = myBusidex.Any(b => b.CardId == card.CardId);

				return new HttpResponseMessage
				{
					Content = new JsonContent(new
					{
						Success = true,
						Model = model
					}),
					StatusCode = HttpStatusCode.OK
				};
			} catch (Exception ex)
			{
				_cardRepository.SaveApplicationError(ex, userId);
				return new HttpResponseMessage
				{
					Content = new JsonContent(new
					{
						Success = false
					}),
					StatusCode = HttpStatusCode.InternalServerError
				};
			}
		}

		// GET api/<controller>
		[System.Web.Http.HttpGet]
		public HttpResponseMessage Get(long? id = 0, long? userId = 0)
		{
			userId = ValidateUser();
			if (userId <= 0)
			{
				return new HttpResponseMessage
				{
					Content = new JsonContent(new
					{
						Success = false
					}),
					StatusCode = HttpStatusCode.Unauthorized
				};
			}

			try
			{
				if (id.GetValueOrDefault() == 0)
				{
					var card = _cardRepository.GetCardsByOwnerId(userId.GetValueOrDefault()).FirstOrDefault();
					if (card == null)
					{
						return new HttpResponseMessage
						{
							Content = new JsonContent(new
							{
								Success = false
							}),
							StatusCode = HttpStatusCode.NotFound
						};
					}

					id = card.CardId;
				}

				BusidexUser bu = _cardRepository.GetBusidexUserById(userId.GetValueOrDefault());
				if (bu == null)
				{
					return new HttpResponseMessage
					{
						Content = new JsonContent(new
						{
							Success = false
						}),
						StatusCode = HttpStatusCode.Unauthorized
					};
				}

				AddOrEditCardModel model = _cardRepository.GetAddOrEditModel(id.GetValueOrDefault(), bu, "Add");

				var accountTags = _accountRepository.GetUserAccountTags(userId.GetValueOrDefault());

				model.Tags.AddRange(
					accountTags.Where(at => model.Tags.All(t => at.Text != t.Text))
					);

				model.FrontImage = null;
				model.BackImage = null;
				model.MyBusidex = null;

				model.MyEmail = bu.Email;
				model.CreatedBy = bu.UserId;

				var response = new HttpResponseMessage
				{
					Content = new JsonContent(new
					{
						Success = true,
						Model = model
					})
				};
				//response.Headers.Add("Access-Control-Allow-Origin", "*");
				return response;
			} catch (Exception ex)
			{
				_cardRepository.SaveApplicationError(ex, userId.GetValueOrDefault());
				return new HttpResponseMessage
				{
					Content = new JsonContent(new
					{
						Success = false
					}),
					StatusCode = HttpStatusCode.InternalServerError
				};
			}
		}


		//private void ExtractTextFromImage()
		//{
		//    var _MODIDocument = new MODI.Document();
		//    _MODIDocument.Create("temp.txt");
		//    _MODIDocument.OCR(MODI.MiLANGUAGES.miLANG_ENGLISH, true, true);

		//    int numOfCharacters = 0;
		//    int charactersHeights = 0;

		//    MODI.Image image = new ImageClass(); // (MODI.Image)_MODIDocument.Images[i];

		//    MODI.Layout layout = image.Layout;
		//    // getting the page's words
		//    for (int j = 0; j < layout.Words.Count; j++)
		//    {
		//        MODI.Word word = (MODI.Word)layout.Words[j];
		//        // getting the word's characters
		//        for (int k = 0; k < word.Rects.Count; k++)
		//        {
		//            MODI.MiRect rect = (MODI.MiRect)word.Rects[k];
		//            charactersHeights += rect.Bottom - rect.Top;
		//            numOfCharacters++;
		//        }
		//    }
		//}

		private HttpStatusCode ValidateImagePost(long userId)
		{
			var myCard = _cardRepository.GetCardsByOwnerId(userId).FirstOrDefault();
			if (myCard == null)
			{
				return HttpStatusCode.NotFound;
			}

			// Basic accounts can't have a card
			var account = _accountRepository.GetUserAccountByUserId(userId);
			if (account == null || account.AccountTypeId == (int)Models.AccountType.Basic)
			{
				return HttpStatusCode.Forbidden;
			}

			//if (request.Files.Count == 0 || request.Files[0].ContentLength == 0)
			//{
			//    return HttpStatusCode.BadRequest;
			//}

			return HttpStatusCode.OK;
		}

        [System.Web.Http.HttpPut]
        public HttpResponseMessage ConfirmCardOwner(long cardId, long ownerId)
        {
            var userId = ValidateUser();
            //var buser = _accountRepository.GetBusidexUserById(userId);
            //var isAdmin = Roles.IsUserInRole(buser.UserName, "Administrator");

            if (userId != ownerId)
            {
                return new HttpResponseMessage
                {
                    Content = new JsonContent(new { Success = false }),
                    StatusCode = HttpStatusCode.Unauthorized
                };
            }

            var ok = _cardRepository.SaveCardOwner(cardId, ownerId);

            return new HttpResponseMessage
            {
                Content = new JsonContent(new { Success = ok }),
                StatusCode = ok ? HttpStatusCode.OK : HttpStatusCode.NotModified
            };
        }

		[System.Web.Http.HttpPut]
		public HttpResponseMessage SaveMobileCardImage([FromBody] MobileCardImage card)
		{
			var userId = ValidateUser();
			try
			{
				#region Authorization

				if (userId <= 0)
				{
					return new HttpResponseMessage
					{
						Content = new JsonContent(new { Success = false }),
						StatusCode = HttpStatusCode.Unauthorized
					};
				}

				#endregion

				if (!ModelState.IsValid)
				{
					return new HttpResponseMessage
					{
						Content = new JsonContent(new { Success = false }),
						StatusCode = HttpStatusCode.BadRequest
					};
				}
				bool useFront = card.Side == MobileCardImage.SideIndex.Front;

				var myCard = _cardRepository.GetCardsByOwnerId(userId).FirstOrDefault() ?? new CardDetailModel();

				#region Reset Card Image

				var reset = false;
				if (useFront && card.FrontFileId == Guid.Empty)
				{
					_cardRepository.UpdateCardFileId(myCard.CardId, Guid.Empty, myCard.FrontType,
						myCard.BackFileId.GetValueOrDefault(), myCard.BackType);
					reset = true;
				}
				if (!useFront && card.BackFileId == Guid.Empty)
				{
					_cardRepository.UpdateCardFileId(myCard.CardId, myCard.FrontFileId.GetValueOrDefault(), myCard.FrontType,
						Guid.Empty, myCard.BackType);
					reset = true;
				}
				if (reset)
				{
					return new HttpResponseMessage
					{
						Content = new JsonContent(new { Success = true }),
						StatusCode = HttpStatusCode.OK
					};
				}
				#endregion

				var editModel = new AddOrEditCardModel(myCard)
				{
					CardId = myCard.CardId,
					OwnerId = myCard.OwnerId,
					Display = DisplayType.IMG,
					UserId = userId,
					Action = AddOrEditCardModel.CardAction.ImageOnly,
					FrontFileId = useFront ? card.FrontFileId : myCard.FrontFileId,
					BackFileId = useFront ? myCard.BackFileId : card.BackFileId,
					FrontOrientation = useFront ? card.Orientation : myCard.FrontOrientation,
					BackOrientation = useFront ? myCard.BackOrientation : card.Orientation
				};

				if (!string.IsNullOrEmpty(card.EncodedCardImage))
				{
					var imageBytes = Convert.FromBase64String(card.EncodedCardImage);
					if (imageBytes.Length == 0)
					{
						return new HttpResponseMessage
						{
							Content = new JsonContent(new { Success = false }),
							StatusCode = HttpStatusCode.BadRequest
						};
					}

					const string EXTENSION = "jpg";
					if (useFront)
					{
						editModel.FrontFileId = editModel.FrontFileId == Guid.Empty || editModel.FrontFileId.GetValueOrDefault().ToString() == OLD_DEFAULT_CARD_FILE_ID
							? Guid.NewGuid()
							: editModel.FrontFileId ?? Guid.NewGuid();
						editModel.BackFileId = myCard.BackFileId;
						editModel.BackType = string.IsNullOrEmpty(myCard.BackType) ? EXTENSION : myCard.BackType;
						editModel.FrontImage = imageBytes;
						editModel.FrontType = EXTENSION;
						editModel.UpdateFrontImage = true;
					} else
					{
						editModel.BackFileId = editModel.BackFileId == Guid.Empty || editModel.BackFileId.GetValueOrDefault().ToString() == OLD_DEFAULT_CARD_FILE_ID
							? Guid.NewGuid()
							: editModel.BackFileId ?? Guid.NewGuid();
						editModel.FrontFileId = myCard.FrontFileId;
						editModel.FrontType = myCard.FrontType;
						editModel.BackImage = imageBytes;
						editModel.BackType = EXTENSION;
						editModel.UpdateBackImage = true;
					}
				} else
				{
					editModel.UpdateBackImage = editModel.UpdateFrontImage = false;
				}

				var cardRef = Guid.NewGuid().ToString();
				_cardRepository.UploadCardUpdateToBlobStorage(editModel, cardUpdateStorageConnectionString, cardRef);
				_cardRepository.AddCardToQueue(cardUpdateStorageConnectionString, cardRef);

				return new HttpResponseMessage
				{
					Content = new JsonContent(new { Success = true }),
					StatusCode = HttpStatusCode.OK
				};
			} catch (Exception ex)
			{
				_cardRepository.SaveApplicationError(ex, userId);
				return new HttpResponseMessage
				{
					Content = new JsonContent(new { Success = false }),
					StatusCode = HttpStatusCode.InternalServerError
				};
			}
		}

		[System.Web.Http.HttpPut]
		public HttpResponseMessage SaveCardImage(int idx, string imageUrl, string orientation)
		{
			//const int MAX_IMAGE_SIZE = 1024 * 100;

			var userId = ValidateUser();
			try
			{
				#region Authorization
				if (userId <= 0)
				{
					return new HttpResponseMessage
					{
						Content = new JsonContent(new { Success = false }),
						StatusCode = HttpStatusCode.Unauthorized
					};
				}
				#endregion

				#region validation
				var status = ValidateImagePost(userId);
				if (status != HttpStatusCode.OK)
				{
					return new HttpResponseMessage
					{
						Content = new JsonContent(new { Success = false }),
						StatusCode = status
					};
				}
				#endregion

				bool useFront = idx == 0;
				var myCard = _cardRepository.GetCardsByOwnerId(userId).FirstOrDefault() ?? new CardDetailModel();

				var editModel = new AddOrEditCardModel(myCard)
				{
					CardId = myCard.CardId,
					OwnerId = myCard.OwnerId,
					Display = DisplayType.IMG,
					UserId = userId,
					Action = AddOrEditCardModel.CardAction.ImageOnly,
					FrontOrientation = useFront ? orientation : myCard.FrontOrientation,
					BackOrientation = useFront ? myCard.BackOrientation : orientation
				};

				if (!string.IsNullOrEmpty(imageUrl) && imageUrl != "undefined")
				{
					var imageBytes = GetImageFromStream(HttpUtility.UrlDecode(imageUrl));
					if (imageBytes == null || imageBytes.Length == 0)
					{
						return new HttpResponseMessage
						{
							Content = new JsonContent(new { Success = false }),
							StatusCode = HttpStatusCode.BadRequest
						};
					}

					var extension = "jpg";// FIXME: don't hardcode this.  Path.GetExtension(image.FileName);
					if (useFront)
					{
						//editModel.FrontFileId = editModel.FrontFileId == Guid.Empty || editModel.FrontFileId.GetValueOrDefault().ToString() == OLD_DEFAULT_CARD_FILE_ID
						//    ? Guid.NewGuid()
						//    : editModel.FrontFileId ?? Guid.NewGuid();
						editModel.FrontFileId = Guid.NewGuid();
						editModel.BackFileId = myCard.BackFileId;
						editModel.BackType = string.IsNullOrEmpty(myCard.BackType) ? "jpg" : myCard.BackType;
						editModel.FrontImage = imageBytes;
						editModel.FrontType = extension.ToLower().Replace(".", "").Trim();
						editModel.UpdateFrontImage = true;
					} else
					{
						//editModel.BackFileId = editModel.BackFileId == Guid.Empty || editModel.BackFileId.GetValueOrDefault().ToString() == OLD_DEFAULT_CARD_FILE_ID
						//    ? Guid.NewGuid()
						//    : editModel.BackFileId ?? Guid.NewGuid();
						editModel.BackFileId = Guid.NewGuid();
						editModel.FrontFileId = myCard.FrontFileId;
						editModel.FrontType = myCard.FrontType;
						editModel.BackImage = imageBytes;
						editModel.BackType = extension.ToLower().Replace(".", "").Trim();
						if (string.IsNullOrEmpty(editModel.BackType))
						{
							editModel.BackType = "jpg";
						}
						editModel.UpdateBackImage = true;
					}
				} else
				{
					editModel.UpdateBackImage = editModel.UpdateFrontImage = false;
				}

				if (System.Diagnostics.Debugger.IsAttached || Request.RequestUri.Host.Contains("local"))
				{
					if (editModel.UpdateBackImage || editModel.UpdateFrontImage)
					{
						_cardRepository.CardToFile(myCard.CardId, editModel.UpdateFrontImage, editModel.UpdateBackImage,
							editModel.FrontImage, editModel.FrontFileId.GetValueOrDefault(), editModel.FrontType,
							editModel.BackImage, editModel.BackFileId.GetValueOrDefault(), editModel.BackType, userId);
					}

					_cardRepository.UpdateCardOrientation(myCard.CardId, editModel.FrontOrientation, editModel.BackOrientation);

				} else
				{
					var cardRef = Guid.NewGuid().ToString();
					_cardRepository.UploadCardUpdateToBlobStorage(editModel, cardUpdateStorageConnectionString, cardRef);
					_cardRepository.AddCardToQueue(cardUpdateStorageConnectionString, cardRef);
				}

				return new HttpResponseMessage
				{
					Content = new JsonContent(new { Success = true }),
					StatusCode = HttpStatusCode.OK
				};
			} catch (Exception ex)
			{
				_cardRepository.SaveApplicationError(ex, userId);
				return new HttpResponseMessage
				{
					Content = new JsonContent(new { Success = false }),
					StatusCode = HttpStatusCode.InternalServerError
				};
			}
		}

		private byte[] GetImageFromStream(string url)
		{
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

			var client = new WebClient();
			//byte[] imageBytes = new byte[0];
			//client.OpenReadCompleted += (s, e) =>
			//{
			//    imageBytes = new byte[e.Result.Length];
			//    e.Result.Read(imageBytes, 0, imageBytes.Length);

			//};
			//client.OpenRead(new Uri(url));

			return client.DownloadData(url);
		}

		[System.Web.Http.HttpPut]
		public HttpResponseMessage SaveCardVisibility(byte visibility)
		{
			var userId = ValidateUser();

			try
			{
				var myCard = _cardRepository.GetCardsByOwnerId(userId).FirstOrDefault();
				if (myCard == null)
				{
					return new HttpResponseMessage
					{
						Content = new JsonContent(new { Success = false }),
						StatusCode = HttpStatusCode.NotFound
					};
				}

				var model = new AddOrEditCardModel(myCard)
				{
					Visibility = visibility,
					Action = AddOrEditCardModel.CardAction.Edit
				};

				if (System.Diagnostics.Debugger.IsAttached || Request.RequestUri.Host.Contains("local"))
				{
					var card = Card.Clone(model);
					card.CardId = model.CardId;
					_cardRepository.EditCard(card, true, userId, string.Empty);
				} else
				{
					var cardRef = Guid.NewGuid().ToString();
					_cardRepository.UploadCardUpdateToBlobStorage(model, cardUpdateStorageConnectionString, cardRef);
					_cardRepository.AddCardToQueue(cardUpdateStorageConnectionString, cardRef);
				}

				return new HttpResponseMessage
				{
					Content = new JsonContent(new { Success = true }),
					StatusCode = HttpStatusCode.OK
				};
			} catch (Exception ex)
			{
				_cardRepository.SaveApplicationError(ex, userId);
				return new HttpResponseMessage
				{
					Content = new JsonContent(new { Success = false }),
					StatusCode = HttpStatusCode.InternalServerError
				};
			}
		}

		[System.Web.Http.HttpPut]
		//[System.Web.Http.Route("SaveCardExternalLinks")]
		public HttpResponseMessage SaveCardExternalLinks([FromBody] CardLinksModel model)
		{
			var userId = ValidateUser();

			try
			{
				var myCard = _cardRepository.GetCardsByOwnerId(userId).FirstOrDefault();
				if (myCard == null)
				{
					return new HttpResponseMessage
					{
						Content = new JsonContent(new { Success = false }),
						StatusCode = HttpStatusCode.NotFound
					};
				}

				if (model == null)
				{
					return new HttpResponseMessage
					{
						Content = new JsonContent(new { Success = false }),
						StatusCode = HttpStatusCode.BadRequest
					};
				}

				var links = new List<DataAccess.ExternalLink>();
				links.AddRange(model.Links?.Select(link => new DataAccess.ExternalLink
				{
					Link = link.Link,
					ExternalLinkId = link.ExternalLinkId,
					ExternalLinkTypeId = link.ExternalLinkTypeId,
					CardId = link.CardId
				}));
				_cardRepository.UpdateCardLinks(model.CardId, links);

				return new HttpResponseMessage
				{
					Content = new JsonContent(new { Success = true }),
					StatusCode = HttpStatusCode.OK
				};
			}
			catch (Exception ex)
			{
				_cardRepository.SaveApplicationError(ex, userId);
				return new HttpResponseMessage
				{
					Content = new JsonContent(new { Success = false }),
					StatusCode = HttpStatusCode.InternalServerError
				};
			}
		}

		[System.Web.Http.HttpPut]
		public HttpResponseMessage SaveContactInfo([FromBody] CardDetailModel cardInfo)
		{
			var userId = ValidateUser();

			try
			{
				var myCard = _cardRepository.GetCardsByOwnerId(userId).FirstOrDefault();
				if (myCard == null)
				{
					return new HttpResponseMessage
					{
						Content = new JsonContent(new { Success = false }),
						StatusCode = HttpStatusCode.NotFound
					};
				}

				if (cardInfo == null)
				{
					return new HttpResponseMessage
					{
						Content = new JsonContent(new { Success = false }),
						StatusCode = HttpStatusCode.BadRequest
					};
				}

				var model = new AddOrEditCardModel(myCard)
				{
					Email = cardInfo.Email,
					Url = cardInfo.Url,
					CompanyName = cardInfo.CompanyName,
					Name = cardInfo.Name,
					Title = cardInfo.Title,
					Action = AddOrEditCardModel.CardAction.Edit,
					Visibility = cardInfo.Visibility > 0 ? cardInfo.Visibility : myCard.Visibility
				};
				model.PhoneNumbers = cardInfo.PhoneNumbers != null
					? new List<PhoneNumber>(cardInfo.PhoneNumbers)
					: model.PhoneNumbers;

				model.Tags = cardInfo.Tags != null
					? new List<Tag>(cardInfo.Tags.Where(t => !string.IsNullOrEmpty(t.Text)).ToList())
					: model.Tags;

				model.Addresses = cardInfo.Addresses != null
					? new List<CardAddress>(cardInfo.Addresses)
					: model.Addresses;

				if (System.Diagnostics.Debugger.IsAttached || Request.RequestUri.Host.Contains("local"))
				{
					var card = Card.Clone(model);
					card.CardId = model.CardId;
					_cardRepository.EditCard(card, true, userId, string.Empty);
				} else
				{
					var cardRef = Guid.NewGuid().ToString();
					_cardRepository.UploadCardUpdateToBlobStorage(model, cardUpdateStorageConnectionString, cardRef);
					_cardRepository.AddCardToQueue(cardUpdateStorageConnectionString, cardRef);
				}

				return new HttpResponseMessage
				{
					Content = new JsonContent(new { Success = true }),
					StatusCode = HttpStatusCode.OK
				};
			} catch (Exception ex)
			{
				_cardRepository.SaveApplicationError(ex, userId);
				return new HttpResponseMessage
				{
					Content = new JsonContent(new { Success = false }),
					StatusCode = HttpStatusCode.InternalServerError
				};
			}
		}

		[System.Web.Http.HttpPost]
		public HttpResponseMessage Post(long id, long userId)
		{

			userId = ValidateUser();
			if (userId <= 0)
			{
				return new HttpResponseMessage
				{
					Content = new JsonContent(new
					{
						Success = false
					}),
					StatusCode = HttpStatusCode.Unauthorized
				};
			}

			try
			{
				if (userId <= 0)
				{
					return new HttpResponseMessage
					{
						Content = new JsonContent(new
						{
							Success = false
						}),
						StatusCode = HttpStatusCode.Unauthorized
					};
				}

				var frontImage = HttpContext.Current.Request.Files.Count > 0
					? HttpContext.Current.Request.Files[0]
					: null;
				var backImage = HttpContext.Current.Request.Files.Count > 1
					? HttpContext.Current.Request.Files[1]
					: null;

				var addedModel = Serializer.Deserialize<AddOrEditCardModel>(HttpContext.Current.Request.Form["model"]);
				addedModel.UserId = userId;
				addedModel.Action = AddOrEditCardModel.CardAction.Add;

				// Enforce busiess rule of only one card per account.
				CardDetailModel existingCard = null;
				if (addedModel.IsMyCard.GetValueOrDefault())
				{
					existingCard = _cardRepository.GetCardsByOwnerId(userId).FirstOrDefault();
				}
				if (existingCard != null)
				{
					return new HttpResponseMessage
					{
						Content = new JsonContent(new
						{
							Success = false
						}),
						StatusCode = HttpStatusCode.BadRequest
					};
				}

				if (frontImage != null && frontImage.ContentLength > 0)
				{
					var frontBuf = new byte[frontImage.InputStream.Length];
					frontImage.InputStream.Read(frontBuf, 0, (int)frontImage.InputStream.Length);
					addedModel.FrontImage = frontBuf;
					if (frontImage.InputStream.Length > 1024 * 100)
					{
						var resizedImage = _cardRepository.ScaleImage(new Bitmap(new MemoryStream(frontBuf)),
							addedModel.FrontOrientation);
						addedModel.FrontImage = ImageToByte(resizedImage);
					}
					var extension = Path.GetExtension(frontImage.FileName);
					addedModel.FrontFileId = Guid.NewGuid();
					addedModel.FrontType = extension.ToLower().Replace(".", "").Trim();
					addedModel.UpdateFrontImage = true;
				}

				if (backImage != null && backImage.ContentLength > 0)
				{
					var backBuf = new byte[backImage.InputStream.Length];
					backImage.InputStream.Read(backBuf, 0, (int)backImage.InputStream.Length);
					addedModel.BackImage = backBuf;
					if (backImage.InputStream.Length > 1024 * 100)
					{
						var resizedImage = _cardRepository.ScaleImage(new Bitmap(new MemoryStream(backBuf)),
							addedModel.BackOrientation);
						addedModel.BackImage = ImageToByte(resizedImage);
					}
					var extension = Path.GetExtension(backImage.FileName);
					addedModel.BackType = extension.ToLower().Replace(".", "").Trim();
					addedModel.BackFileId = Guid.NewGuid();
					addedModel.UpdateBackImage = true;
				}
				Card card = Card.Clone(addedModel);
				AddOrUpdateCardErrors errors = _cardRepository.CheckForCardModelErrors(card,
					addedModel.IsMyCard.GetValueOrDefault())
											   ?? new AddOrUpdateCardErrors();

				if (errors.ErrorCollection.Count == 0)
				{
					addedModel.CardId = id;
					addedModel.ExistingCards = null;
					addedModel.MyBusidex = null;
					addedModel.PhoneNumberTypes = null;
					addedModel.FileSizeInfoContent = string.Empty;
					addedModel.ModelErrors = null;

					if (System.Diagnostics.Debugger.IsAttached || Request.RequestUri.Host.Contains("local"))
					{
						long cardId;
						_cardRepository.AddCard(card, addedModel.IsMyCard.GetValueOrDefault(), userId, addedModel.Notes,
							out cardId);

						_cardRepository.CardToFile(cardId, addedModel.UpdateFrontImage, addedModel.UpdateBackImage,
							addedModel.FrontImage, addedModel.FrontFileId.GetValueOrDefault(), addedModel.FrontType,
							addedModel.BackImage, addedModel.BackFileId.GetValueOrDefault(), addedModel.BackType, userId);
					} else
					{
						var cardRef = Guid.NewGuid().ToString();
						_cardRepository.UploadCardUpdateToBlobStorage(addedModel, cardUpdateStorageConnectionString, cardRef);
						_cardRepository.AddCardToQueue(cardUpdateStorageConnectionString, cardRef);
					}
					_cardRepository.ClearBusidexCache();

					return new HttpResponseMessage
					{
						Content = new JsonContent(new
						{
							Success = true,
							Model = addedModel
						}),
						StatusCode = HttpStatusCode.OK
					};
				}

				return new HttpResponseMessage
				{
					Content = new JsonContent(new
					{
						Success = false,
						Model = errors
					}),
					StatusCode = HttpStatusCode.BadRequest
				};
			} catch (Exception ex)
			{
				_cardRepository.SaveApplicationError(ex, userId);
				return new HttpResponseMessage
				{
					Content = new JsonContent(new
					{
						Success = false
					}),
					StatusCode = HttpStatusCode.InternalServerError
				};
			}
		}

        [System.Web.Http.HttpPut]
        public HttpResponseMessage UpdateCardOrientation(long id, long userId)
        {
            userId = ValidateUser();
            if (userId <= 0)
            {
                return new HttpResponseMessage
                {
                    Content = new JsonContent(new
                    {
                        Success = false
                    }),
                    StatusCode = HttpStatusCode.Unauthorized
                };
            }

            return new HttpResponseMessage
            {
                Content = new JsonContent(new
                {
                    Success = true
                }),
                StatusCode = HttpStatusCode.OK
            };

        }

        [System.Web.Http.HttpPut]
		public HttpResponseMessage Put(long id, long userId)
		{
			userId = ValidateUser();
			if (userId <= 0)
			{
				return new HttpResponseMessage
				{
					Content = new JsonContent(new
					{
						Success = false
					}),
					StatusCode = HttpStatusCode.Unauthorized
				};
			}

			try
			{
				var frontImage = HttpContext.Current.Request.Files["file0"];
				var backImage = HttpContext.Current.Request.Files["file1"];

				if (userId > 0)
				{
					var updatedModel =
						Serializer.Deserialize<AddOrEditCardModel>(HttpContext.Current.Request.Form["model"]);
					var existingCard = _cardRepository.GetCardById(id, userId);
					var card = Serializer.Deserialize<Card>(HttpContext.Current.Request.Form["model"]);
					var busidexUser = _accountRepository.GetBusidexUserById(userId);
					var isAdmin = Roles.IsUserInRole(busidexUser.UserName, "Administrator");
					// allow admins to edit cards

					// verify that the person logged in is either the card owner or an admin user. If they are not, 
					// return Unauthorized.
					if (card.OwnerId.GetValueOrDefault() != userId && !isAdmin)
					{
						return new HttpResponseMessage
						{
							Content = new JsonContent(new
							{
								Success = false
							}),
							StatusCode = HttpStatusCode.Unauthorized
						};
					}

					if (isAdmin)
					{
						// If this is an admin user, we don't want to overwrite the owner id value with the admin id. Need
						// to get the owner id off of the existing card in the database
						if (existingCard.OwnerId.HasValue)
						{
							userId = existingCard.OwnerId.Value;
							card.OwnerId = updatedModel.OwnerId = existingCard.OwnerId;
							card.Searchable = updatedModel.Searchable = existingCard.Searchable;
							updatedModel.IsMyCard = true;
						}
					}

					if (frontImage != null && frontImage.ContentLength > 0)
					{
						var frontBuf = new byte[frontImage.InputStream.Length];
						frontImage.InputStream.Read(frontBuf, 0, (int)frontImage.InputStream.Length);
						card.FrontImage = updatedModel.FrontImage = frontBuf;
						if (frontImage.InputStream.Length > 1024 * 100)
						{
							var resizedImage = _cardRepository.ScaleImage(new Bitmap(new MemoryStream(frontBuf)),
								updatedModel.FrontOrientation);
							card.FrontImage = updatedModel.FrontImage = ImageToByte(resizedImage);
						}
						var extension = Path.GetExtension(frontImage.FileName);
						card.FrontType = updatedModel.FrontType = extension.ToLower().Replace(".", "").Trim();
						card.FrontFileId = updatedModel.FrontFileId = Guid.NewGuid();
						card.BackFileId = existingCard.BackFileId;
						updatedModel.UpdateFrontImage = true;
					} else
					{
						card.FrontImage = updatedModel.FrontImage = existingCard.FrontImage;
						card.FrontType = updatedModel.FrontType = existingCard.FrontType;
						card.FrontFileId = updatedModel.FrontFileId = existingCard.FrontFileId;
					}

					if (backImage != null && backImage.ContentLength > 0)
					{
						var backBuf = new byte[backImage.InputStream.Length];
						backImage.InputStream.Read(backBuf, 0, (int)backImage.InputStream.Length);
						card.BackImage = updatedModel.BackImage = backBuf;
						if (backImage.InputStream.Length > 1024 * 100)
						{
							var resizedImage = _cardRepository.ScaleImage(new Bitmap(new MemoryStream(backBuf)),
								updatedModel.BackOrientation);
							card.BackImage = updatedModel.BackImage = ImageToByte(resizedImage);
						}
						var extension = Path.GetExtension(backImage.FileName);
						card.BackType = updatedModel.BackType = extension.ToLower().Replace(".", "").Trim();
						card.FrontFileId = existingCard.FrontFileId;
						card.BackFileId = updatedModel.BackFileId = Guid.NewGuid();
						updatedModel.UpdateBackImage = true;
					} else if (updatedModel.ResetBackImage)
					{
						card.BackImage = null;
						card.BackType = "png";
						card.BackFileId = null;
					} else
					{
						card.BackImage = updatedModel.BackImage = existingCard.BackImage;
						card.BackType = updatedModel.BackType = existingCard.BackType;
						card.BackFileId = updatedModel.BackFileId = existingCard.BackFileId;
					}

					updatedModel.Action = AddOrEditCardModel.CardAction.Edit;

					var errors = _cardRepository.CheckForCardModelErrors(card, updatedModel.IsMyCard.GetValueOrDefault());
					if (errors.ErrorCollection.Count == 0)
					{
						updatedModel.CardId = id;
						updatedModel.UserId = userId;
						updatedModel.ExistingCards = null;
						updatedModel.MyBusidex = null;
						updatedModel.PhoneNumberTypes = null;
						updatedModel.FileSizeInfoContent = string.Empty;
						updatedModel.ModelErrors = null;

						if (System.Diagnostics.Debugger.IsAttached || Request.RequestUri.Host.Contains("local"))
						{
							_cardRepository.EditCard(card, updatedModel.IsMyCard.GetValueOrDefault(), userId,
								updatedModel.Notes);
							if (card.CardId > 1 && errors.ErrorCollection.Count == 0)
							{
								if (card.Display == DisplayType.IMG)
								{
									_cardRepository.CardToFile(updatedModel.CardId, updatedModel.UpdateFrontImage,
										updatedModel.UpdateBackImage,
										updatedModel.FrontImage, updatedModel.FrontFileId.GetValueOrDefault(), updatedModel.FrontType,
										updatedModel.BackImage, updatedModel.BackFileId.GetValueOrDefault(), updatedModel.BackType, userId);
								}
							}
						} else
						{
							var cardRef = Guid.NewGuid().ToString();
							_cardRepository.UploadCardUpdateToBlobStorage(updatedModel, cardUpdateStorageConnectionString, cardRef);
							_cardRepository.AddCardToQueue(cardUpdateStorageConnectionString, cardRef);
						}

						return new HttpResponseMessage
						{
							Content = new JsonContent(new
							{
								Success = true,
								Model = updatedModel
							}),
							StatusCode = HttpStatusCode.OK
						};
					}
					return new HttpResponseMessage
					{
						Content = new JsonContent(new
						{
							Success = false,
							Model = errors
						}),
						StatusCode = HttpStatusCode.BadRequest
					};
				}

				return new HttpResponseMessage
				{
					Content = new JsonContent(new
					{
						Success = false
					}),
					StatusCode = HttpStatusCode.Unauthorized
				};
			} catch (MessageSizeExceededException mEx)
			{
				_cardRepository.SaveApplicationError(mEx, userId);
				return new HttpResponseMessage
				{
					Content = new JsonContent(new
					{
						Success = false,
					}),
					StatusCode = HttpStatusCode.InternalServerError
				};
			} catch (Exception ex)
			{
				_cardRepository.SaveApplicationError(ex, userId);
				return new HttpResponseMessage
				{
					Content = new JsonContent(new
					{
						Success = false,
					}),
					StatusCode = HttpStatusCode.InternalServerError
				};
			}
		}

		[System.Web.Http.HttpPost]
		public HttpResponseMessage ImagePreview(int idx)
		{
			var userId = ValidateUser();
			try
			{
				string img = _ImagePreview(idx);

				if (string.IsNullOrEmpty(img))
				{
					return Request.CreateResponse(HttpStatusCode.BadRequest, img);
				}
				return Request.CreateResponse(HttpStatusCode.OK, img);
			} catch (Exception ex)
			{
				_cardRepository.SaveApplicationError(ex, userId);
				return Request.CreateResponse(HttpStatusCode.InternalServerError, ex);
			}

		}

		// DELETE api/<controller>/5
		public void Delete(long id, long userId)
		{
			userId = ValidateUser();
			if (userId <= 0)
			{
				return;
			}
			_cardRepository.DeleteCard(id, userId);
		}

		private string _ImagePreview(int idx)
		{
			string image = string.Empty;
			var request = HttpContext.Current.Request;

			var httpPostedFile = (request.Files.Count == 1)
				? request.Files[0]
				: request.Files[idx];

			if (httpPostedFile.ContentLength > 0)
			{
				var fileLen = httpPostedFile.InputStream.Length;
				//if (fileLen > 1024*170)
				//{
				//    return string.Empty;
				//}

				var input = new byte[fileLen];

				// Initialize the stream.
				var myStream = httpPostedFile.InputStream;

				// Read the file into the byte array.
				myStream.Read(input, 0, (int)fileLen);
				image = Convert.ToBase64String(input);
			}
			return image;
		}


		//public HttpResponseMessage GetSeoCardNames()
		//{
		//    string cardList = string.Join("; ",
		//        _cardRepository.GetSeoCardResult()
		//            .Select(c => c.Name + " of " + c.CompanyName + ", " + c.Title)
		//            .ToArray());
		//    return new HttpResponseMessage
		//    {
		//        Content = new JsonContent(new
		//        {
		//            Success = true,
		//            CardList = cardList
		//        }),
		//        StatusCode = HttpStatusCode.OK
		//    }; 
		//}
	}
}