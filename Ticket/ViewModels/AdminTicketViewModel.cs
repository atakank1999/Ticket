using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ticket.Models;
using Type = Ticket.Models.Type;

namespace Ticket.ViewModels
{
    public class AdminTicketViewModel
    {
        public Models.Ticket Ticket { get; set; }
        public List<Users> Admins { get; set; }
        public List<SelectListItem> AdminList { get; set; }
        public TimeSpan TimeSpan { get; set; }
    }
}