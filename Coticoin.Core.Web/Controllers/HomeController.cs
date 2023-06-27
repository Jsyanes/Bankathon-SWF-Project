using Coticoin.Core.Web.Models;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Razor.Text;
using System.Web.Services.Description;
using static Coticoin.Core.Web.Controllers.HomeController;

namespace Coticoin.Core.Web.Controllers
{
    public class HomeController : Controller
    {
        public class Exchange
        {
            public string Name { get; set; }
            public decimal Balance { get; set; }
        }
        public async Task<ActionResult> Index()
        {
          
            var oClient= ClientAccount();
            return View(oClient);
        }
        public async Task<ActionResult> IndexProviders()
        {
            return PartialView();
        }
        //Codigo para simular un pago desde el exchange de towerbank
        public ExchangeAccountDataModel Excersice(string exChange, decimal Amount=20000)
        {
            var data = GetProviderTransaction();
            if (Amount == 0)
            {
                BalanceFunds(data);
            }

            var oInput = data.Data.Where(k => k.Name == exChange).FirstOrDefault();
            if (oInput.Balance >= Amount)
            {
                //Puede avanzar
            }
            else
            {
                var value = Amount - oInput.Balance;
                List<ExchangesCompany> input = data.Data.Where(m=> m.Name != exChange).OrderByDescending(k => k.Balance).ToList();
                foreach (var item in input)
                {
                  
                    decimal  averageBalance = Math.Round( (value + 20000) / input.Count);


                    ProviderTransaction(item.Name, exChange, averageBalance);
                    data = GetProviderTransaction();
                    oInput = data.Data.Where(k => k.Name == exChange).FirstOrDefault();
                    if (oInput.Balance >= Amount)
                    {
                        break;
                    }
                }
               
              
            }
            data = GetProviderTransaction();

            return data;
        }
        //Codigo para balancear los fondos entre enchange de towerbank
        public void BalanceFunds(ExchangeAccountDataModel exchanges)
        {
            decimal totalBalance = exchanges.Data.Sum(e => e.Balance);
            decimal averageBalance = totalBalance / exchanges.Data.Count;

            // Ordena los exchanges por saldo descendente
            List<ExchangesCompany> input = exchanges.Data.OrderByDescending(e => e.Balance).ToList();

            for (int i = 0; i < input.Count; i++)
            {
                decimal targetBalance = input[i].Balance- averageBalance;
                
                input[i].Balance = input[i].Balance - targetBalance;
                if (i== input.Count-1)
                {
                    break;
                }
                ProviderTransaction(input[i].Name, input[i + 1].Name, targetBalance);
                input[i + 1].Balance = input[i + 1].Balance + targetBalance;
            }
            var data = GetProviderTransaction();
        }
        //Metodo para traer la informacion del cliente
        public BankAccountModel ClientAccount()
        {
            var client = new RestClient("https://towerbank.bankathontb.com/bankathon/v1/account");

            var request = new RestRequest();
            request.AddHeader("Authorization", "Bearer 86e7b63e-a39a-4774-90eb-c1fdb364fed0!040e8fc1aa27f7685440e4d264edfb439a3962c540d05c10fb40204576415120dede8df37305fb");
            request.AddHeader("Cookie", "ASP.NET_SessionId=020mvucyjigfj0j1qsl54v0e");
            RestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);

            BankAccountModel account = JsonConvert.DeserializeObject<BankAccountModel>(response.Content);
            return account;

        }
        //Metodo para ejecutar una orden de compra o venta  de un cliente 
        public BankAccountModel AccountTransaction(string AccountId,decimal Amount, string TransactionType)
        {
            var client = new RestClient("https://towerbank.bankathontb.com/bankathon/v1/transaction");
           
            var request = new RestRequest("",Method.Post);
            request.AddHeader("Authorization", "Bearer 86e7b63e-a39a-4774-90eb-c1fdb364fed0!040e8fc1aa27f7685440e4d264edfb439a3962c540d05c10fb40204576415120dede8df37305fb");
            request.AddHeader("Content-Type", "text/plain");
            request.AddHeader("Cookie", "ASP.NET_SessionId=020mvucyjigfj0j1qsl54v0e");
            request.AddParameter("text/plain", "{\r\n\t\"accountId\": \""+ AccountId + "\",\r\n\t\"amount\": "+ Amount + ",\r\n\t\"transactionType\": \""+ TransactionType + "\"\r\n}'", ParameterType.RequestBody);
            RestResponse response = client.Execute(request);
            BankAccountModel account = JsonConvert.DeserializeObject<BankAccountModel>(response.Content);

            return account;

        }
        //Metodo para traer los balances de los echanges de towerbank
        public ExchangeAccountDataModel GetProviderTransaction()
        {
            var client = new RestClient("https://towerbank.bankathontb.com/bankathon/v1/providers");
          
            var request = new RestRequest();
            request.AddHeader("Authorization", "Bearer 86e7b63e-a39a-4774-90eb-c1fdb364fed0!040e8fc1aa27f7685440e4d264edfb439a3962c540d05c10fb40204576415120dede8df37305fb");
            request.AddHeader("Cookie", "ASP.NET_SessionId=020mvucyjigfj0j1qsl54v0e");
            RestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);

            ExchangeAccountDataModel accountData = JsonConvert.DeserializeObject<ExchangeAccountDataModel>(response.Content);
            return accountData;

        }
        //Metodo para transferir balances entre exchange de towerbank
        public BankAccountModel ProviderTransaction(string from,string to, decimal amount)
        {
            var client = new RestClient("https://towerbank.bankathontb.com/bankathon/v1/providerstransaction");
           
            var request = new RestRequest("", Method.Post);
            request.AddHeader("Authorization", "Bearer 86e7b63e-a39a-4774-90eb-c1fdb364fed0!040e8fc1aa27f7685440e4d264edfb439a3962c540d05c10fb40204576415120dede8df37305fb");
            request.AddHeader("Content-Type", "text/plain");
          
            request.AddParameter("text/plain", "{\r\n\t\"from\": \""+ from + "\",\r\n\t\"to\": \""+ to + "\",\r\n\t\"amount\": "+ amount + "\r\n}", ParameterType.RequestBody);
            RestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);

            BankAccountModel account = JsonConvert.DeserializeObject<BankAccountModel>(response.Content);
            return account;

        }
        //Action result donde busco los precios de los exchange Binance y kraken para luego comparar precios
        public ActionResult Exchanges(decimal Price = 0)
        {

            string binanceApiKey = "MRmI8njksPucLlehOIFomY1BfPkk2PZFxUaCyUuOMMW2Hckc9RH373nJp3X0c1c0";
            string krakenApiKey = "UUoMzRzCQZkQndgwC9YcQQncIWccs2Dw0TRBhoRj+VfFwY64MVANN4m1";

            ExchangeApiClient binanceApiClient = new ExchangeApiClient(binanceApiKey);
            ExchangeApiClient krakenApiClient = new ExchangeApiClient(krakenApiKey);
            List<ExChangesModel> input = new List<ExChangesModel>();
            try
            {
                decimal precioBinanceBTC = binanceApiClient.ObtenerPrecioBitcoin("binance");
                decimal precioKrakenBTC = krakenApiClient.ObtenerPrecioBitcoin("kraken");
                decimal precioBinanceETH = binanceApiClient.ObtenerPrecioEthereum("binance");
                decimal precioKrakenETH = krakenApiClient.ObtenerPrecioEthereum("kraken");

                var priceRealBinanceBTC = (Price * 1) / precioBinanceBTC;
                var priceRealKrakenBTC = (Price * 1) / precioKrakenBTC;
                var priceRealBinanceETH = (Price * 1) / precioBinanceETH;
                var priceRealKrakenETH = (Price * 1) / precioKrakenETH;
                ExChangesModel a = new ExChangesModel()
                {
                    NameExchange = "Binance",
                    PriceBTC = precioBinanceBTC.ToString("N2"),
                    PriceRealBTC = priceRealBinanceBTC,
                    PriceETH = precioBinanceETH.ToString("N2"),
                    PriceRealETH = priceRealBinanceETH,
                    Logo = "https://flexifilesatest.blob.core.windows.net/videos/Binance-logo.png",
                    order = precioBinanceBTC < precioKrakenBTC ? 1 : 2

                };
                ExChangesModel b = new ExChangesModel()
                {
                    NameExchange = "Kraken",
                    PriceBTC = precioKrakenBTC.ToString("N2"),
                    PriceRealBTC = priceRealKrakenBTC,
                    PriceETH = precioKrakenETH.ToString("N2"),
                    PriceRealETH = priceRealKrakenETH,
                    Logo = "https://flexifilesatest.blob.core.windows.net/videos/Kraken-Logo.png",
                    order = precioKrakenBTC < precioBinanceBTC ? 1 : 2
                };
                input.Add(a);
                input.Add(b);
                input.OrderBy(k => k.order).ToList();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener los precios: {ex.Message}");
            }
            return PartialView(input);
        }
        //ActionResult para mostrar los balances de los exchanges luego de ejecutar una compra o venta
        public ActionResult ProvidersBalance(string exChange,decimal Monto = 0)
        {

           var input = Excersice(exChange, Monto);
            foreach (var student in input.Data)
            {
                if (student.Name == exChange)
                {
                    student.Balance = student.Balance -Monto;
                }
            }
            return PartialView(input);
        }
        //Metodo para comprar o vender Binance
        public async Task<ActionResult> BuysellBinance(decimal PriceBinance, string Type)
        {

            var oBalanceClient = ClientAccount();
            var oClient = AccountTransaction("2d183848-0861-4035-b566-0387b4af4c92", PriceBinance, Type);
            if (!string.IsNullOrEmpty(oClient.message) || PriceBinance > oBalanceClient.Balance)
            {
                return Json(new { Success = false, Message = "Fondos insuficientes" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Success = true, Balance = oClient.Balance, transferId = oClient.transferId }, JsonRequestBehavior.AllowGet);

        }
        //Metodo para comprar o vender Kraken
        public async Task<ActionResult> Buysellkraken(decimal Pricekraken, string Type)
        {

            var oBalanceClient = ClientAccount();
            var oClient = AccountTransaction("2d183848-0861-4035-b566-0387b4af4c92", Pricekraken, Type);
            if (!string.IsNullOrEmpty(oClient.message) || Pricekraken > oBalanceClient.Balance)
            {
                return Json(new { Success = false, Message = "Fondos insuficientes" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { Success = true, Balance = oClient.Balance, transferId = oClient.transferId }, JsonRequestBehavior.AllowGet);

        }

    }
}