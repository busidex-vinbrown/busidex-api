using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using Busidex.BL.Interfaces;
using Busidex.DAL;

namespace Busidex4.Controllers
{
    public class BusinessController : BaseController
    {
        public BusinessController(ICardRepository cardRepository, IAccountRepository accountRepository)
            : base(cardRepository, accountRepository)
        {
            GetBaseViewData();
        }

        //
        // GET: /Business/

        //public ActionResult Index()
        //{
        //    return View();
        //}

        ////
        //// GET: /Business/Details/5

        //public ActionResult Details(int id)
        //{
        //    return View();
        //}

        ////
        //// GET: /Business/Create

        //public ActionResult Add()
        //{
        //    return View();
        //} 

        //
        // POST: /Business/Create

        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult Add(Business business)
        //{
        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            // TODO: Add insert logic here
        //            using (BusidexDataContext bdc = new BusidexDataContext())
        //            {
        //                Business b = new Business();
        //                if (TryUpdateModel<Business>(b))
        //                {
        //                    bdc.Businesses.InsertOnSubmit(b);
        //                    bdc.SubmitChanges();
        //                }

        //                return View(business);

        //            }
        //        }
        //        else
        //        {
        //            return View(business);
        //        }
        //    }
        //    catch
        //    {
        //        return View(business);
        //    }
        //}

        //
        // GET: /Business/Edit/5

        //public ActionResult Edit(int id)
        //{
        //    return View();
        //}

        //
        // POST: /Business/Edit/5

        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult Edit(int id, FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add update logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}
    }
}
