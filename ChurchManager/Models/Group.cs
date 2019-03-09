using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ChurchManager.Models
{
    public class Group
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime DateEntered { get; set; }
        public Guid EnteredBy { get; set; }         
        public DateTime? DateLastEdited { get; set; }
        public Guid? EditedBy { get; set; }
        public Guid OwnerGroupId { get; set; }

        [ForeignKey("GroupId")]
        public virtual ICollection<Person> Persons { get; set; }

    }
}