using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChurchManager.Models
{
    public class Attendance
    {
        public Guid Id { get; set; }
        public DateTime Date{get;set;}
        public int Value { get; set; }
        public virtual Group Group { get; set; }
        public DateTime DateEntered { get; set; }
        public DateTime? DateLastEdited { get; set; }
        public Guid EnteredBy { get; set; }
        public Guid? EditedBy { get; set; }

        public Guid OwnerGroupId { get; set; }
    }
}