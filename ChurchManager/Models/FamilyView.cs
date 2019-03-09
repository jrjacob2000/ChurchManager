using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ChurchManager.Models
{
    public class FamilyView
    {
        public Guid ID { get; set; }

        [Required]
        [Display(Name = "Family Name")]
        public string Name { get; set; }

        public string AddressDisplay
        {
            get
            {
                var address2 = (!string.IsNullOrEmpty(Address2) ? string.Format(" {0}", Address2) : "");
                var state = !string.IsNullOrEmpty(State) ? string.Format(", {0}", State) : "";
                return string.Format("{0}{1}{2}, {3}", Address1, address2, state, Country);
            }
        }
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

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? WeddingDate { get; set; }
        //scanCheck
        //scanCredit
        public bool SendNewsLetter { get; set; }
        //OkToCanvass
        //Canvasser
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public int Envelope { get; set; }
        public string SectedPersonIdToAdd { get; set; }
        public List<SelectListItem> PersonList { get; set; }
        public IEnumerable<PersonView> FamilyMembers { get; set; }


    }

    public class FamilyListView : CanPerformOperation
    {
        public IEnumerable<FamilyView> Items { get; set; }
    }
}