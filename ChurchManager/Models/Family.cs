using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace ChurchManager.Models
{
    public class Family
    {
        public Guid ID { get; set; }
        [Required]
        public string Name { get; set; }
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
        [EmailAddress(ErrorMessage ="Invalid Email")]
        public string Email { get; set; }
        public DateTime? WeddingDate { get; set; }
        public DateTime DateEntered { get; set; }
        public DateTime? DateLastEdited { get; set; }
        public Guid EnteredBy { get; set; }
        public Guid? EditedBy { get; set; }
        //scanCheck
        //scanCredit
        public bool SendNewsLetter { get; set; }
        public bool DateDeactivated { get; set; }
        //OkToCanvass
        //Canvasser
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public int Envelope { get; set; }

        public Guid OwnerGroupId { get; set; }

    }

   
}