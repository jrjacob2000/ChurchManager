using ChurchManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ChurchManager.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        DateTime _yearPrevious = DateTime.Now.AddMonths(-12);

        private ApplicationDbContext db = new ApplicationDbContext();
     
        public ActionResult Index()
        {
            

            var people = db.People.Count();
            var family = db.Families.Count();
            var yearToDateExpneses = (from t in db.Transactions
                                      join tl in db.TransactionLines on t.Id equals tl.TransactionId
                                      join a in db.AccountCharts on tl.AccountId equals a.Id
                                      where a.Type == AccountChartTypeEnum.Expenses && t.TransactionDate >= _yearPrevious
                                      select (decimal?) tl.Amount).Sum().GetValueOrDefault();

            var yearToDateRevenue = (from t in db.Transactions
                                      join tl in db.TransactionLines on t.Id equals tl.TransactionId
                                      join a in db.AccountCharts on tl.AccountId equals a.Id
                                      where a.Type == AccountChartTypeEnum.Income && t.TransactionDate >= _yearPrevious
                                     select (decimal?)tl.Amount).Sum().GetValueOrDefault();

           

            var result = new DashboardView()
            {
                People = people,
                Family = family,
                YearToDateExpenses = yearToDateExpneses,
                YearToDateRevenue = yearToDateRevenue * -1
            };
            return View(result);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public JsonResult GetLineChart()
        {
            var expenseByMonthquery = (from t in db.Transactions
                                       join tl in db.TransactionLines on t.Id equals tl.TransactionId
                                       join a in db.AccountCharts on tl.AccountId equals a.Id
                                       where a.Type == AccountChartTypeEnum.Expenses && t.TransactionDate >= _yearPrevious
                                       select new { TransactionDate = t.TransactionDate, Amount = tl.Amount })
                                       .GroupBy(x => x.TransactionDate.Month)
                                       .Select(grp => new { Month = grp.Key, TransactionDate = grp.FirstOrDefault(), Amount = grp.Sum(i => i.Amount) });

            var revenueByMonthquery = (from t in db.Transactions
                                       join tl in db.TransactionLines on t.Id equals tl.TransactionId
                                       join a in db.AccountCharts on tl.AccountId equals a.Id
                                       where a.Type == AccountChartTypeEnum.Income && t.TransactionDate >= _yearPrevious
                                       select new { TransactionDate = t.TransactionDate, Amount = tl.Amount }
                                       )
                                       .GroupBy(x => x.TransactionDate.Month)
                                       .Select(grp => new { Month = grp.Key, TransactionDate=grp.FirstOrDefault().TransactionDate, Amount = grp.Sum(i => i.Amount) });


            var transactionByByMonthQuery = (from t in db.Transactions
                                             group t by t.TransactionDate.Month into grp
                                             select new { Month = grp.Key, TransactionDate = grp.FirstOrDefault().TransactionDate });

            var result = (from t in transactionByByMonthQuery
                          join ex in expenseByMonthquery on t.Month equals ex.Month into ex_join
                          join re in revenueByMonthquery on t.Month equals re.Month into re_join
                          from tex in ex_join.DefaultIfEmpty()
                          from tre in re_join.DefaultIfEmpty()
                          select new { TransactionDate = t.TransactionDate, Revenue = (tre != null? tre.Amount : 0 ), Expenses = (tex != null ? tex.Amount:0) })
                          .AsEnumerable()
                          .Select(s => new { month = s.TransactionDate.ToString("yyyy-MM"), revenue = s.Revenue * -1, expenses = s.Expenses });
                      

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetKnobChart()
        {
            var query = (from tl in db.TransactionLines
                         join f in db.AccountCharts on tl.FundId equals f.Id
                         join a in db.AccountCharts on tl.AccountId equals a.Id
                         where a.Type == AccountChartTypeEnum.Asset
                         select new { Fund = f.Name, Amount = tl.Amount })
                        .GroupBy(x => x.Fund)
                        .Select(s => new { label = s.Key, value = s.Sum(i => i.Amount) });

            return Json(query, JsonRequestBehavior.AllowGet);
        }
    }
}