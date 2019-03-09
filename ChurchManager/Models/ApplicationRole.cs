using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ChurchManager.Models
{
    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole() { }
        [ForeignKey("RoleId")]
        public virtual ICollection<RoleOperation> Operations { get; set; }

        public string Description { get; set; }

        public Guid OwnerGroupId { get; set; }

        [Required]
        public bool IsDefaultRole { get; set; }
    }

    
}