using System.Collections.Generic;
using Ticket.Models;

namespace Ticket.ViewModels
{
    public class ListAssignmentAndTicket
    {
        public List<Assignment> Assignments { get; set; }
        public Models.Ticket Ticket { get; set; }
    }
}