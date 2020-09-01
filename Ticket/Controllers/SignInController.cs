using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ticket.Models;
using Ticket.Models.Context;

namespace Ticket.Controllers
{
    public class SignInController : Controller
    {
        // GET: SignIn
        public ActionResult Index()
        {
            return View(new Users());
        }
        [HttpPost,ValidateAntiForgeryToken]
        public ActionResult Index(Users user)
        {
            DatabaseContext db = new DatabaseContext();
            List<Users> usersList = db.Users.ToList();
            foreach (Users u in usersList)
            {
                if (u.Email == user.Email && u.Password == user.Password)
                {
                    Session["Login"] = u.Username;
                    if (u.IsAdmin)
                    {
                        Session["Admin"] = u.Username;
                    }
                    return RedirectToAction("Index","Home");
                }
            }
            ModelState.AddModelError("","E-posta ya da şifre yanlış.");

            return View(user);
        }
    }
}