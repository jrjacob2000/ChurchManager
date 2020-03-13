using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ChurchManager.Models
{
    public class TransactionView
    {
        public Guid Id { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        [Display(Name = "Transaction Date")]
        public DateTime? TransactionDate { get; set; }
        public string TransactionDateString { get; set; }
        public string Payee { get; set; }
        public string Comment { get; set; }
        [Display(Name ="Account Register")]
        [Required]
        public Guid AccountRegistryId { get; set; }
        [Display(Name = "Account")]
        public Guid AccountId { get; set; }
        [Display(Name = "Account")]
        public string AccountName { get; set; }
        [Display(Name ="Fund")]
        public Guid AccountFundId { get; set; }
        [Display(Name = "Fund")]
        public string FundName { get; set; }
        [RequiredIf("Deposit", null, ErrorMessage ="Either Deposit or Payment is required")]
        public Decimal? Payment { get; set; }
        [RequiredIf("Payment", null, ErrorMessage = "Either Deposit or Payment is required")]
        public Decimal? Deposit { get; set; }
        [Display(Name = "Updated Date")]
        public DateTime? DateLastEdited { get; set; }
        [Display(Name = "Created Date")]
        public DateTime DateEntered { get; set; }
        [Display(Name = "Created By")]
        public Guid EnteredBy { get; set; }
        [Display(Name = "Updated By")]
        public Guid? EditedBy { get; set; }
        public Guid OwnerGroupId { get; set; }
        [Display(Name = "Closed")]
        public bool IsClosed { get; set; }
        public bool Deleted { get; set; }
        public IEnumerable<AccountChart> AccountOptions { get; set; }
        
    }
}