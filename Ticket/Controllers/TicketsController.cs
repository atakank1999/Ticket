using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ticket.Models.Context;
using Ticket.Models;

namespace Ticket.Controllers
{
    public class TicketsController : Controller
    {
        // GET: Tickets
        public ActionResult Index()
        {
            DatabaseContext db = new DatabaseContext();
            List<Models.Users> ticketlist = db.Users.ToList();
            foreach (Users u in ticketlist)
            {
                if (Session["Login"].ToString()==u.Username)
                {
                    return View(u.Tickets);
                }

            }
            return View();

        }
        public ActionResult Create()
        {
            
            return View(new Models.Ticket());
        }
        [HttpPost,ValidateAntiForgeryToken]
        public ActionResult Create(Models.Ticket t)
        {
            DatabaseContext db = new DatabaseContext();
            List<Users> userslist = db.Users.ToList();
            foreach (Users u in userslist)
            {
                if (u.Username == Session["Login"].ToString())
                {
                    t.Author = u;
                }
            }
            db.Tickets.Add(t);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}