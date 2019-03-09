using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ChurchManager.Models
{
    public class Church
    {
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Address { get; set; }  
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }

        public string ZipCode { get; set; }
        public string Phone { get; set; }

        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Invalid Email")]
        public string Email { get; set; }
        public string FbPage { get; set; }

        public Guid OwnerGroupId { get; set; }
        
        public DateTime DateEntered { get; set; }
        public DateTime? DateLastEdited { get; set; }
        public Guid EnteredBy { get; set; }
        public Guid? EditedBy { get; set; }
    }
}