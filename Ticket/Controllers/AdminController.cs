using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Ticket.Comparer;
using Ticket.Filters;
using Ticket.Models;
using Ticket.Models.Automation;
using Ticket.Models.Context;
using Ticket.ViewModels;
using Assignment = Ticket.Models.Assignment;
using SelectListItem = System.Web.Mvc.SelectListItem;
using Ticket = Ticket.Models.Ticket;

namespace Ticket.Controllers
{
    [LogFilter]
    [AdminFilter]
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index(string sortby = "-date")
        {
            DatabaseContext db = new DatabaseContext();

            List<Models.Ticket> model = db.Tickets.Where(x => !x.IsDeleted).ToList();

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

            model = model.Where(x => !x.IsDeleted).ToList();

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
            List<Users> usersList = db.Users.Where(x => !x.IsDeleted).ToList();
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
            List<Users> userlist = db.Users.Where(x => !x.IsDeleted).ToList();
            List<Models.Ticket> ticketlList = db.Tickets.Where(x => !x.IsDeleted).ToList();
            foreach (Users u in userlist)
            {
                if (u.Username == Session["Admin"].ToString() && !u.IsDeleted)
                {
                    reply.WriterAdmin = u;
                }
            }

            foreach (Models.Ticket t in ticketlList)
            {
                if (t.Id == id && !t.IsDeleted)
                {
                    reply.RepliedTicket = t;
                }
            }
            reply.date = DateTime.UtcNow;
            reply.RepliedTicket.Status = reply.RepliedTicket.Status;
            reply.RepliedTicket.EditedOn = DateTime.Now;
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
                if (t.IsDeleted)
                {
                    list.Add("danger");
                    list.Add("Bu bilet silinmiş.");
                    return PartialView("_Success", list);
                }
                Users u = db.Users.Find(p.Ticket.assignedTo.Admin.ID);
                Assignment assignment = null;

                List<Assignment> alList = db.Assignments.Where(x => !x.IsDeleted).ToList();
                foreach (Assignment a in alList)
                {
                    if (a.Ticket == t)
                    {
                        assignment = a;

                        if (DateTime.Compare(p.Ticket.assignedTo.Deadline.Date.Add(p.TimeSpan), DateTime.Now) > 0)
                        {
                            Models.Ticket oldt = new Models.Ticket(t);

                            Assignment old = new Assignment();
                            old.IsDeleted = true;
                            old.Admin = a.Admin;
                            old.Ticket = oldt;
                            old.Deadline = a.Deadline;
                            old.IsDone = a.IsDone;
                            oldt.EditedOn = DateTime.Now;
                            oldt.IsDeleted = true;
                            assignment.Deadline = p.Ticket.assignedTo.Deadline.Date.Add(p.TimeSpan);
                            assignment.Admin = u;
                            Log l = new Log();
                            l.ObjecType = typeof(Assignment).ToString();
                            l.Assignment = assignment;
                            l.Type = "Modified";
                            string user = Session["Login"].ToString();
                            l.Users = db.Users.Where(x => x.Username == user && x.IsDeleted == false).FirstOrDefault();
                            l.PreviousAssignment = old;
                            l.NextAssignment = assignment;
                            l.IP = HttpContext.Request.UserHostAddress;
                            l.Time = DateTime.Now;
                            l.routevalues = HttpContext.Request.Url.PathAndQuery;

                            db.Tickets.Add(oldt);
                            db.Logs.Add(l);
                            db.Assignments.Add(old);
                        }
                        else
                        {
                            list.Add("danger");
                            list.Add("Lütfen geçerli bir tarih giriniz");
                            return PartialView("_Success", list);
                        }
                    }
                }

                if (u != null && assignment == null)
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
            foreach (Users u in db.Users.Where(x => !x.IsDeleted))
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

            if (model.Ticket.IsDeleted)
            {
                return RedirectToAction("Index");
            }
            return View(model);
        }

        [HttpPost]
        public PartialViewResult PriorityResult(AdminTicketViewModel model)
        {
            DatabaseContext db = new DatabaseContext();
            int result = -1;
            Models.Ticket ticket = db.Tickets.Find(model.Ticket.Id);
            Models.Ticket oldticket = new Models.Ticket(ticket);
            oldticket.IsDeleted = true;
            db.Tickets.Add(oldticket);
            Log l = new Log();
            l.ObjecType = typeof(Models.Ticket).ToString();
            l.Type = "Modified";
            string user = Session["Login"].ToString();
            l.Users = db.Users.Where(x => x.Username == user && x.IsDeleted == false).FirstOrDefault();
            l.PreviousTicket = oldticket;
            l.NexTicket = ticket;
            l.IP = HttpContext.Request.UserHostAddress;
            l.Time = DateTime.Now;
            l.routevalues = HttpContext.Request.Url.PathAndQuery;

            List<String> list = new List<String>();
            if (ticket != null && !ticket.IsDeleted)
            {
                if (ticket.Priority != model.Ticket.Priority)
                {
                    ticket.Priority = model.Ticket.Priority;
                    ticket.EditedOn = DateTime.Now;
                    l.Ticket = ticket;
                    db.Logs.Add(l);
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
            Models.Ticket oldticket = new Models.Ticket(ticket);
            oldticket.IsDeleted = true;
            db.Tickets.Add(oldticket);
            Log l = new Log();
            l.ObjecType = typeof(Models.Ticket).ToString();
            l.Type = "Modified";
            string user = Session["Login"].ToString();
            l.Users = db.Users.Where(x => x.Username == user && x.IsDeleted == false).FirstOrDefault();
            l.NexTicket = ticket;
            l.PreviousTicket = oldticket;
            l.IP = HttpContext.Request.UserHostAddress;
            l.Time = DateTime.Now;
            l.routevalues = HttpContext.Request.Url.PathAndQuery;
            List<String> list = new List<String>();
            if (ticket != null && !ticket.IsDeleted)
            {
                if (ticket.Status != model.Ticket.Status)
                {
                    ticket.Status = model.Ticket.Status;
                    ticket.EditedOn = DateTime.Now;
                    l.Ticket = ticket;
                    db.Logs.Add(l);
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
            if (ticket.IsDeleted)
            {
                return RedirectToAction("Index");
            }
            return File(path, "application/force-download", Path.GetFileName(path));
        }

        public ActionResult Logs(int? user, int? ticket, int? assignment, int? reply, string sortby = "-date")
        {
            DatabaseContext db = new DatabaseContext();
            List<Log> model = db.Logs.OrderBy(x => x.Time).ToList();
            if (sortby == "date")
            {
                model = db.Logs.OrderBy(x => x.Time).ToList();
            }
            else if (sortby == "-date")
            {
                model = db.Logs.OrderByDescending(x => x.Time).ToList();
            }
            else if (sortby == "-process")
            {
                model = db.Logs.OrderByDescending(x => x.Type).ToList();
            }
            else if (sortby == "process")
            {
                model = db.Logs.OrderBy(x => x.Type).ToList();
            }
            else if (sortby == "user")
            {
                model = db.Logs.OrderBy(x => x.Users.Username).ToList();
            }
            else if (sortby == "-user")
            {
                model = db.Logs.OrderByDescending(x => x.Users.Username).ToList();
            }
            else if (sortby == "-type")
            {
                model = db.Logs.OrderByDescending(x => x.ObjecType).ToList();
            }
            else if (sortby == "type")
            {
                model = db.Logs.OrderBy(x => x.ObjecType).ToList();
            }
            else if (sortby == "url")
            {
                model = db.Logs.OrderBy(x => x.routevalues).ToList();
            }
            else if (sortby == "-url")
            {
                model = db.Logs.OrderByDescending(x => x.routevalues).ToList();
            }
            else if (sortby == "admin")
            {
                model = db.Logs.OrderBy(x => x.Users.IsAdmin).ToList();
            }
            else if (sortby == "-admin")
            {
                model = db.Logs.OrderByDescending(x => x.Users.IsAdmin).ToList();
            }
            else if (sortby == "-IP")
            {
                model = db.Logs.OrderByDescending(x => x.IP).ToList();
            }
            else if (sortby == "IP")
            {
                model = db.Logs.OrderBy(x => x.IP).ToList();
            }

            foreach (Log log in model)
            {
                switch (log.Type)
                {
                    case "Added":
                        log.Type = "Eklendi";
                        break;

                    case "Modified":
                        log.Type = "Düzenlendi";
                        break;

                    case "Deleted":
                        log.Type = "Silindi";
                        break;
                }

                switch (log.ObjecType)
                {
                    case "Ticket.Models.Users":
                        log.ObjecType = "Kullanıcı";
                        break;

                    case "Ticket.Models.Ticket":
                        log.ObjecType = "Bilet";
                        break;

                    case "Ticket.Models.Assignment":
                        log.ObjecType = "Atama";
                        break;

                    case "Ticket.Models.Reply":
                        log.ObjecType = "Yanıt";
                        break;
                }
            }

            if (user != null)
            {
                model = model.Where(x => x.Users != null && x.Users.ID == user).ToList();
            }

            if (ticket != null)
            {
                model = model.Where(x => x.Ticket != null && x.Ticket.Id == ticket).ToList();
            }

            if (reply != null)
            {
                model = model.Where(x => x.Reply != null && x.Reply.ID == reply).ToList();
            }

            if (assignment != null)
            {
                model = model.Where(x => x.Assignment != null && x.Assignment.ID == assignment).ToList();
            }

            return View(model);
        }

        public ActionResult Log(int id)
        {
            DatabaseContext db = new DatabaseContext();
            Log log = db.Logs.Find(id);

            switch (log.Type)
            {
                case "Added":
                    log.Type = "Eklendi";
                    break;

                case "Modified":
                    log.Type = "Düzenlendi";
                    break;

                case "Deleted":
                    log.Type = "Silindi";
                    break;
            }

            switch (log.ObjecType)
            {
                case "Ticket.Models.Users":
                    log.ObjecType = "Kullanıcı";
                    List<Log> loglist2 = db.Logs.Where(x => x.Users.ID == log.Users.ID).ToList();

                    int index2 = loglist2.FindIndex(x => x.ID == log.ID);

                    if (index2 <= loglist2.Count - 2)
                    {
                        log.NextUsers = loglist2[index2 + 1].PreviousUsers;
                    }

                    break;

                case "Ticket.Models.Ticket":
                    log.ObjecType = "Bilet";
                    List<Log> loglist = db.Logs.Where(x => x.Ticket.Id == log.Ticket.Id).ToList();

                    int index = loglist.FindIndex(x => x.ID == log.ID);
                    if (index <= loglist.Count - 2)
                    {
                        log.NexTicket = loglist[index + 1].PreviousTicket;
                    }
                    break;

                case "Ticket.Models.Assignment":
                    log.ObjecType = "Atama";
                    List<Log> loglist3 = db.Logs.Where(x => x.Assignment == log.Assignment).ToList();

                    int index3 = loglist3.FindIndex(x => x.ID == log.ID);

                    if (index3 <= loglist3.Count - 2)
                    {
                        log.NextAssignment = loglist3[index3 + 1].PreviousAssignment;
                    }
                    break;

                case "Ticket.Models.Reply":
                    log.ObjecType = "Yanıt";
                    List<Log> loglist4 = db.Logs.Where(x => x.Reply == log.Reply).ToList();

                    int index4 = loglist4.FindIndex(x => x.ID == log.ID);

                    if (index4 <= loglist4.Count - 2)
                    {
                        log.NextReply = loglist4[index4 + 1].PreviousReply;
                    }
                    break;
            }

            return View(log);
        }
    }
}