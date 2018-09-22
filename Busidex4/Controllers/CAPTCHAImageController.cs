using System.Web.Mvc;
using System.Drawing;

using Busidex.DAL;

namespace Busidex4.Controllers
{
    public class CAPTCHAImageController : Controller
    {
        public CAPTCHAImageActionResult Index(int c)
        {

            var ctx = new BusidexDataContext();

            Captcha item = ctx.GetCaptchaItemById(c);
            string randomText = item.CaptchaText;// SelectRandomWord(6);
            if (HttpContext.Session != null) HttpContext.Session["RandomText"] = randomText;
            return new CAPTCHAImageActionResult { BackGroundColor = Color.LightGray, RandomTextColor = Color.Black, RandomWord = randomText };

        }
        /*
                private string SelectRandomWord(int numberOfChars)
                {
                    if (numberOfChars > 36)
                    {
                        throw new InvalidOperationException("Random Word Characters can not be greater than 36.");
                    }
                    // Creating an array of 26 characters  and 0-9 numbers
                    char[] columns = new char[36];

                    for (int charPos = 65; charPos < 65 + 26; charPos++)
                        columns[charPos - 65] = (char)charPos;

                    for (int intPos = 48; intPos <= 57; intPos++)
                        columns[26 + (intPos - 48)] = (char)intPos;

                    var randomBuilder = new StringBuilder();
                    // pick characters from random position
                    var randomSeed = new Random();
                    for (int incr = 0; incr < numberOfChars; incr++)
                    {
                        randomBuilder.Append(columns[randomSeed.Next(36)].ToString());

                    }

                    return randomBuilder.ToString();
                }
        */
    }
}
