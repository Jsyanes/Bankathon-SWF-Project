using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Web;

namespace Coticoin.Core.Web
{
    public class BinanceTickerResponse
    {
        public string symbol { get; set; }
        public string price { get; set; }
    }
    
   
    public class KrakenTickerResponse
    {
        public KrakenTickerResult result { get; set; }

        public class KrakenTickerResult
        {
            public KrakenTickerPair XXBTZUSD { get; set; }
            public KrakenTickerPair XETHZUSD { get; set; }

            public class KrakenTickerPair
            {
                public string[] c { get; set; }
            }
        }
    }

    public class ExchangeApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public ExchangeApiClient(string apiKey)
        {
            _apiKey = apiKey;
            _httpClient = new HttpClient();
        }

        public decimal ObtenerPrecioBitcoin(string exchange)
        {
            string url = string.Empty;

            switch (exchange.ToLower())
            {
                case "binance":
                    url = "https://api.binance.us/api/v3/ticker/price?symbol=BTCUSDT";
                    break;
                case "kraken":
                    url = "https://api.kraken.com/0/public/Ticker?pair=XBTUSD";
                    break;
               
                    break;
                default:
                    throw new ArgumentException("Exchange no válido.");
            }

            try
            {
                var uri = String.Format(url, 500);

                WebClient client = new WebClient();
                client.UseDefaultCredentials = true;
                var data = client.DownloadString(uri);


                switch (exchange.ToLower())
                {
                    case "binance":
                        var binanceData = JsonConvert.DeserializeObject<BinanceTickerResponse>(data);
                        return decimal.Parse(binanceData.price);
                    case "kraken":
                        var krakenData = JsonConvert.DeserializeObject<KrakenTickerResponse>(data);
                        return decimal.Parse(krakenData.result.XXBTZUSD.c[0]);
                   
                    default:
                        throw new ArgumentException("Exchange no válido.");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public decimal ObtenerPrecioEthereum(string exchange)
        {
            string url = string.Empty;

            switch (exchange.ToLower())
            {
                case "binance":
                    url = "https://api.binance.us/api/v3/ticker/price?symbol=ETHUSDT";
                    break;
                case "kraken":
                    url = "https://api.kraken.com/0/public/Ticker?pair=ETHUSD";
                    break;
               
                    break;
                default:
                    throw new ArgumentException("Exchange no válido.");
            }

            try
            {
                var uri = String.Format(url, 500);

                WebClient client = new WebClient();
                client.UseDefaultCredentials = true;
                var data = client.DownloadString(uri);


                switch (exchange.ToLower())
                {
                    case "binance":
                        var binanceData = JsonConvert.DeserializeObject<BinanceTickerResponse>(data);
                        return decimal.Parse(binanceData.price);
                    case "kraken":
                        var krakenData = JsonConvert.DeserializeObject<KrakenTickerResponse>(data);
                        return decimal.Parse(krakenData.result.XETHZUSD.c[0]);
                    default:
                        throw new ArgumentException("Exchange no válido.");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


    }
}