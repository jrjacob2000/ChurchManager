using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ChurchManager.Models
{
    public class TransactionDetailView
    {
        public Guid Id { get; set; }
        [Display(Name ="Transaction Date")]
        public DateTime TransactionDate { get; set; }
        public string Payee { get; set; }
        public string Comment { get; set; }
        [Display(Name = "Account Registry")]
        public string AccountRegistryName { get; set; }
        [Display(Name = "Account")]
        public string AccountName { get; set; }
        [Display(Name = "Fund")]
        public string FundName { get; set; }
        public decimal? Payment { get; set; }
        public decimal? Deposit { get; set; }
        [Display(Name = "Updated By")]
        public string EditedBy { get; set; }
        [Display(Name = "Updated Date")]
        public DateTime? DateLastEdited { get; set; }
        [Display(Name = "Created By")]
        public string EnteredBy { get; set; }
        [Display(Name = "Created Date")]
        public DateTime DateEntered { get; set; }
    }
}