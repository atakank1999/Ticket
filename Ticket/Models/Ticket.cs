using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
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
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required(ErrorMessage = "Boş Geçilemez"),Display(Name = "Başlık")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Boş Geçilemez"), Display(Name = "Biletinizin içeriği")]

        public string Text { get; set; }
        public virtual Users Author { get; set; }
        public virtual List<Reply> Replies { get; set; }
        public string FilePath { get; set; }
        public Priority Priority { get; set; }
        public Type Type { get; set; }
        public virtual Assignment assignedTo { get; set; }
        public DateTime DateTime { get; set; }
        public Status Status { get; set; }

        public Ticket()
        {
            DateTime = DateTime.UtcNow;
        }
    }
}