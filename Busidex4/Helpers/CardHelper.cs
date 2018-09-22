using System;

namespace Busidex4.Helpers
{
    public static class CardHelper
    {
        public static string CleanPhoneNumber(string phone)
        {
            return string.IsNullOrEmpty(phone) ? phone : phone.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "").Replace(".", "");
        }

        public static string FormatPhoneNumber(string phone)
        {

            string cleanPhone = CleanPhoneNumber(phone);

            long lPhone = -1;
            long.TryParse(cleanPhone, out lPhone);
            string formatedPhone = String.Format("{0:(###) ###-####}", lPhone);

            return formatedPhone;
        }
    }
}