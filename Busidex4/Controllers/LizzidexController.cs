using System.Web.Mvc;
using Busidex.BL.Interfaces;
using Busidex.DAL;

namespace Busidex4.Controllers
{
    public class LizzidexController : BaseController
    {

        public LizzidexController(ICardRepository cardRepository, IAccountRepository accountRepository)
            : base(cardRepository, accountRepository)
        {
            GetBaseViewData();
        }

        public ActionResult Index()
        {
            var lizzy = new Lizzidex(); // BusidexDAL.GetLizzidex();             
            return View(lizzy);
        }

    }
}
