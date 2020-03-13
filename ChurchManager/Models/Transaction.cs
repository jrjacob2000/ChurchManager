using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChurchManager.Models
{
    public class Transaction
    {
        public Guid Id { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Payee { get; set; }
        public string Comment { get; set; }
        public Guid AccountRegisterId { get; set; }
        public DateTime? DateLastEdited { get; set; }
        public DateTime DateEntered { get; set; }
        public Guid EnteredBy { get; set; }
        public Guid? EditedBy { get; set; }
        public Guid OwnerGroupId { get; set; }
        [Display(Name ="Closed")]
        public bool IsClosed { get; set; }
        public bool Deleted { get; set; }


        [ForeignKey("TransactionId")]
        public virtual ICollection<TransactionLine> TransactionLines { get; set; }

    }
}