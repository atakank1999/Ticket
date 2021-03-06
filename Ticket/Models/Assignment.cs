﻿using System;
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

        [Required(ErrorMessage = "Lütfen Bir Kişi Seçiniz.")]
        public virtual Users Admin { get; set; }

        public bool IsDone { get; set; }
        public bool IsDeleted { get; set; }

        [DataType(DataType.Date)]
        public DateTime Deadline { get; set; }

        public virtual List<Log> Logs { get; set; }

        public Assignment()
        {
            Deadline = default(DateTime);
            this.IsDeleted = false;
        }

        public Assignment(Assignment a)
        {
            if (a != null)
            {
                this.Admin = a.Admin;
                this.Ticket = a.Ticket;
                this.Deadline = a.Deadline;
                this.IsDone = a.IsDone;
                this.IsDeleted = a.IsDeleted;
            }
        }
    }
}