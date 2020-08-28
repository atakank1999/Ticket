using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages.Html;
using Ticket.Models;
using Ticket.Models.Context;
using Ticket.ViewModels;
using SelectListItem = System.Web.Mvc.SelectListItem;
using Ticket = Ticket.Models.Ticket;

namespace Ticket.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            DatabaseContext db = new DatabaseContext();
            ListofUserAndTicketViewModel model = new ListofUserAndTicketViewModel();
             model.TicketsList = db.Tickets.ToList();
             foreach (Users u in db.Users.ToList())
             {
                 if (u.IsAdmin)
                 {
                     model.AssignList.Add(new SelectListItem{Text = u.Username,Value = u.ID.ToString()});
                 }
             }
             model.PriorityList.Add(new SelectListItem{Text = "Düşük",Value = Priority.Dusuk.ToString(), Selected = false });
             model.PriorityList.Add(new SelectListItem {Text = "Orta", Value = Priority.Orta.ToString() , Selected = false });
             model.PriorityList.Add(new SelectListItem {Text = "Önemli",Value = Priority.Onemli.ToString(), Selected = false });
             model.PriorityList.Add(new SelectListItem {Text = "Acil", Value = Priority.Acil.ToString() ,Selected = false});




            return View(model);
        }
        public ActionResult Login()
        {
            return View(new Users());
        }
        [HttpPost,ValidateAntiForgeryToken]
        public ActionResult Login(Users admin)
        {
            DatabaseContext db = new DatabaseContext();
            List<Users>usersList =db.Users.ToList();
            foreach (Users u in usersList)
            {
                if (admin.Email== u.Email && admin.Password == u.Password && u.IsAdmin)
                {
                    Session["Admin"] = u.Username;
                    return RedirectToAction("Index");
                }
            }

            return View(admin);
        }
        public ActionResult Reply()
        {
            return View(new Reply());
        }

        [HttpPost,ValidateAntiForgeryToken]
        public ActionResult Reply(int id,Reply reply)
        {
            DatabaseContext db = new DatabaseContext();
            List<Users> userlist = db.Users.ToList();
            List<Models.Ticket> ticketlList = db.Tickets.ToList();
            foreach (Users u in userlist)
            {
                if (u.Username == Session["Admin"].ToString())
                {
                    reply.WriterAdmin = u;
                }
            }

            foreach (Models.Ticket t in ticketlList)
            {
                if (t.Id == id)
                {
                    reply.RepliedTicket = t;
                }
            }

            db.Replies.Add(reply);
            db.SaveChanges();



            return RedirectToAction("Index");
        }
        [HttpPost]
        public PartialViewResult Assign(ListofUserAndTicketViewModel p)
        {
            DatabaseContext db = new DatabaseContext();
            Models.Ticket t = db.Tickets.Find(p.ID);
            Users u = db.Users.Find(p.Admin.ID);

            List<Assignment> alList = db.Assignments.ToList();
            foreach (Assignment a in alList)
            {
                if (a.Ticket==t)
                {
                    a.Admin = u;
                }
            }
            Assignment assignment = new Assignment();
            assignment.Admin = u;
            assignment.Ticket = t;
            assignment.IsDone = false;
            db.Assignments.Add(assignment);
            int result =db.SaveChanges();
            List<String> list = new List<String>();

            if (result != 0)
            {
                list.Add("success");
                list.Add("Değişiklikler uygulanmıştır");
            }
            else
            {
                list.Add("danger");
                list.Add("Değişikler uygulanamamıştır ya da değişiklik olmamıştır");
            }



            return PartialView("_Success",list);
        }
        [HttpPost]
        public PartialViewResult PriorityResult(ListofUserAndTicketViewModel model)
        {
            DatabaseContext db = new DatabaseContext();
            int result = -1;
            Models.Ticket ticket = db.Tickets.Find(model.TicketID);
            List<String> list = new List<String>();
            if (ticket != null)
            {
                if (ticket.Priority != model.Tickets.Priority)
                {
                    ticket.Priority = model.Tickets.Priority;
                    result = db.SaveChanges();
                }



            }
            if (result != 0 )
            {
               
                list.Add("success");
                list.Add("Değişiklikler uygulanmıştır");

            }
            else
            {
                list.Add("danger");
                list.Add("Değişikler uygulanamamıştır");
            }

            return PartialView("_Success",list);
        }
    }
}