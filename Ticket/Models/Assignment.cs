using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Ticket.Models
{
    [Table("Assignments")]
    public class Assignment
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public virtual Ticket Ticket { get; set; }
        public virtual Users Admin { get; set; }
        public bool IsDone { get; set; }
    }
}