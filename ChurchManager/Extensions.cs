using ChurchManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ChurchManager
{
    public static class Extensions
    {
        public static IEnumerable<SelectListItem> GetAccountRegister(this IEnumerable<AccountChart> accts, string selectedValue = null)
        {
            var result = accts.Where(x => x.ShowInRegister  )
                 .Select(s => new SelectListItem() { Value = s.Id.ToString(), Text = string.Format("{0} - {1}",s.Type.ToString().Substring(0,3).ToUpper(), s.Name), Selected= s.Id.ToString() == selectedValue })
                 .OrderBy(o => o.Text)
                 .ToList();
            return result;
        }

        public static IEnumerable<SelectListItem> GetAccounts(this IEnumerable<AccountChart> accts, string selectedValue = null)
        {
            var result = accts.Where(x => x.Type != AccountChartTypeEnum.FundBalance)
                 .Select(s => new SelectListItem() { Value = s.Id.ToString(), Text = string.Format("{0} - {1}", s.Type.ToString().Substring(0, 3).ToUpper(), s.Name), Selected = s.Id.ToString() == selectedValue })
                 .OrderBy(o => o.Text)
                 .ToList();
            return result;
        }

        public static IEnumerable<SelectListItem> GetFunds(this IEnumerable<AccountChart> accts, string selectedValue = null)
        {
            var result = accts.Where(x => x.Type == AccountChartTypeEnum.FundBalance)
                 .Select(s => new SelectListItem() { Value = s.Id.ToString(), Text = s.Name, Selected = s.Id.ToString() == selectedValue } )
                 .OrderBy(o => o.Text)
                 .ToList();
            return result;
        }
    }
}