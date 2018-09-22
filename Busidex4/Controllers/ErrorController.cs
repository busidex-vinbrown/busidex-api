using System;
using System.Web.Mvc;
using System.Web.Security;
using Busidex.BL.Interfaces;
using Busidex.DAL;

namespace Busidex4.Controllers
{
    public class ErrorController : BaseController
    {

        public ErrorController(ICardRepository cardRepository, IAccountRepository accountRepository)
            : base(cardRepository, accountRepository)
        {
            GetBaseViewData();
        }

        public ActionResult Index()
        {
            //var bdc = new BusidexDataContext();
            //try
            //{
            //string message = error.Exception.Message;
            //string inner = error.Exception.InnerException != null ? error.Exception.InnerException.Message : "";
            //string stack = error.Exception.StackTrace;
            //var membershipUser = Membership.GetUser();
            //long userId = 0;
            //if ( membershipUser != null )
            //{
            //    var providerUserKey = membershipUser.ProviderUserKey;
            //    if ( providerUserKey != null )
            //    {
            //        userId = ( long ) providerUserKey;
            //    }
            //}
            //bdc.SaveApplicationError ( message, inner, stack, userId );
            //}
            //catch ( Exception ex )
            //{

            //}

            return View();
        }

    }
}
