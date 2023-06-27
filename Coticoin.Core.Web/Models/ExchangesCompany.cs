using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Coticoin.Core.Web.Models
{
   
    public class ExchangesCompany
    {
        public string Name { get; set; }
        public decimal Balance { get; set; }
    }

    public class ExchangeAccountDataModel
    {
        public List<ExchangesCompany> Data { get; set; }
    }
}