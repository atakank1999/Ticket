using System;
using System.Collections.Generic;
using System.IO;
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
             model.StatusList.Add(new SelectListItem()
             {
                 Value = Status.Başlamadı.ToString(),
                 Text = "Başlamadı"
             });
             model.StatusList.Add(new SelectListItem()
             {
                 Value = Status.Sürüyor.ToString(),
                 Text = "Sürüyor"
             });
             model.StatusList.Add(new SelectListItem()
             {
                 Value = Status.Çözüldü.ToString(),
                 Text = "Çözüldü"
             });



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
                    Session["Login"] = u.Username;
                    return RedirectToAction("Index");
                }
            }

            return View(admin);
        }
        public ActionResult Reply()
        {
            ReplyWithSelectlist replyWithSelectlist = new ReplyWithSelectlist();
            replyWithSelectlist.SelectListItems = new List<SelectListItem>();
            replyWithSelectlist.SelecListStatus = new List<SelectListItem>();
            replyWithSelectlist.SelectListItems.Add(new SelectListItem()
            {
                Value = Status.Başlamadı.ToString(),
                Text = "Başlamadı"
            });
            replyWithSelectlist.SelectListItems.Add(new SelectListItem()
            {
                Value = Status.Sürüyor.ToString(),
                Text = "Sürüyor"
            });
            replyWithSelectlist.SelectListItems.Add(new SelectListItem()
            {
                Value = Status.Çözüldü.ToString(),
                Text = "Çözüldü"
            });
            replyWithSelectlist.SelecListStatus.Add(new SelectListItem { Text = "Düşük", Value = Priority.Dusuk.ToString(), Selected = false });
            replyWithSelectlist.SelecListStatus.Add(new SelectListItem { Text = "Orta", Value = Priority.Orta.ToString(), Selected = false });
            replyWithSelectlist.SelecListStatus.Add(new SelectListItem { Text = "Önemli", Value = Priority.Onemli.ToString(), Selected = false });
            replyWithSelectlist.SelecListStatus.Add(new SelectListItem { Text = "Acil", Value = Priority.Acil.ToString(), Selected = false });
            return View(replyWithSelectlist);
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
            reply.date = DateTime.UtcNow;
            reply.RepliedTicket.Status = reply.RepliedTicket.Status;

            db.Replies.Add(reply);
            db.SaveChanges();



            return RedirectToAction("Index");
        }
        [HttpPost]
        public PartialViewResult Assign(ListofUserAndTicketViewModel p)
        {
            List<String> list = new List<String>();

            if (p.Admin.ID!=0)
            {
                DatabaseContext db = new DatabaseContext();
                Models.Ticket t = db.Tickets.Find(p.ID);
                Users u = db.Users.Find(p.Admin.ID);
                Assignment assignment = null;

                List<Assignment> alList = db.Assignments.ToList();
                foreach (Assignment a in alList)
                {
                    if (a.Ticket == t)
                    {
                        assignment = a;

                        if (DateTime.Compare(p.Deadline, DateTime.UtcNow) > 0)
                        {
                            assignment.Deadline = p.Deadline;
                        }
                        else
                        {
                            assignment.Deadline = null;
                        }

                    }
                }

                if (assignment != null)
                {
                    assignment.Admin = u;
                }
                else if (u != null)
                {

                }
                {
                    assignment = new Assignment();
                    assignment.Ticket = t;
                    assignment.IsDone = false;
                    assignment.Admin = u;
                    if (DateTime.Compare(p.Deadline, DateTime.UtcNow) > 0)
                    {
                        assignment.Deadline = p.Deadline;
                    }
                    else
                    {
                        assignment.Deadline = null;
                    }

                    db.Assignments.Add(assignment);

                }
                int result = db.SaveChanges();

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
        [HttpPost]
        public PartialViewResult PriorityResultAssignment(ListAssignmentAndTicket model)
        {
            DatabaseContext db = new DatabaseContext();
            int result = -1;
            Models.Ticket ticket = db.Tickets.Find(model.Ticket.Id);
            List<String> list = new List<String>();
            if (ticket != null)
            {
                if (ticket.Priority != model.Ticket.Priority)
                {
                    ticket.Priority = model.Ticket.Priority;
                    result = db.SaveChanges();
                }



            }
            if (result != 0)
            {

                list.Add("success");
                list.Add("Değişiklikler uygulanmıştır");

            }
            else
            {
                list.Add("danger");
                list.Add("Değişikler uygulanamamıştır");
            }

            return PartialView("_Success", list);
        }
        [HttpPost]
        public ActionResult StatusResult(ListofUserAndTicketViewModel model)
        {
            DatabaseContext db = new DatabaseContext();
            int result = -1;
            Models.Ticket ticket = db.Tickets.Find(model.TicketID);
            List<String> list = new List<String>();
            if (ticket != null)
            {
                if (ticket.Status != model.Tickets.Status)
                {
                    ticket.Status = model.Tickets.Status;
                    result = db.SaveChanges();
                }



            }
            if (result != 0)
            {

                list.Add("success");
                list.Add("Değişiklikler uygulanmıştır");

            }
            else
            {
                list.Add("danger");
                list.Add("Değişikler uygulanamamıştır");
            }

            return PartialView("_Success", list);
        }
        [HttpPost]
        public ActionResult StatusResultAssignments(ListAssignmentAndTicket model)
        {
            DatabaseContext db = new DatabaseContext();
            int result = -1;
            Models.Ticket ticket = db.Tickets.Find(model.Ticket.Id);
            List<String> list = new List<String>();
            if (ticket != null)
            {
                if (ticket.Status != model.Ticket.Status)
                {
                    ticket.Status = model.Ticket.Status;
                    result = db.SaveChanges();
                }



            }
            if (result != 0)
            {

                list.Add("success");
                list.Add("Değişiklikler uygulanmıştır");

            }
            else
            {
                list.Add("danger");
                list.Add("Değişikler uygulanamamıştır");
            }

            return PartialView("_Success", list);
        }
        public ActionResult Download(int id)
        {
            DatabaseContext db = new DatabaseContext();
            Models.Ticket ticket = db.Tickets.Find(id);
            string path = ticket.FilePath;
            return File(path, "application/force-download",Path.GetFileName(path));
        }
        public ActionResult Assignments()
        {
            DatabaseContext db =new DatabaseContext();
            Users modeluser = null;
            foreach (Users u in db.Users.ToList())
            {
                if (Session["Login"].ToString()==u.Username)
                {
                    modeluser = u;
                }
            }
            
            ListAssignmentAndTicket model = new ListAssignmentAndTicket();
            model.Assignments = modeluser.Assignments;

            return View(model);
        }
    }
}