using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ChurchManager.Models
{
    public class RoleOperation
    {

        [Key, Column(Order = 1)]
        public string RoleId { get; set; }

        [Key, Column(Order = 2)]
        public string OperationId { get; set; }

        public Guid OwnerGroupId { get; set; }
    }
}