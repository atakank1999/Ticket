using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ticket.Models;
using Ticket.Models.Context;

namespace Ticket.Controllers
{
    public class RegisterController : Controller
    {
        // GET: Register
        public ActionResult Index()
        {
            return View(new Users());
        }
        [HttpPost,ValidateAntiForgeryToken]
        public ActionResult Index(Users user)

        {
            
            if (!ModelState.IsValid)
            {
                return View(user);
            }
            DatabaseContext db = new DatabaseContext();
            foreach (var u in db.Users.ToList())
            {
                if (user.Email == u.Email)
                {
                    ModelState.AddModelError("", "Bu e-posta adresi zaten kullanılıyor");
                    return View(user);
                }

                if (user.Username == u.Username)
                {
                    ModelState.AddModelError("","Bu kullanıcı adı zaten kullanılıyor");
                    return View(user);

                }

            }

            if (ModelState.IsValid)
            {
                db.Users.Add(user);
                int num = db.SaveChanges();

                return RedirectToAction("Index", "Tickets");

            }
            else
            {
                ModelState.AddModelError("","Birşeyler Yanlış");
                return View(user);
            }


        }
    }
}