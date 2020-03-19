using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChurchManager.Models
{
    public class TransactionListView
    {
        public Guid Id { get; set; }
        public string TransactionDate { get; set; }
        public string Payee { get; set; }
        public string Comment { get; set; }
        public Guid AccountRegistryId { get; set; }
        public string AccountName { get; set; }
        public string FundName { get; set; }
        public decimal? Payment { get; set; }
        public decimal? Deposit { get; set; }
    }
}