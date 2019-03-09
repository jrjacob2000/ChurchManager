using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ChurchManager.Models
{
    public class GroupView
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage ="Please select person to add")]
        public string SectedPersonIdToAdd { get; set; }

        public List<SelectListItem> PersonList { get; set; }
        public List<GroupMemberRow> Members { get; set; }

        public GroupView()
        {
            Members = new List<GroupMemberRow>();
        }
    }

    public class GroupMemberRow
    {
        public Guid Id { get; set; }

        public string Name {
            get {
                return string.Format("{0}, {1}",this.LastName, this.FirstName);
            }
        }
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public bool IsDeleted { get; set; }
    }
}