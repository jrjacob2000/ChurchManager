using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChurchManager.Models
{
    public class FinancialPosition
    {
        public ReportTitle ReportTitle { get; set; }
        public List<dynamic> Assets { get; set; }
        public List<dynamic> LIabilities { get; set; }

        public List<dynamic> NetAssets { get; set; }

    }
}