using System.Web.Mvc;
using Busidex.BL.Interfaces;

namespace Busidex4.Controllers
{
    public class SharedController : Controller
    {
        private readonly IApplicationRepository _applicationRepository;

        public SharedController(IApplicationRepository repository)
        {
            _applicationRepository = repository;
        }
    }
}