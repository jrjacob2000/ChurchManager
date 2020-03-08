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
        public bool IsActive { get; set; }
        public DateTime DateEntered { get; set; }
        public Guid EnteredBy { get; set; }
        public DateTime? DateLastEdited { get; set; }
        public Guid? EditedBy { get; set; }
        public Guid OwnerGroupId { get; set; }
        public AccountChartType Type { get; set; }

    }
}