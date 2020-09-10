using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Ticket.Filters;
using Ticket.Models;
using Ticket.Models.Context;

namespace Ticket.Controllers
{
    [LogFilter]
    public class RegisterController : Controller
    {
        // GET: Register
        public ActionResult Index()
        {
            return View(new Users());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Index(Users user)

        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }
            DatabaseContext db = new DatabaseContext();
            foreach (var u in db.Users.ToList())
            {
                if (user.Email == u.Email && !u.IsDeleted)
                {
                    ModelState.AddModelError("", "Bu e-posta adresi zaten kullanılıyor");
                    return View(user);
                }

                if (user.Username == u.Username && !u.IsDeleted)
                {
                    ModelState.AddModelError("", "Bu kullanıcı adı zaten kullanılıyor");
                    return View(user);
                }
            }

            if (ModelState.IsValid)
            {
                user.Password = Crypto.Hash(user.Password);
                db.Users.Add(user);
                int num = db.SaveChanges();

                return RedirectToAction("verify", "Register", new { id = user.ID });
            }
            else
            {
                ModelState.AddModelError("", "Birşeyler Yanlış");
                return View(user);
            }
        }

        public ActionResult verify(int id)
        {
            DatabaseContext db = new DatabaseContext();
            Users u = db.Users.Find(id);
            MailMessage message = new MailMessage();
            message.To.Add(new MailAddress(u.Email.ToString()));
            message.Subject = "Üyeliğinizi Onaylayın";
            message.Body = "Üyeliğinizi Onaylamak İçin :" +
                          "https://ticket20200831142313.azurewebsites.net/Register/guid/" + u.ConfirmGuid.ToString();
            using (var smtp = new SmtpClient())
            {
                smtp.Send(message);
            }
            return View(u);
        }

        public ActionResult guid(Guid? id)
        {
            DatabaseContext db = new DatabaseContext();
            List<Users> userlist = db.Users.Where(x => !x.IsDeleted).ToList();
            foreach (Users u in userlist)
            {
                if (id == u.ConfirmGuid)
                {
                    u.IsConfirmed = true;
                    Session["Login"] = u.Username;
                }
            }

            db.SaveChanges();
            return View();
        }
    }
}