using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ticket.Filters;

namespace Ticket.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        [LogFilter]
        public ActionResult Index()
        {
            if (Session["Login"] == null)
            {
                return View();
            }

            return RedirectToAction("Index", "Tickets");
        }

        public ActionResult SignOut()
        {
            Session["Login"] = null;
            Session["Admin"] = null;
            return RedirectToAction("Index");
        }
    }
}