using ChurchManager.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace ChurchManager.Controllers
{
    public class ReportsController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Reports
        public ActionResult Index(DateTime? dateFrom, DateTime? dateTo)
        {
            //var dateFrom = DateTime.Parse( "3/1/2020");
            //var dateTo = DateTime.Parse("3/30/2020");
            if (dateFrom == null || dateTo == null)
                return View();

            var grpId = Operator().OwnerGroupId;
            var church = db.Churches.Where(x => x.OwnerGroupId == grpId).FirstOrDefault();

            var reporTitle = new ReportTitle()
            {
                ChurchName = church.Name,
                Period = string.Format("Period: {0} - {1}", dateFrom.Value.ToShortDateString(), dateTo.Value.ToShortDateString())
            };

            var resut = new IncomeStatement()
            {
                Incomes = GetReportByType(AccountChartTypeEnum.Income, dateFrom.Value, dateTo.Value),
                Expenses = GetReportByType(AccountChartTypeEnum.Expenses, dateFrom.Value, dateTo.Value),
                NetAssetEndOfPeriod = GetNetAsset(null, dateTo.Value,NetAsset.End),
                NetAssetBeginningOfPeriod = GetNetAsset(null, dateFrom.Value, NetAsset.Begginning),
                ReportTitle = reporTitle
            };

            return View(resut);
        }


        public ActionResult FinancialPosition()
        {
            var grpId = Operator().OwnerGroupId;
            var church = db.Churches.Where(x => x.OwnerGroupId == grpId).FirstOrDefault();
            var reporTitle = new ReportTitle()
            {
                ChurchName = church.Name,
                Period = string.Format("As of: {0}", DateTime.Now.ToShortDateString())
            };

            var result = new FinancialPosition()
            {
                ReportTitle = reporTitle,
                Assets = GetFinancialPositionByType(AccountChartTypeEnum.Asset),
                LIabilities = GetFinancialPositionByType(AccountChartTypeEnum.Liability),
                NetAssets = GetNetAsset(null, null, NetAsset.Net),
            };

            return View(result);
        }

        private List<dynamic> GetFinancialPositionByType(AccountChartTypeEnum accountType)
        {
            var query = (from tl in db.TransactionLines
                         join a in db.AccountCharts on tl.AccountId equals a.Id
                         join f in db.AccountCharts on tl.FundId equals f.Id
                         where a.Type == accountType
                         group new { tl, a, f } by new { Fund = f.Id, Account = a.Id } into grp
                         select new
                         {
                             Fund = grp.FirstOrDefault().f.Name,
                             Account = grp.FirstOrDefault().a.Name,
                             Amount = grp.Sum(s => s.tl.Amount > 0 ? s.tl.Amount : s.tl.Amount * -1)

                         }).ToList();


            var pivotTable = query.ToPivotTable(
                item => item.Fund,
              item => item.Account,
              items => items.Any() ? items.Sum(x => x.Amount) : 0);


            return pivotTable.ToDynamicList();
        }

        private List<dynamic> GetReportByType(AccountChartTypeEnum accountType, DateTime dateFrom, DateTime dateTo)
        {
            var transview = (from t in db.Transactions
                             join tl in db.TransactionLines on t.Id  equals tl.TransactionId
                             join a in db.AccountCharts on tl.AccountId equals a.Id
                             join f in db.AccountCharts on tl.FundId equals f.Id
                             where a.Type == accountType && (t.TransactionDate >= dateFrom && t.TransactionDate <= dateTo) 
                             group new { tl, a, f } by new { Fund = f.Id, Account = a.Id } into grp
                             select new 
                             {
                                 Fund = grp.FirstOrDefault().f.Name,
                                 //Type = grp.FirstOrDefault().a.Type,
                                 Account = grp.FirstOrDefault().a.Name,
                                 Amount = grp.Sum(s => s.tl.Amount > 0 ? s.tl.Amount : s.tl.Amount * -1)

                             }).ToList();

            var pivotTable = transview.ToPivotTable(
                item => item.Fund,
              item => item.Account,              
              items => items.Any() ? items.Sum(x => x.Amount) : 0);


            return pivotTable.ToDynamicList();
 
        }

        private List<dynamic> GetNetAsset(DateTime? dateFrom, DateTime? dateTo, NetAsset type)
        {
            string accountName;
            switch(type)
            {
                case NetAsset.Begginning:
                    accountName = "NET ASSETS, BEGGINNING OF PERIOD";
                    break;
                case NetAsset.End:
                    accountName = "NET ASSETS, END OF PERIOD";
                    break;
                default:
                    accountName = "TOTAL NET ASSETS";
                    break; 
            }

            var transview = (from t in db.Transactions
                             join tl in db.TransactionLines on t.Id equals tl.TransactionId
                             join a in db.AccountCharts on tl.AccountId equals a.Id
                             join f in db.AccountCharts on tl.FundId equals f.Id
                             where (a.Type ==  AccountChartTypeEnum.Asset || a.Type == AccountChartTypeEnum.Liability) && 
                             ((dateFrom == null || t.TransactionDate >= dateFrom) && (dateTo == null || (type == NetAsset.Begginning ? t.TransactionDate < dateTo : t.TransactionDate <= dateTo)))
                             group new { tl, a, f } by new { Fund = f.Id} into grp
                             select new
                             {
                                 Fund = grp.FirstOrDefault().f.Name,
                                 Account = accountName,
                                 Amount = grp.Sum(s => s.tl.Amount)

                             }).ToList();

            var pivotTable = transview.ToPivotTable(
                item => item.Fund,
              item => item.Account,
              items => items.Any() ? items.Sum(x => x.Amount) : 0);


            return pivotTable.ToDynamicList();

        }

       
        private enum NetAsset
        {
            Begginning,
            End,
            Net
        }

    }

   

    public static class Helper
    {
        public static DataTable ToPivotTable<T, TColumn, TRow, TData>(this IEnumerable<T> source, Func<T, TColumn> columnSelector, Expression<Func<T, TRow>> rowSelector, Func<IEnumerable<T>, TData> dataSelector)
        {
            DataTable table = new DataTable();
            var rowName = ((MemberExpression)rowSelector.Body).Member.Name;
            table.Columns.Add(new DataColumn(rowName));
            var columns = source.Select(columnSelector).Distinct();

            foreach (var column in columns)
                table.Columns.Add(new DataColumn(column.ToString()));

            var rows = source.GroupBy(rowSelector.Compile())
                             .Select(rowGroup => new
                             {
                                 Key = rowGroup.Key,
                                 Values = columns.GroupJoin(
                                     rowGroup,
                                     c => c,
                                     r => columnSelector(r),
                                     (c, columnGroup) => dataSelector(columnGroup))
                             });

            foreach (var row in rows)
            {
                var dataRow = table.NewRow();
                var items = row.Values.Cast<object>().ToList();
                items.Insert(0, row.Key);
                dataRow.ItemArray = items.ToArray();
                table.Rows.Add(dataRow);
            }

            return table;
        }

        public static List<dynamic> ToDynamicList(this DataTable dt)
        {
            var list = new List<dynamic>();
            foreach (DataRow row in dt.Rows)
            {
                dynamic dyn = new ExpandoObject();
                list.Add(dyn);
                foreach (DataColumn column in dt.Columns)
                {
                    var dic = (IDictionary<string, object>)dyn;
                    dic[column.ColumnName] = row[column];
                }
            }
            return list;
        }
    }
}