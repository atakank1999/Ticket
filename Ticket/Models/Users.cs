using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Configuration;

namespace Ticket.Models
{
    [Table("Users")]
    public class Users
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [DisplayName("Ad"),
        Required(ErrorMessage = "{0} boş geçilemez")]
        public string Name { get; set; }

        [DisplayName("Soyad"),
         Required(ErrorMessage = "{0} boş geçilemez")]
        public string Surname { get; set; }

        [DisplayName("E-posta"), EmailAddress]
        public string Email { get; set; }

        [DisplayName("Kullanıcı adı"),
         Required(ErrorMessage = "Lütfen bir {0} giriniz."),
         MinLength(5, ErrorMessage = "{0} min. {1} karakter olmalıdır."),
         MaxLength(25, ErrorMessage = "{0} max. {1} karakter olmalıdır.")]
        public string Username { get; set; }

        [DisplayName("Şifre"),
         Required(ErrorMessage = "Lütfen bir {0} giriniz."),
         DataType(DataType.Password)]
        public string Password { get; set; }

        public Guid ConfirmGuid { get; set; }

        public bool IsConfirmed { get; set; }

        public bool IsAdmin { get; set; }

        // validation
        public virtual List<Assignment> Assignments { get; set; }

        public virtual List<Ticket> Tickets { get; set; }
        public virtual List<Reply> Replies { get; set; }
        public virtual List<Log> Logs { get; set; }
        public bool IsDeleted { get; set; }

        public Users()
        {
            ConfirmGuid = Guid.NewGuid();
            this.IsDeleted = false;
        }

        public Users(Users u)
        {
            if (u != null)
            {
                this.Name = u.Name;
                this.Surname = u.Surname;
                this.Username = u.Username;
                this.Email = u.Email;
                this.Password = u.Password;
                this.ConfirmGuid = u.ConfirmGuid;
                this.IsConfirmed = u.IsConfirmed;
                this.IsAdmin = u.IsAdmin;
                if (u.Tickets != null)
                {
                    foreach (Ticket ticket in u.Tickets)
                    {
                        Ticket t = new Ticket(ticket);
                        t.Author = this;
                        if (this.Tickets == null)
                        {
                            this.Tickets = new List<Ticket>();
                        }
                        this.Tickets.Add(t);
                    }
                }

                if (u.Assignments != null)
                {
                    foreach (Assignment assignment in u.Assignments)
                    {
                        Assignment a = new Assignment(assignment);
                        if (this.Assignments == null)
                        {
                            this.Assignments = new List<Assignment>();
                        }
                        a.Admin = this;
                        this.Assignments.Add(a);
                    }
                }

                if (u.Replies != null)
                {
                    foreach (Reply reply in u.Replies)
                    {
                        Reply r = new Reply(reply);
                        if (this.Replies == null)
                        {
                            this.Replies = new List<Reply>();
                        }
                        r.WriterAdmin = this;
                        this.Replies.Add(r);
                    }
                }

                this.IsDeleted = u.IsDeleted;
            }
        }
    }
}