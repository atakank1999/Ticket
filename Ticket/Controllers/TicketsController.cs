using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Ticket.Filters;
using Ticket.Models;
using Ticket.Models.Context;
using Ticket.ViewModels;

namespace Ticket.Controllers
{
    [AuthFilter]
    public class TicketsController : Controller
    {
        private List<string> acceptedExtensions = new List<string>()
        {
            ".pdf",
            ".xls",
            ".xlsx",
            ".jpg",
            ".docx",
            ".doc",
            ".jpeg"
        };

        // GET: Tickets
        public ActionResult Index(string sortby = "date")
        {
            if (Session["Login"] == null)
            {
                return RedirectToAction("Index", "SignIn");
            }
            DatabaseContext db = new DatabaseContext();
            List<Models.Users> users = db.Users.ToList();
            foreach (Users u in users)
            {
                if (Session["Login"].ToString() == u.Username)
                {
                    if (sortby == "name")
                    {
                        u.Tickets = u.Tickets.OrderBy(o => o.Title).ToList();
                    }
                    else if (sortby == "-name")
                    {
                        u.Tickets = u.Tickets.OrderByDescending(o => o.Title).ToList();
                    }
                    else if (sortby == "date")
                    {
                        u.Tickets = u.Tickets.OrderBy(o => o.DateTime).ToList();
                    }
                    if (sortby == "-date")
                    {
                        u.Tickets = u.Tickets.OrderByDescending(o => o.DateTime).ToList();
                    }
                    else if (sortby == "status")
                    {
                        u.Tickets = u.Tickets.OrderBy(o => o.Status).ToList();
                    }
                    else if (sortby == "-status")
                    {
                        u.Tickets = u.Tickets.OrderByDescending(o => o.Status).ToList();
                    }
                    else if (sortby == "type")
                    {
                        u.Tickets = u.Tickets.OrderBy(o => o.Type).ToList();
                    }
                    else if (sortby == "-type")
                    {
                        u.Tickets = u.Tickets.OrderByDescending(o => o.Type).ToList();
                    }
                    return View(u);
                }
            }

            return RedirectToAction("Index", "SignIn");
        }

        public ActionResult Create()
        {
            TicketwithListViewModel model = new TicketwithListViewModel();

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Create(TicketwithListViewModel t, HttpPostedFileBase file)
        {
            if (!ModelState.IsValid)
            {
                return View(t);
            }

            if (file != null)
            {
                if (file != null)
                {
                    if (acceptedExtensions.Contains(Path.GetExtension(file.FileName)))
                    {
                        if (!Directory.Exists(Server.MapPath("~/userfiles")))
                        {
                            Directory.CreateDirectory(Server.MapPath("~/userfiles"));
                        }
                        file.SaveAs(Path.Combine(Server.MapPath("~/userfiles"), file.FileName));
                        t.Ticket.FilePath = Path.Combine(Server.MapPath("~/userfiles"), file.FileName);
                    }
                    else
                    {
                        @ViewBag.file = "Bu dosya tipi Desteklenmemektedir";
                        return View(t);
                    }
                }
            }
            DatabaseContext db = new DatabaseContext();
            List<Users> userslist = db.Users.ToList();
            foreach (Users u in userslist)
            {
                if (u.Username == Session["Login"].ToString())
                {
                    t.Ticket.Author = u;
                }
            }
            db.Tickets.Add(t.Ticket);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Ticket(int id)
        {
            DatabaseContext db = new DatabaseContext();
            Models.Ticket t = db.Tickets.Find(id);

            return View(t);
        }

        public ActionResult EditProfile()
        {
            DatabaseContext db = new DatabaseContext();
            foreach (Users users in db.Users.ToList())
            {
                if (users.Username == Session["Login"].ToString())
                {
                    return View(users);
                }
            }

            return RedirectToAction("Index", "SignIn");
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult EditProfile(Users model)
        {
            DatabaseContext db = new DatabaseContext();
            List<Users> userslList = db.Users.ToList();

            Users updateUser = new Users();
            int result = 0;
            int changes = 0;
            foreach (Users u in userslList)
            {
                if (u.Username == Session["Login"].ToString())
                {
                    updateUser = u;
                }
            }

            if (ModelState.IsValid && Crypto.Hash(model.Password) == updateUser.Password)
            {
                Session["Login"] = model.Username;
                updateUser.Username = model.Username;
                updateUser.Name = model.Name;
                updateUser.Surname = model.Surname;
                updateUser.Email = model.Email;
                changes = db.ChangeTracker.Entries().ToList().Count;

                result = db.SaveChanges();
            }

            if (Crypto.Hash(model.Password) != updateUser.Password)
            {
                ViewBag.status = "danger";
                ViewBag.result = "Şifre Yanlış.";
                return View(updateUser);
            }

            if (changes > 0)
            {
                if (result > 0)
                {
                    ViewBag.status = "success";
                    ViewBag.result = "Değişiklikler Kaydedilmiştir.";
                }
                else
                {
                    ViewBag.status = "danger";
                    ViewBag.result = "Değişiklikler Kaydedilememiştir.";
                }
            }
            else
            {
                ViewBag.status = "warning";
                ViewBag.result = "Değişiklik Olmamıştır.";
            }
            return View(updateUser);
        }

        public ActionResult EditTicket(int id)
        {
            DatabaseContext db = new DatabaseContext();
            Models.Ticket model = db.Tickets.Find(id);

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult EditTicket(Models.Ticket model, int id, HttpPostedFileBase file)
        {
            DatabaseContext db = new DatabaseContext();

            Models.Ticket updateTicket = db.Tickets.Find(id);

            if (ModelState.IsValid)
            {
                updateTicket.Type = model.Type;
                updateTicket.Text = model.Text;
                updateTicket.Title = model.Title;
                updateTicket.EditedOn = DateTime.UtcNow;
                if (file != null)
                {
                    if (acceptedExtensions.Contains(Path.GetExtension(file.FileName)))
                    {
                        if (!Directory.Exists(Server.MapPath("~/userfiles")))
                        {
                            Directory.CreateDirectory(Server.MapPath("~/userfiles"));
                        }
                        file.SaveAs(Path.Combine(Server.MapPath("~/userfiles"), file.FileName));
                        System.IO.File.Delete(updateTicket.FilePath);
                        updateTicket.FilePath = Path.Combine(Server.MapPath("~/userfiles"), file.FileName);
                    }
                    else
                    {
                        @ViewBag.file = "Bu dosya tipi Desteklenmemektedir";
                        return View(updateTicket);
                    }
                }
            }
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            DatabaseContext db = new DatabaseContext();
            Models.Ticket t = db.Tickets.Find(id);
            if (t.assignedTo != null)
            {
                db.Assignments.Remove(t.assignedTo);
            }

            if (t.Replies.Count != 0)
            {
                foreach (Reply reply in t.Replies.ToList())
                {
                    db.Replies.Remove(reply);
                }
            }

            db.Tickets.Remove(t);
            db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}