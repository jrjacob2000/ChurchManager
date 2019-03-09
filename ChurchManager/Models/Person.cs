using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ChurchManager.Models
{
    public class Person
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        [Required]
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string Suffix { get; set; }
        [Required]
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        [Required]
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        [Required]
        public string Country { get; set; }
        public string HomePhone { get; set; }
        public string WorkPhone { get; set; }
        public string CellPhone { get; set; }

        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Invalid Email")]
        public string Email { get; set; }

        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Invalid Email")]
        public string WorkEmail { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? BirthDay { get; set; }
        public DateTime? MembershipDate { get; set; }
        [Required]
        public string Gender { get; set; }
        public int Envelope { get; set; }
        public string Classification { get; set; }       
        public DateTime? DateLastEdited { get; set; }
        public DateTime DateEntered { get; set; }
        public Guid EnteredBy { get; set; }
        public Guid? EditedBy { get; set; }

        public Guid OwnerGroupId { get; set; }

        #region ForeignKey navigation
        //********************************
        //*ForeignKey navigation
        //********************************
        public virtual Family Family { get;set; }
        public virtual ReferenceList FamilyRole { get; set; }
        [ForeignKey("PersonId")]
        public virtual ICollection<Group> Groups { get; set; }
        #endregion
    }

}