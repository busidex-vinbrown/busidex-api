using System;
using System.Web.Mvc;
using System.Drawing;
using System.Drawing.Imaging;

namespace Busidex4.Controllers
{
    public class CAPTCHAImageActionResult : ActionResult
    {
        public Color BackGroundColor { get; set; }
        public Color RandomTextColor { get; set; }
        public string RandomWord { get; set; }

        public override void ExecuteResult(ControllerContext context)
        {
            RenderCAPTCHAImage(context);
        }

        private void RenderCAPTCHAImage(ControllerContext context)
        {
            var objBmp = new Bitmap(350, 70);
            Graphics objGraphics = Graphics.FromImage(objBmp);

            objGraphics.Clear(BackGroundColor);


            // Instantiate object of brush with black color             
            var objBrush = new SolidBrush(RandomTextColor);

            Font objFont = null;
            int a;

            //Creating an array for most readable yet cryptic fonts for OCR's
            // This is entirely up to developer's discretion
            var crypticFonts = new String[18];

            crypticFonts[0] = "Arial";
            crypticFonts[1] = "Verdana";
            crypticFonts[2] = "Comic Sans MS";
            crypticFonts[3] = "Garamond";
            crypticFonts[4] = "Garamond";
            crypticFonts[5] = "Lucida Sans Unicode";
            crypticFonts[6] = "Garamond";
            crypticFonts[7] = "Courier New";
            crypticFonts[8] = "Book Antiqua";
            crypticFonts[9] = "Arial Narrow";
            crypticFonts[10] = "Estrangelo Edessa";
            crypticFonts[11] = "Comic Sans MS";
            crypticFonts[12] = "Comic Sans MS";
            crypticFonts[13] = "Garamond";
            crypticFonts[14] = "Book Antiqua";
            crypticFonts[15] = "Comic Sans MS";
            crypticFonts[16] = "Garamond";
            crypticFonts[17] = "Book Antiqua";
            //Loop to write the characters on image  
            // with different fonts.
            for (a = 0; a <= RandomWord.Length - 1; a++)
            {
                String myFont = crypticFonts[new Random().Next(a)];
                objFont = new Font(myFont, 24, FontStyle.Bold | FontStyle.Italic);
                String str = RandomWord.Substring(a, 1);
                objGraphics.DrawString(str, objFont, objBrush, new Point(a * 32, 19));
                objGraphics.Flush();
            }
            context.HttpContext.Response.ContentType = "image/GF";
            objBmp.Save(context.HttpContext.Response.OutputStream, ImageFormat.Gif);
            if (objFont != null) objFont.Dispose();
            objGraphics.Dispose();
            objBmp.Dispose();

        }
    }
}
