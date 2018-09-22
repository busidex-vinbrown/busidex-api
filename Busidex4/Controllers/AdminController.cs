using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Busidex.BL.Interfaces;
using Busidex.DAL;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.StorageClient;

namespace Busidex4.Controllers
{
    public class AdminController : BaseController
    {
        private readonly Dictionary<string, string> _mimeTypes = new Dictionary<string, string>();
        private readonly IAdminRepository _adminRepository;

        public AdminController(IAdminRepository adminRepository, ICardRepository cardRepository, IAccountRepository accountRepository)
            : base(cardRepository, accountRepository)
        {
            if (adminRepository == null) throw new ArgumentNullException("adminRepository");
            _adminRepository = adminRepository;

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

        [Authorize(Roles = "Administrator")]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult OwnerEmails()
        {
            var model = _adminRepository.GetAllUnownedCards();
            return View(model);
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult SendOwnerEmail()
        {
            var token = Guid.NewGuid();
            var template = _accountRepository.GetEmailTemplate(EmailTemplateCode.ConfirmOwner);
            string email = Request.Form["email"];
            long cardId = long.Parse(Request.Form["cardId"]);
            var sentById = GetUserId();

            template.Body = template.Body.Replace("###", token.ToString());

            var communication = new Communication
            {
                EmailTemplate = template,
                Email = email,
                DateSent = DateTime.UtcNow,
                UserId = 0,
                SentById = sentById,
                OwnerToken = token,
                Failed = false,
                EmailTemplateId = template.EmailTemplateId
            };

            try
            {
                SendEmail(communication);
            }
            catch (Exception ex)
            {
                string exception = ex.Message;
                communication.Failed = true;
            }
            finally
            {
                _accountRepository.SaveCommunication(communication);

                _cardRepository.SaveCardOwnerToken(cardId, token);
            }

            return RedirectToAction("OwnerEmails");
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult CardMaintenance()
        {
            return View();
        }

        public ActionResult NewCards()
        {
            var cards = _cardRepository.GetAllCards();
            var count = ConfigurationManager.AppSettings["AdminRecentCardCount"];
            var model = cards.OrderByDescending(c => c.Updated).Take(int.Parse(count)).OrderByDescending(c => c.Updated).ToList();

            return View(model);
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult ExtractCardsToFile()
        {
            var cards = _cardRepository.GetAllCards();
            Card defaultCard = _cardRepository.GetCardById(1);
            foreach (var card in cards.Where(c => c.CardId > 430))
            {
                CardToFile(card.CardId, defaultCard);
            }

            return RedirectToAction("CardMaintenance");
        }

        [Authorize(Roles = "Administrator")]
        public ActionResult ExtractCardToFile()
        {
            long cardId = long.Parse(Request.Form["cardId"]);
            Card defaultCard = _cardRepository.GetCardById(1);

            CardToFile(cardId, defaultCard);
            return RedirectToAction("CardMaintenance");
        }

        public static Image ScaleImage(Image image, int maxWidth, int maxHeight)
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

        protected void CardToFile(long cardId, Card defaultCard)
        {

            var card = _cardRepository.GetCardById(cardId);
            if (card != null)
            {
                var frontGuid = card.FrontFileId ?? defaultCard.FrontFileId.GetValueOrDefault();
                var backGuid = card.BackFileId ?? defaultCard.BackFileId.GetValueOrDefault();
                if (card.BackImage == null)
                {
                    card.BackFileId = backGuid = defaultCard.BackFileId.GetValueOrDefault();
                }

                _cardRepository.UpdateCardFileId(cardId, frontGuid, backGuid);

                var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["StorageConnection"].ConnectionString);
                var blobStorage = storageAccount.CreateCloudBlobClient();
                var container = blobStorage.GetContainerReference("http://busidexcdn.blob.core.windows.net/mobile-images");

                string uniqueBlobName = string.Format("{0}.{1}", card.FrontFileId, card.FrontType);
                var frontImageBlob = container.GetBlockBlobReference(uniqueBlobName);
                frontImageBlob.Properties.ContentType = _mimeTypes[card.FrontType ?? "image/jpeg"];
                frontImageBlob.Properties.CacheControl = "public, max-age=31536000";

                var options = new BlobRequestOptions
                                  {
                                      AccessCondition =
                                          AccessCondition.None
                                  };
                //frontImageBlob.SetProperties(options);

                frontImageBlob.Properties.ContentEncoding = _mimeTypes[card.FrontType ?? "image/jpeg"];
                frontImageBlob.Properties.ContentLanguage = "en-US";
                //frontImageBlob.SetProperties();

                int maxHeight = card.FrontOrientation == "H" ? 95 : 130;
                int maxWidth = card.FrontOrientation == "H" ? 130 : 95;
                Image img = new Bitmap(new MemoryStream(card.FrontImage.ToArray()));
                var scaledImage = ScaleImage(img, maxWidth, maxHeight);

                using (var memoryStream = new MemoryStream())
                {
                    scaledImage.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);

                    memoryStream.Seek(0, SeekOrigin.Begin);
                    frontImageBlob.UploadFromStream(memoryStream);
                }

                if (card.BackImage != null)
                {
                    maxHeight = card.BackOrientation == "H" ? 75 : 110;
                    maxWidth = card.BackOrientation == "H" ? 110 : 75;
                    img = new Bitmap(new MemoryStream(card.FrontImage.ToArray()));
                    scaledImage = ScaleImage(img, maxWidth, maxHeight);

                    using (var memoryStream = new MemoryStream())
                    {
                        scaledImage.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        uniqueBlobName = string.Format("{0}.{1}", card.BackFileId, card.BackType);
                        var backImageBlob = container.GetBlockBlobReference(uniqueBlobName);
                        backImageBlob.Properties.ContentType = _mimeTypes[card.BackType ?? "image/jpeg"];
                        backImageBlob.Properties.CacheControl = "public, max-age=31536000";
                        backImageBlob.UploadFromStream(memoryStream);
                    }
                }
            }
        }
    }
}
