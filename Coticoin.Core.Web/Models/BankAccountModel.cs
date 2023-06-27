using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Coticoin.Core.Web.Models
{
    public class BankAccountModel
    {
        public string AccountId { get; set; }
        public decimal Balance { get; set; }
        public string transferId { get; set; }
        public string transactionid { get; set; }
        public string message { get; set; }

    }
}