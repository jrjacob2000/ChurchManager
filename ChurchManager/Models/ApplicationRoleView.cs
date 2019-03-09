using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ChurchManager.Models
{
    public class ApplicationRoleView
    {
        public string Id { get; set; }
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }

        public bool IsDefaultRole { get; set; }

        public List<string> SelectedOperation { get; set; }
        public IEnumerable<System.Web.Mvc.SelectListItem> OperationList { get; set; }
    }

    public class ApplicationRoleListView : CanPerformOperation
    {
        public IEnumerable<ApplicationRoleView> Items { get; set; }
    }
}