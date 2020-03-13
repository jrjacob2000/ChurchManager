using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ChurchManager.Models
{
    public class TransactionLine
    {
        public Guid Id { get; set; } 
        public Guid TransactionId { get; set; }
        public Guid AccountId { get; set; }
        public Guid FundId { get; set; }  
        public Decimal Amount { get; set; }
    }
}