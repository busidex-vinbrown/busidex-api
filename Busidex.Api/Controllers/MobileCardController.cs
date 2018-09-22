using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Busidex.Api.DataAccess.DTO;
using Busidex.Api.DataServices.Interfaces;
using Busidex.Api.Models;
using log4net;

namespace Busidex.Api.Controllers
{
    public class MobileCardController : BaseApiController
    {
        private static ILog _log;
        private const string NO_FILE_UPLOADED = "No file uploaded";

         public MobileCardController(ICardRepository cardRepository)
        {
            _cardRepository = cardRepository;
            _log = _log ?? LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }

        [HttpPut]
        [Obsolete]
        public HttpResponseMessage Put([FromBody] CardDTO cardDTO)
        {
            return new HttpResponseMessage
            {
                Content = new JsonContent(new
                {
                    Success = false
                }),
                StatusCode = HttpStatusCode.BadRequest,
                ReasonPhrase = "Method No Longer Supported"
            };

            //try
            //{
            //    if (cardDTO == null || cardDTO.mId <= 0)
            //    {
            //        return new HttpResponseMessage
            //        {
            //            Content = new JsonContent(new
            //            {
            //                Success = false
            //            }),
            //            StatusCode = HttpStatusCode.Unauthorized
            //        };
            //    }

            //    var existingCard = _cardRepository.GetCardsByOwnerId(cardDTO.mId).FirstOrDefault();
            //    if (existingCard == null)
            //    {
            //        return new HttpResponseMessage
            //        {
            //            Content = new JsonContent(new
            //            {
            //                Success = false
            //            }),
            //            StatusCode = HttpStatusCode.BadRequest
            //        };
            //    }
            //    BusidexUser bu = _cardRepository.GetBusidexUserById(cardDTO.mId);
            //    var model = _cardRepository.GetAddOrEditModel(existingCard.CardId, bu, "EDIT");

            //    model.Email = cardDTO.email;
            //    model.Name = cardDTO.name;
            //    if (model.PhoneNumbers.Count > 0)
            //    {
            //        model.PhoneNumbers[0].Number = cardDTO.phone;
            //    }

            //    byte[] frontImage =
            //        Convert.FromBase64String(cardDTO.src.Replace("data:image/jpeg;base64,", string.Empty));
            //    if (frontImage.Length == 0)
            //    {
            //        throw new ArgumentNullException(NO_FILE_UPLOADED);
            //    }

            //    var resizedImage = _cardRepository.ScaleImage(new Bitmap(new MemoryStream(frontImage)), "H");

            //    model.FrontFileId = existingCard.FrontFileId;
            //    model.BackFileId = existingCard.BackFileId;
            //    model.FrontImage = ImageToByte(resizedImage);
            //    model.FrontType = existingCard.FrontFileType;
            //    model.BackType = existingCard.BackFileType;
            //    model.UpdateFrontImage = true;
            //    model.Action = AddOrEditCardModel.CardAction.Edit;

            //    _cardRepository.AddCardToQueue(model);

            //}
            //catch (ArgumentNullException nEx)
            //{
            //    return new HttpResponseMessage
            //    {
            //        Content = new JsonContent(new
            //        {
            //            Success = false,
            //            Model = nEx.Message
            //        }),
            //        StatusCode = HttpStatusCode.BadRequest
            //    };
            //}
            //catch (Exception ex)
            //{
            //    _cardRepository.SaveApplicationError(ex, cardDTO != null ? cardDTO.mId : 0);

            //    return new HttpResponseMessage
            //    {
            //        Content = new JsonContent(new
            //        {
            //            Success = false
            //        }),
            //        StatusCode = HttpStatusCode.InternalServerError
            //    };
            //}
            //return new HttpResponseMessage
            //{
            //    Content = new JsonContent(new
            //    {
            //        Success = true
            //    }),
            //    StatusCode = HttpStatusCode.OK
            //};

        }

        [Obsolete]
        public HttpResponseMessage Post([FromBody] CardDTO cardDTO)
        {
            return new HttpResponseMessage
            {
                Content = new JsonContent(new
                {
                    Success = false
                }),
                StatusCode = HttpStatusCode.BadRequest,
                ReasonPhrase = "Method No Longer Supported"
            };

            //const string NO_IMAGE_SOURCE = "Image source is empty";
            //const string NO_DATA = "No data uploaded";
            //try
            //{

            //    if (cardDTO == null)
            //    {
            //        throw new ArgumentNullException(NO_DATA);
            //    }
            //    if (string.IsNullOrEmpty(cardDTO.src))
            //    {
            //        throw new ArgumentNullException(NO_IMAGE_SOURCE);
            //    }

            //    var existingCard = _cardRepository.GetCardsByOwnerId(cardDTO.mId).FirstOrDefault();
            //    if (existingCard != null && existingCard.CardId > 1)
            //    {
            //        return new HttpResponseMessage
            //        {
            //            Content = new JsonContent(new
            //            {
            //                Success = false
            //            }),
            //            StatusCode = HttpStatusCode.BadRequest
            //        };
            //    }

            //    var model = new AddOrEditCardModel
            //    {
            //        Name = cardDTO.name,
            //        CreatedBy = cardDTO.mId,
            //        BackOrientation = "H",
            //        FrontOrientation = "H",
            //        FrontType = "jpg",
            //        BackType = "png",
            //        OwnerId = cardDTO.mId,
            //        Searchable = true,
            //        Email = string.IsNullOrEmpty(cardDTO.email) ? "no email" : cardDTO.email,
            //        Action = AddOrEditCardModel.CardAction.Add,
            //        PhoneNumbers = new List<PhoneNumber>
            //        {
            //            new PhoneNumber
            //            {
            //                Number = cardDTO.phone,
            //                PhoneNumberTypeId = 1,
            //                PhoneNumberType = new PhoneNumberType
            //                {
            //                    PhoneNumberTypeId = 1
            //                }
            //            }
            //        },
            //        Tags = new List<Tag>(),
            //        Addresses = new List<CardAddress>(),
            //        Notes = string.Empty,
            //        UserId = cardDTO.mId
            //    };

            //    byte[] frontImage = Convert.FromBase64String(cardDTO.src.Replace("data:image/jpeg;base64,", string.Empty));
            //    var resizedImage = _cardRepository.ScaleImage(new Bitmap(new MemoryStream(frontImage)), "H");

            //    if (frontImage.Length > 0)
            //    {

            //        model.FrontFileId = null;
            //        model.BackFileId = null;
            //        model.FrontImage = ImageToByte(resizedImage);
            //        model.FrontType = "jpg";
            //        model.BackType = "jpg";
            //        model.UpdateFrontImage = true;
            //        _cardRepository.AddCardToQueue(model);

            //    }
            //    else
            //    {
            //        throw new ArgumentNullException(NO_FILE_UPLOADED);
            //    }
            //}
            //catch (ArgumentNullException nEx)
            //{
            //    //_cardRepository.SaveApplicationError(nEx, cardDTO != null ? cardDTO.mId : -1);
            //    return new HttpResponseMessage
            //    {
            //        Content = new JsonContent(new
            //        {
            //            Success = false,
            //            Model = nEx.Message
            //        }),
            //        StatusCode = HttpStatusCode.BadRequest
            //    };
            //}
            //catch (Exception ex)
            //{
            //    //_cardRepository.SaveApplicationError(ex, cardDTO != null ? cardDTO.mId : -1);

            //    return new HttpResponseMessage
            //    {
            //        Content = new JsonContent(new
            //        {
            //            Success = false
            //        }),
            //        StatusCode = HttpStatusCode.InternalServerError
            //    };
            //}
            //return new HttpResponseMessage
            //{
            //    Content = new JsonContent(new
            //    {
            //        Success = true
            //    }),
            //    StatusCode = HttpStatusCode.OK
            //};

        }
    }
}
