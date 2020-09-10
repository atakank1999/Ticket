using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.UI.WebControls;

namespace Ticket.Models
{
    public enum Priority
    {
        Yok,
        Acil,
        Onemli,
        Orta,
        Dusuk
    }

    public enum Type
    {
        Yok,
        Şikayet,
        Görüş,
        Öneri
    }

    public enum Status
    {
        Başlamadı,
        Sürüyor,
        Çözüldü
    }

    [Table("Tickets")]
    public class Ticket
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "Boş Geçilemez"), Display(Name = "Başlık")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Boş Geçilemez"), Display(Name = "Biletinizin içeriği")]
        public string Text { get; set; }

        public virtual Users Author { get; set; }
        public virtual List<Reply> Replies { get; set; }
        public string FilePath { get; set; }
        public Priority Priority { get; set; }

        [Display(Name = "Biletinizin Tipi")]
        public Type Type { get; set; }

        public virtual Assignment assignedTo { get; set; }
        public DateTime DateTime { get; set; }
        public Status Status { get; set; }
        public DateTime EditedOn { get; set; }
        public virtual List<Log> Logs { get; set; }
        public bool IsDeleted { get; set; }

        public Ticket()
        {
            DateTime = DateTime.Now;
            EditedOn = DateTime.Now;
        }

        public Ticket(Ticket oldticket)
        {
            this.Title = oldticket.Title;
            this.Text = oldticket.Text;
            this.Author = oldticket.Author;
            this.Replies = oldticket.Replies;
            this.FilePath = oldticket.FilePath;
            this.Priority = oldticket.Priority;
            this.Type = oldticket.Type;
            this.assignedTo = oldticket.assignedTo;
            this.DateTime = oldticket.DateTime;
            this.Status = oldticket.Status;
            this.EditedOn = oldticket.EditedOn;
            this.Logs = oldticket.Logs;
        }
    }
}