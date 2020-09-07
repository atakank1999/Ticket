using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.UI.WebControls.Expressions;
using System.Web.WebPages.Html;
using Quartz;
using Quartz.Impl;
using Quartz.Util;
using Ticket.Comparer;
using Ticket.Filters;
using Ticket.Migrations;
using Ticket.Models;
using Ticket.Models.Automation;
using Ticket.Models.Context;
using Ticket.Models.Quartz;
using Ticket.ViewModels;
using Assignment = Ticket.Models.Assignment;
using SelectListItem = System.Web.Mvc.SelectListItem;
using Ticket = Ticket.Models.Ticket;

namespace Ticket.Controllers
{
    [AdminFilter]
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index(string sortby)
        {
            DatabaseContext db = new DatabaseContext();

            List<Models.Ticket> model = db.Tickets.ToList();

            //foreach (Users u in db.Users.ToList())
            //{
            //    if (u.IsAdmin)
            //    {
            //        model.AssignList.Add(new SelectListItem { Text = u.Username, Value = u.ID.ToString() });
            //    }
            //}
            //model.PriorityList.Add(new SelectListItem { Text = "Düşük", Value = Priority.Dusuk.ToString(), Selected = false });
            //model.PriorityList.Add(new SelectListItem { Text = "Orta", Value = Priority.Orta.ToString(), Selected = false });
            //model.PriorityList.Add(new SelectListItem { Text = "Önemli", Value = Priority.Onemli.ToString(), Selected = false });
            //model.PriorityList.Add(new SelectListItem { Text = "Acil", Value = Priority.Acil.ToString(), Selected = false });
            //model.StatusList.Add(new SelectListItem()
            //{
            //    Value = Status.Başlamadı.ToString(),
            //    Text = "Başlamadı"
            //});
            //model.StatusList.Add(new SelectListItem()
            //{
            //    Value = Status.Sürüyor.ToString(),
            //    Text = "Sürüyor"
            //});
            //model.StatusList.Add(new SelectListItem()
            //{
            //    Value = Status.Çözüldü.ToString(),
            //    Text = "Çözüldü"
            //});
            if (sortby == "name")
            {
                model = model.OrderBy(o => o.Title).ToList();
            }
            else if (sortby == "-name")
            {
                model = model.OrderByDescending(o => o.Title).ToList();
            }
            else if (sortby == "date")
            {
                model = model.OrderBy(o => o.DateTime).ToList();
            }
            if (sortby == "-date")
            {
                model = model.OrderByDescending(o => o.DateTime).ToList();
            }
            else if (sortby == "status")
            {
                model = model.OrderBy(o => o.Status).ToList();
            }
            else if (sortby == "-status")
            {
                model = model.OrderByDescending(o => o.Status).ToList();
            }
            else if (sortby == "type")
            {
                model = model.OrderBy(o => o.Type).ToList();
            }
            else if (sortby == "-type")
            {
                model = model.OrderByDescending(o => o.Type).ToList();
            }
            else if (sortby == "admin")
            {
                model = model.OrderBy(o => o.assignedTo, new AdminComparer()).ToList();
            }
            else if (sortby == "-admin")
            {
                model = model = model.OrderByDescending(o => o.assignedTo, new AdminComparer()).ToList();
            }
            else if (sortby == "deadline")
            {
                model = model.OrderBy(o => o.assignedTo, new DeadlineComparer()).ToList();
            }
            else if (sortby == "-deadline")
            {
                model = model.OrderByDescending(o => o.assignedTo, new DeadlineComparer()).ToList();
            }

            return View(model);
        }

        public ActionResult Login()
        {
            return View(new Users());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Login(Users admin)
        {
            DatabaseContext db = new DatabaseContext();
            List<Users> usersList = db.Users.ToList();
            foreach (Users u in usersList)
            {
                if (admin.Email == u.Email && Crypto.Hash(admin.Password) == u.Password && u.IsAdmin)
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

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<ActionResult> Reply(int id, Reply reply)
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
            int result = db.SaveChanges();
            if (result > 0)
            {
                MailMessage message = new MailMessage();
                message.To.Add(new MailAddress(reply.RepliedTicket.Author.Email.ToString()));
                message.Subject = reply.RepliedTicket.Title + " başlıklı biletinize yanıt geldi";
                message.Body = reply.Text;
                using (var smtp = new SmtpClient())
                {
                    await smtp.SendMailAsync(message);
                }
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Assign(AdminTicketViewModel p)
        {
            List<String> list = new List<String>();

            if (p.Ticket.assignedTo.Admin.ID != 0)
            {
                DatabaseContext db = new DatabaseContext();
                Models.Ticket t = db.Tickets.Find(p.Ticket.Id);
                Users u = db.Users.Find(p.Ticket.assignedTo.Admin.ID);
                Assignment assignment = null;

                List<Assignment> alList = db.Assignments.ToList();
                foreach (Assignment a in alList)
                {
                    if (a.Ticket == t)
                    {
                        assignment = a;

                        if (DateTime.Compare(p.Ticket.assignedTo.Deadline.Date.Add(p.TimeSpan), DateTime.Now) > 0)
                        {
                            assignment.Deadline = p.Ticket.assignedTo.Deadline.Date.Add(p.TimeSpan);
                        }
                        else
                        {
                            list.Add("danger");
                            list.Add("Lütfen geçerli bir tarih giriniz");
                            return PartialView("_Success", list);
                        }
                    }
                }

                if (assignment != null)
                {
                    assignment.Admin = u;
                }
                else if (u != null)
                {
                    assignment = new Assignment();
                    assignment.Ticket = t;
                    assignment.IsDone = false;
                    assignment.Admin = u;
                    if (DateTime.Compare(p.Ticket.assignedTo.Deadline.Date.Add(p.TimeSpan), DateTime.Now) > 0)
                    {
                        assignment.Deadline = p.Ticket.assignedTo.Deadline.Date.Add(p.TimeSpan);
                    }
                    else
                    {
                        list.Add("danger");
                        list.Add("Geçerli bir tarih giriniz");
                        return PartialView("_Success", list);
                    }

                    db.Assignments.Add(assignment);
                }
                int result = db.SaveChanges();

                if (result != 0)
                {
                    list.Add("success");
                    list.Add("Değişiklikler uygulanmıştır");

                    //IJobDetail job = JobBuilder.Create<EmailClass>()  //send email every 6 hours
                    //    .WithIdentity("myJob", "group1")
                    //    .UsingJobData("Email", assignment.Admin.Email)
                    //    .UsingJobData("Time", assignment.Deadline.Subtract(DateTime.Now).Hours.ToString())
                    //    .UsingJobData("Body", assignment.Ticket.Text)
                    //    .UsingJobData("Seconds",assignment.Deadline.Subtract(DateTime.Now).Seconds.ToString())
                    //    .UsingJobData("Title",assignment.Ticket.Title).Build();
                    //ISimpleTrigger trigger = (ISimpleTrigger) TriggerBuilder.Create()
                    //    .WithIdentity("trigger1", "group1")
                    //    .StartAt(assignment.Deadline.AddDays(-1))
                    //    .WithSimpleSchedule(x=>x.WithIntervalInSeconds(2))
                    //    .ForJob("myJob", "group1")
                    //    .EndAt(assignment.Deadline)
                    //    .Build();
                    //StdSchedulerFactory factory = new StdSchedulerFactory();
                    //IScheduler scheduler = await factory.GetScheduler();
                    //await scheduler.Start();
                    //await scheduler.ScheduleJob(job, trigger);
                    //ISimpleTrigger trigger2 = (ISimpleTrigger) TriggerBuilder.Create()
                    //    .WithIdentity("trigger2", "group1")
                    //    .StartAt(assignment.Deadline.AddSeconds(-10))
                    //    .WithSimpleSchedule(x => x.WithIntervalInSeconds(1).WithRepeatCount(0))
                    //    .ForJob("myJob", "group1")
                    //    .Build();
                    //StdSchedulerFactory factory2 = new StdSchedulerFactory();

                    //IScheduler scheduler2 = await factory2.GetScheduler();
                    //await scheduler2.Start();
                    //await scheduler2.ScheduleJob(job, trigger2);
                    Email email = new Email();
                    DateTime deadline = assignment.Deadline;
                    deadline = DateTime.SpecifyKind(deadline, DateTimeKind.Local);
                    DateTimeOffset deadline2 = deadline;
                    long unixTimeSeconds = deadline2.ToUnixTimeSeconds();

                    email.run_cmd(Server.MapPath("~/SendMail/sendmail.py"), assignment.Ticket.Author.Email.ToString() + " " + assignment.Ticket.Text + " " + assignment.Ticket.Title + " " + unixTimeSeconds);
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
                list.Add("Lütfen geçerli bir kişi seçiniz");
            }

            return PartialView("_Success", list);
        }

        public ActionResult Ticket(int id)
        {
            AdminTicketViewModel model = new AdminTicketViewModel();
            DatabaseContext db = new DatabaseContext();
            model.Admins = new List<Users>();
            foreach (Users u in db.Users)
            {
                if (u.IsAdmin)
                {
                    model.Admins.Add(u);
                }
            }

            model.Ticket = db.Tickets.Find(id);
            if (model.Ticket.assignedTo != null)
            {
                model.TimeSpan = model.Ticket.assignedTo.Deadline.TimeOfDay;
            }
            model.AdminList = new List<SelectListItem>();
            foreach (Users u in model.Admins)
            {
                model.AdminList.Add(new SelectListItem()
                {
                    Text = u.Username,
                    Value = u.ID.ToString(),
                });
            }

            return View(model);
        }

        [HttpPost]
        public PartialViewResult PriorityResult(AdminTicketViewModel model)
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
        public PartialViewResult PriorityResultAssignment(AdminTicketViewModel model)
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
        public ActionResult StatusResult(AdminTicketViewModel model)
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
            return File(path, "application/force-download", Path.GetFileName(path));
        }

        public ActionResult Assignments()
        {
            DatabaseContext db = new DatabaseContext();
            Users modeluser = null;
            foreach (Users u in db.Users.ToList())
            {
                if (Session["Login"].ToString() == u.Username)
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