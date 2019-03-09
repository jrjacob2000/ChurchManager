using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ChurchManager.Models
{
    public class Operation
    {

        [Required]
        public string Id { get; set; }

        [Required]
        public string Description { get; set; }

        [ForeignKey("OperationId")]
        public virtual ICollection<RoleOperation> ApplicationRoles { get; set; }

    }
}