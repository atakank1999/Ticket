using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ticket.ViewModels
{
    public class TicketwithListViewModel
    {
        public List<SelectListItem> SelectListItems { get; set; }
        public Models.Ticket Ticket { get; set; }
    }
}