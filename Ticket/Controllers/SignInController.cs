using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Ticket.Filters;
using Ticket.Models;
using Ticket.Models.Context;

namespace Ticket.Controllers
{
    [LogFilter]
    public class SignInController : Controller
    {
        // GET: SignIn
        public ActionResult Index()
        {
            return View(new Users());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Index(Users user)
        {
            DatabaseContext db = new DatabaseContext();
            List<Users> usersList = db.Users.Where(x => !x.IsDeleted).ToList();
            foreach (Users u in usersList)
            {
                if (u.Email == user.Email && u.Password == Crypto.Hash(user.Password))
                {
                    Session["Login"] = u.Username;
                    if (u.IsAdmin)
                    {
                        Session["Admin"] = u.Username;
                        return RedirectToAction("Index", "Admin");
                    }
                    return RedirectToAction("Index", "Home");
                }
            }
            ModelState.AddModelError("", "E-posta veya şifre yanlış.");

            return View(user);
        }
    }
}