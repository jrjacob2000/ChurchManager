using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChurchManager.Models
{
    public class DashboardView
    {
        public int People { get; set; }
        public int Family { get; set; }
        public decimal YearToDateExpenses { get; set; }
        public decimal YearToDateRevenue { get; set; }
        public object RevenueVsExpneseLineChart { get; set; }

    }
}