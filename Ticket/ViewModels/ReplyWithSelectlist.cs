using System.Collections.Generic;
using System.Web.Mvc;
using Ticket.Models;

namespace Ticket.ViewModels
{
    public class ReplyWithSelectlist
    {
        public Reply Reply { get; set; }
        public List<SelectListItem> SelectListItems { get; set; }
        public List<SelectListItem> SelecListStatus { get; set; }
    }
}