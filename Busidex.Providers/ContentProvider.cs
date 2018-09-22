using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Threading;

namespace Busidex.Providers
{
    public class ContentProvider
    {
        public static string GetContent(string key, string cultureCode = "")
        {
            var rm = new ResourceManager(typeof(BusidexContent));

            var ci = new CultureInfo(cultureCode);
            Thread.CurrentThread.CurrentUICulture = ci;

            return rm.GetString(key);
        }
    }
}
