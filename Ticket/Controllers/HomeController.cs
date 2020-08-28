using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ticket.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            if (Session["Login"]==null)
            {
            return View();

            }

            return RedirectToAction("Index", "Tickets");
            
        }
    }
}