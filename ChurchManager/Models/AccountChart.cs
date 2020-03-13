using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ChurchManager.Models
{
    public class AccountChart
    {
        public Guid Id { get; set; }
        [Display(Name ="Account Number")]
        public int Code { get; set; }
        [Required]
        public string Name { get; set; }
        [Display(Name ="Active")]
        public DateTime DateEntered { get; set; }
        public Guid EnteredBy { get; set; }
        public DateTime? DateLastEdited { get; set; }
        public Guid? EditedBy { get; set; }
        public Guid OwnerGroupId { get; set; }
        public AccountChartTypeEnum Type { get; set; }
        [Display(Name = "Show in Register")]
        public bool ShowInRegister { get; set; }

        [ForeignKey("AccountId")]
        public virtual ICollection<TransactionLine> TransactionLines { get; set; }

        [ForeignKey("AccountRegisterId")]
        public virtual ICollection<Transaction> Transactions { get; set; }

    }
}