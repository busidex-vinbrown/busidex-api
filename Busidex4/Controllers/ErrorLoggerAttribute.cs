using System.Web.Mvc;
using System.Web.Security;
using Busidex.DAL;

namespace Busidex4.Controllers {
    public class ErrorLoggerAttribute : HandleErrorAttribute {

        public override void OnException(ExceptionContext filterContext)
        {

            var bdc = new BusidexDataContext();
            try
            {
                var error = filterContext.Exception;

                string message = error.Message;
                string inner = error.InnerException != null ? error.InnerException.Message : "";
                string stack = error.StackTrace;
                var membershipUser = Membership.GetUser();
                long userId = 0;
                if ( membershipUser != null )
                {
                    var providerUserKey = membershipUser.ProviderUserKey;
                    if ( providerUserKey != null )
                    {
                        userId = ( long ) providerUserKey;
                    }
                }
                bdc.SaveApplicationError ( message, inner, stack, userId );
            }
            finally
            {
                base.OnException ( filterContext );
            }
        }
    }
}