﻿using System;
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

            }

            if (ModelState.IsValid)
            {
                ModelState.AddModelError("","Doğru");
                db.Users.Add(user);
                int num = db.SaveChanges();

                return View(user);

            }
            else
            {
                ModelState.AddModelError("","Bişeyler Yanlış");
                return View(user);
            }


        }
    }
}