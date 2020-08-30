using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ticket.Models;
using Ticket = Ticket.Models.Ticket;

namespace Ticket.ViewModels
{
    public class ListofUserAndTicketViewModel
    {
        public List<Models.Ticket> TicketsList { get; set; }
        public List<SelectListItem> AssignList { get; set; }
        public List<SelectListItem> PriorityList { get; set; }
        public List<SelectListItem> StatusList { get; set; }
        public Users Admin { get; set; }
        public Models.Ticket Tickets { get; set; }
        public int ID { get; set; }
        public int TicketID { get; set; }
        public DateTime Deadline { get; set; }

        public ListofUserAndTicketViewModel()
        {
            TicketsList = new List<Models.Ticket>();
            AssignList = new List<SelectListItem>();
            PriorityList = new List<SelectListItem>();
            StatusList = new List<SelectListItem>();
            Admin = new Users();
            Tickets = new Models.Ticket();
        }
    }
}