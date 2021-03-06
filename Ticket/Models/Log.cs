﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Ticket.Models
{
    [Table("Logs")]
    public class Log
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public string ObjecType { get; set; }

        [Required]
        public string routevalues { get; set; }

        public virtual Users Users { get; set; }
        public virtual Assignment Assignment { get; set; }
        public virtual Reply Reply { get; set; }
        public virtual Ticket Ticket { get; set; }
        public virtual Ticket NexTicket { get; set; }
        public virtual Ticket PreviousTicket { get; set; }
        public virtual Reply NextReply { get; set; }

        public virtual Reply PreviousReply { get; set; }
        public virtual Assignment NextAssignment { get; set; }

        public virtual Assignment PreviousAssignment { get; set; }
        public virtual Users NextUsers { get; set; }

        public virtual Users PreviousUsers { get; set; }

        public string IP { get; set; }

        public DateTime Time { get; set; }
        public string Type { get; set; }
    }
}