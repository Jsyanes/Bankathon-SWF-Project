using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Coticoin.Core.Web
{
    public class BinanceApiClient
    {
        private const string BaseUrl = "https://testnet.binance.vision/api/";
        private const string ApiKey = "KtxyShUQ8J0Np20NV9SNbTX1qJ8REeLd6qFmT67dS2uwpwplY35uCRdXh2tGAUrA";
        private const string ApiSecret = "fhVKl7022E8fyaIB2h5N7JDHqg1HscbDCVtE0kzbcYA84Vovc9DgQVejFn0MtvSc";

        private readonly HttpClient _httpClient;

        public BinanceApiClient()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(BaseUrl)
            };
        }

        public async Task PlaceTestOrder(string symbol, decimal quantity)
        {
            var timestamp = GetTimestamp();
            var endpoint = "/api/v3/order/test";
            var parameters = $"symbol={symbol}&side=BUY&type=MARKET&quantity={quantity}&timestamp={timestamp}";
            var signature = GenerateSignature(parameters, ApiSecret);

            var requestUri = $"{endpoint}?{parameters}&signature={signature}";
            var request = new HttpRequestMessage(HttpMethod.Post, requestUri);
            request.Headers.Add("X-MBX-APIKEY", ApiKey);

            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Order successfully placed!");
            }
            else
            {
                Console.WriteLine("Failed to place the order: " + content);
            }
        }

        private static long GetTimestamp()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        private static string GenerateSignature(string parameters, string apiSecret)
        {
             var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(apiSecret));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(parameters));
            return BitConverter.ToString(hash).Replace("-", string.Empty).ToLower();
        }
    }
}