using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Ticket.Models
{
    [Table("Replies")]
    public class Reply
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public virtual Users WriterAdmin { get; set; }

        public virtual Ticket RepliedTicket { get; set; }
        [Required]
        public string Text { get; set; }

    }
}