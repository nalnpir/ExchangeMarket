using ExchangeMarket.Model.Exceptions;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace ExchangeMarket.Infrastructure.Helper.Currencies
{
    /// <summary>
    /// Adaptee for Provincia Api, in order to demostrate how would I solve the new Third Party API
    /// </summary>
    public class UsdCurrency : ICurrency
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;
        private readonly decimal LIMIT = 200;

        public UsdCurrency(HttpClient httpClient, ILogger logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public decimal CurrencyLimit() => LIMIT;

        public async Task<decimal> GetExchangeRateAsync()
        {
            var uri = "https://www.bancoprovincia.com.ar/Principal/Dolar";
            var response = await _httpClient.GetAsync(uri);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Api failed to respond, error code: {(int)response.StatusCode}, reason: {response.ReasonPhrase}");
                throw new UnavailableApiException();
            }

            var result = await response.Content.ReadAsStringAsync();

            // Array positions: 0 - Buy Value, 1 - Sell Value, 2 - LastUpdated
            return decimal.Parse(JsonSerializer.Deserialize<string[]>(result)[1]);
        }
    }
}
