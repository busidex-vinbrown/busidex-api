using System;
using System.Configuration;

namespace Busidex4.Models
{
    [Serializable]
    public class GoogleInfo : ConfigurationSection
    {
        [ConfigurationProperty("ClientId", IsRequired = true)]
        public string ClientId
        {
            get { return this["ClientId"].ToString(); }
            set { this["ClientId"] = value; }
        }

        [ConfigurationProperty("EmailAddress", IsRequired = true)]
        public string EmailAddress
        {
            get { return this["EmailAddress"].ToString(); }
            set { this["EmailAddress"] = value; }
        }

        [ConfigurationProperty("ClientSecret", IsRequired = true)]
        public string ClientSecret
        {
            get { return this["ClientSecret"].ToString(); }
            set { this["ClientSecret"] = value; }
        }

        [ConfigurationProperty("RedirectURIs", IsRequired = true)]
        public string RedirectURIs
        {
            get { return this["RedirectURIs"].ToString(); }
            set { this["RedirectURIs"] = value; }
        }

        [ConfigurationProperty("JavaScriptOrigins", IsRequired = true)]
        public string JavaScriptOrigins
        {
            get { return this["JavaScriptOrigins"].ToString(); }
            set { this["JavaScriptOrigins"] = value; }
        }

        [ConfigurationProperty("AccessToken", IsRequired = true)]
        public string AccessToken
        {
            get { return this["AccessToken"].ToString(); }
            set { this["AccessToken"] = value; }
        }

        [ConfigurationProperty("RefreshToken", IsRequired = true)]
        public string RefreshToken
        {
            get { return this["RefreshToken"].ToString(); }
            set { this["RefreshToken"] = value; }
        }

        [ConfigurationProperty("Scope", IsRequired = true)]
        public string Scope
        {
            get { return this["Scope"].ToString(); }
            set { this["Scope"] = value; }
        }
    }
}