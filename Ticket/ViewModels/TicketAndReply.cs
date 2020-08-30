using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Ticket.Models;

namespace Ticket.ViewModels
{
    public class TicketAndReply
    {
        public List<Models.Ticket> Ticket { get; set; }
        public List<Reply>  Reply { get; set; }
        public int ID { get; set; }
    }
}