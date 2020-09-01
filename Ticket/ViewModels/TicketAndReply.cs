using System.Collections.Generic;
using Ticket.Models;

namespace Ticket.ViewModels
{
    public class TicketAndReply
    {
        public List<Models.Ticket> Ticket { get; set; }
        public List<Reply> Reply { get; set; }
        public int ID { get; set; }
    }
}