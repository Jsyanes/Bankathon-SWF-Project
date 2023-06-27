using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Coticoin.Core.Web.Models
{
    public class ExChangesModel
    {
        public string NameExchange { get; set; }
        public string PriceBTC { get; set; }
        public decimal PriceRealBTC { get; set; }
        public string PriceETH { get; set; }
        public decimal PriceRealETH { get; set; }
        public string Logo { get; set; }
        public int order { get; set; }
    }
}