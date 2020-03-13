using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChurchManager.Models
{
    public class ModalDelete
    {
        public string Action { get; set; }
        public string Controller { get; set; }

        public string ParentId { get; set; }
        public string Id { get; set; }

        public string Name { get; set; }

        public string ModalTitle { get; set; }

        public string ModalMessage { get; set; }

        public bool IsSubmit { get; set; }

        public bool DisableSubmit { get; set; }
    }
}