using System;

namespace Busidex.Api.DataAccess.DTO
{
    [Serializable]
    public class EmailTemplate
    {
        public int EmailTemplateId { get; set; }
        public string Code { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

        private string SanitizeMessage(string message)
        {
            string cleanedMessage = message.Replace("<", "")
                .Replace(">", "")
                .Replace("javascript", "");
                //.Replace("'", "&#x27")
                //.Replace("\"", "&quot;")
                //.Replace("/", "&#x2F");
            return cleanedMessage;

        }

        public void Populate(SharedCard sharedCardModel, UserAccount account, Card card)
        {
            if (string.IsNullOrEmpty(Body) || sharedCardModel == null)
            {
                return;
            }
            string message = System.Web.HttpUtility.UrlDecode(sharedCardModel.Recommendation);
            if (!string.IsNullOrEmpty(message))
            {
                message = "\"" + SanitizeMessage(message) + "\"";
            }


            //Subject = Subject.Replace("%NAME_ON_CARD%", name);
            //Body = Body.Replace("%NAME_ON_CARD%", name);
            Subject = Subject.Replace("%SENT_FROM%", string.IsNullOrEmpty(account.DisplayName) ? "Someone" : account.DisplayName);
            Body = Body.Replace("%PERSONAL_MESSAGE%", message);

            if (card == null)
            {
                return;
            }

            Populate(card, account);
        }

        public void Populate(Card card, UserAccount account)
        {
            string height = card.FrontOrientation == "H" ? "120px" : "220px";
            string width = card.FrontOrientation == "H" ? "210px" : "140px";

            string backHeight = card.BackOrientation == "H" ? "120px" : "220px";
            string backWidth = card.BackOrientation == "H" ? "210px" : "140px";
            string name = card.Name ?? card.CompanyName;

            Subject = Subject.Replace("%NAME_ON_CARD%", name);
            Body = Body.Replace("%NAME_ON_CARD%", name);
            Body = Body.Replace("%SENT_FROM%", string.IsNullOrEmpty(account.DisplayName) ? "Someone" : account.DisplayName);
            Body = Body.Replace("###", card.OwnerId.ToString());
            Body = Body.Replace("+++", card.FrontFileId + "." + card.FrontType);
            if (card.BackFileId != null &&
                card.BackFileId.ToString().ToUpper() != "B66FF0EE-E67A-4BBC-AF3B-920CD0DE56C6" &&
                card.BackFileId.ToString().ToUpper() != Guid.Empty.ToString())
            {
                Body = Body.Replace("---", card.BackFileId + "." + card.BackType);
                Body = Body.Replace("%DISPLAY_BACK%", "block");
                Body = Body.Replace("%BHH%", backHeight);
                Body = Body.Replace("%BWW%", backWidth);
            }
            else
            {
                Body = Body.Replace("---", string.Empty);
                Body = Body.Replace("%BHH%", "0");
                Body = Body.Replace("%BWW%", "0");
                Body = Body.Replace("%DISPLAY_BACK%", "none");
            }
            Body = Body.Replace("%HH%", height);
            Body = Body.Replace("%WW%", width);
        }
    }
}
