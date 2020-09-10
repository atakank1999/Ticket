using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ticket.Models
{
    [Table("Replies")]
    public class Reply
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public virtual Users WriterAdmin { get; set; }

        public virtual Ticket RepliedTicket { get; set; }

        [Required(ErrorMessage = "Boş geçilemez")]
        public string Text { get; set; }

        public DateTime date { get; set; }
        public virtual List<Log> Logs { get; set; }
        public bool IsDeleted { get; set; }

        public Reply()
        {
            date = DateTime.Now;
            this.IsDeleted = false;
        }

        public Reply(Reply r)
        {
            if (r != null)
            {
                this.WriterAdmin = r.WriterAdmin;
                this.RepliedTicket = r.RepliedTicket;
                this.IsDeleted = r.IsDeleted;
                this.Text = r.Text;
                this.date = r.date;
            }
        }
    }
}