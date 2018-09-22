

namespace Busidex.Providers {
    using System.Web.Security;
    using System.Configuration.Provider;
    using System.Collections.Specialized;
    using System;
    using System.Data;
    using System.Data.Odbc;
    using System.Configuration;
    using System.Diagnostics;
    using System.Web;
    using System.Globalization;
    using System.Security.Cryptography;
    using System.Text;
    using System.Web.Configuration;

    public class BusidexMembershipUser : MembershipUser {
        private bool _IsSubscriber;
        private string _CustomerID;

        public bool IsSubscriber {
            get { return _IsSubscriber; }
            set { _IsSubscriber = value; }
        }

        public string CustomerID {
            get { return _CustomerID; }
            set { _CustomerID = value; }
        }

        public BusidexMembershipUser(string providername,
                                  string username,
                                  object providerUserKey,
                                  string email,
                                  string passwordQuestion,
                                  string comment,
                                  bool isApproved,
                                  bool isLockedOut,
                                  DateTime creationDate,
                                  DateTime lastLoginDate,
                                  DateTime lastActivityDate,
                                  DateTime lastPasswordChangedDate,
                                  DateTime lastLockedOutDate,
                                  bool isSubscriber,
                                  string customerID) :
            base(providername,
                                       username,
                                       providerUserKey,
                                       email,
                                       passwordQuestion,
                                       comment,
                                       isApproved,
                                       isLockedOut,
                                       creationDate,
                                       lastLoginDate,
                                       lastActivityDate,
                                       lastPasswordChangedDate,
                                       lastLockedOutDate) {
            this.IsSubscriber = isSubscriber;
            this.CustomerID = customerID;            
        }



    }
}
