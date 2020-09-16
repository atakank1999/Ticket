using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Ticket.Filters;
using Ticket.Models;
using Ticket.Models.Context;
using Ticket.ViewModels;
using Ticket = Ticket.Models.Ticket;

namespace Ticket.Controllers
{
    [LogFilter]
    [AuthFilter]
    public class TicketsController : Controller
    {
        private readonly List<string> acceptedExtensions = new List<string>()
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
        public ActionResult Index(string sortby = "-date")
        {
            if (Session["Login"] == null)
            {
                return RedirectToAction("Index", "SignIn");
            }
            DatabaseContext db = new DatabaseContext();
            List<Models.Users> users = db.Users.Where(x => !x.IsDeleted).ToList();
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

                    u.Tickets = u.Tickets.Where(t => !t.IsDeleted).ToList();
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
            List<Users> userslist = db.Users.Where(x => !x.IsDeleted).ToList();
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
            if (t.IsDeleted)
            {
                return View();
            }

            return View(t);
        }

        public ActionResult EditProfile()
        {
            DatabaseContext db = new DatabaseContext();
            foreach (Users users in db.Users.Where(x => !x.IsDeleted).ToList())
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
            List<Users> userslList = db.Users.Where(x => !x.IsDeleted).ToList();
            Users old = null;
            Log l = null;

            Users updateUser = new Users();
            int result = 0;
            int changes = 0;
            foreach (Users u in userslList)
            {
                if (u.Username != Session["Login"].ToString() && u.Email == model.Email && !u.IsDeleted)
                {
                    ViewBag.status = "warning";
                    ViewBag.result = "Bu E-mail adresi kullanılmaktadır.";
                    return View(updateUser);
                }

                if (u.Username != Session["Login"].ToString() && u.Username == model.Username && !u.IsDeleted)
                {
                    ViewBag.status = "warning";
                    ViewBag.result = "Bu kullanıcı adı kullanılmaktadır.";
                    return View(updateUser);
                }
                if (u.Username == Session["Login"].ToString() && !u.IsDeleted)
                {
                    updateUser = u;
                }
            }

            if (ModelState.IsValid)
            {
                if (Crypto.Hash(model.Password) != updateUser.Password)
                {
                    ViewBag.status = "danger";
                    ViewBag.result = "Şifre Yanlış.";
                    return View(updateUser);
                }
                else
                {
                    old = new Users(updateUser) { IsDeleted = true };
                    l = new Log
                    {
                        ObjecType = typeof(Users).ToString(),
                        Type = "Modified",
                        PreviousUsers = old,
                        IP = HttpContext.Request.UserHostAddress,
                        Time = DateTime.Now,
                        routevalues = HttpContext.Request.Url.PathAndQuery
                    };
                    if (old.Tickets != null)
                    {
                        foreach (Models.Ticket t in old.Tickets)
                        {
                            t.IsDeleted = true;
                        }
                    }

                    Session["Login"] = model.Username;
                    updateUser.Name = model.Name;
                    updateUser.Surname = model.Surname;
                    updateUser.Username = model.Username;
                    updateUser.Email = model.Email;

                    changes = db.ChangeTracker.Entries().Where(x => x.State != EntityState.Unchanged).ToList().Count;

                    result = db.SaveChanges();
                }
            }

            if (changes > 0)
            {
                if (old != null)
                {
                    db.Users.Add(old);
                }

                if (l != null)
                {
                    l.NextUsers = updateUser;

                    l.Users = updateUser;

                    db.Logs.Add(l);
                }
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
            db.SaveChanges();

            return View(updateUser);
        }

        public ActionResult EditTicket(int id)
        {
            DatabaseContext db = new DatabaseContext();
            Models.Ticket model = db.Tickets.Find(id);
            if (model.IsDeleted)
            {
                return View();
            }

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult EditTicket(Models.Ticket model, int id, HttpPostedFileBase file)
        {
            DatabaseContext db = new DatabaseContext();

            Models.Ticket updateTicket = db.Tickets.Find(id);
            Models.Ticket old = null;
            if (ModelState.IsValid && !updateTicket.IsDeleted)
            {
                string user = Session["Login"].ToString();
                old = new Models.Ticket(updateTicket) { IsDeleted = true };

                updateTicket.Type = model.Type;
                updateTicket.Text = model.Text;
                updateTicket.Title = model.Title;
                updateTicket.EditedOn = DateTime.Now;
                Log l = new Log
                {
                    ObjecType = typeof(Models.Ticket).ToString(),
                    Type = "Modified",
                    Users = db.Users.Where(x => x.Username == user && x.IsDeleted == false).FirstOrDefault(),
                    PreviousTicket = old,
                    IP = HttpContext.Request.UserHostAddress,
                    Time = DateTime.Now,
                    routevalues = HttpContext.Request.Url.PathAndQuery
                };
                if (file != null)
                {
                    if (acceptedExtensions.Contains(Path.GetExtension(file.FileName)))
                    {
                        if (!Directory.Exists(Server.MapPath("~/userfiles")))
                        {
                            Directory.CreateDirectory(Server.MapPath("~/userfiles"));
                        }
                        file.SaveAs(Path.Combine(Server.MapPath("~/userfiles"), file.FileName));
                        if (updateTicket.FilePath!=null)
                        {
                            System.IO.File.Delete(updateTicket.FilePath);

                        }
                        updateTicket.FilePath = Path.Combine(Server.MapPath("~/userfiles"), file.FileName);
                    }
                    else
                    {
                        @ViewBag.file = "Bu dosya tipi Desteklenmemektedir";
                        return View(updateTicket);
                    }
                }
                int changes = db.ChangeTracker.Entries().Where(x => x.State != EntityState.Unchanged).ToList().Count;
                if (changes == 0)
                {
                    ModelState.AddModelError("", "Değişiklik Olmamıştır.");
                    return View(updateTicket);
                }
                db.Tickets.Add(old);
                l.NexTicket = updateTicket;
                l.Ticket = updateTicket;
                db.Logs.Add(l);
            }

            db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            DatabaseContext db = new DatabaseContext();
            Models.Ticket t = db.Tickets.Find(id);
            if (!t.IsDeleted)
            {
                t.IsDeleted = true;
                string username = Session["Login"].ToString();

                Log log = new Log
                {
                    Users = db.Users.FirstOrDefault(x => x.Username == username.ToString()),
                    ObjecType = typeof(Models.Ticket).ToString(),
                    Type = "Deleted",
                    IP = HttpContext.Request.UserHostAddress,
                    PreviousTicket = t,
                    Time = DateTime.Now,
                    Ticket = t,
                    routevalues = HttpContext.Request.Url.PathAndQuery
                };
                db.Logs.Add(log);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        public ActionResult Download(int id)
        {
            DatabaseContext db = new DatabaseContext();
            Models.Ticket ticket = db.Tickets.Find(id);
            string path;
            if (!ticket.IsDeleted)
            {
                path = ticket.FilePath;
                return File(path, "application/force-download", Path.GetFileName(path));
            }

            return RedirectToAction("Index");
        }
    }
}