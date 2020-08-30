using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ticket.Models.Context;
using Ticket.Models;
using Ticket.ViewModels;
using Type = Ticket.Models.Type;

namespace Ticket.Controllers
{
    public class TicketsController : Controller
    {
        // GET: Tickets
        public ActionResult Index()
        {
            if (Session["Login"]==null)
            {
                return RedirectToAction("Index", "SignIn");
            }
            DatabaseContext db = new DatabaseContext();
            List<Models.Users> users= db.Users.ToList();
            foreach (Users u in users)
            {
                if (Session["Login"].ToString()==u.Username)
                {

                    return View(u);


                }

            }

            return RedirectToAction("Index", "SignIn");

        }
        public ActionResult Create()
        {
            TicketwithListViewModel model = new TicketwithListViewModel();
            List <SelectListItem> list = new List<SelectListItem>();
            SelectListItem Şikayet = new SelectListItem()
            {
                Text = "Şikayet",Value = Type.Şikayet.ToString()
            };
            SelectListItem Öneri = new SelectListItem()
            {
                Text = "Öneri",
                Value = Type.Öneri.ToString()
            };
            SelectListItem Görüş = new SelectListItem()
            {
                Text = "Görüş",
                Value = Type.Görüş.ToString()
            };
            list.Add(Şikayet);
            list.Add(Görüş);
            list.Add(Öneri);


            model.SelectListItems = list;



            return View(model);
        }
        [HttpPost,ValidateAntiForgeryToken]
        public ActionResult Create(TicketwithListViewModel t,HttpPostedFileBase file)
        {
            List<string> acceptedExtensions = new List<string>()
            {
                ".pdf",
                ".xls",
                ".xlsx",
                ".jpg",
                ".docx",
                ".doc",
                ".jpeg"
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
                    t.Ticket.FilePath = Path.Combine(Server.MapPath("~/userfiles"), file.FileName);

                }
                else
                {
                    return View("Error");
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

    }
}