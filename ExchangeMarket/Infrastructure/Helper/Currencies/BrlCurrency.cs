using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ExchangeMarket.Infrastructure.Helper.Currencies
{
    /// <summary>
    /// This is a mock class since the final implementation will be dependant on the third party being developed
    /// </summary>
    public class BrlCurrency : ICurrency
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;
        private readonly decimal LIMIT = 300;

        public BrlCurrency(HttpClient httpClient, ILogger logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public decimal CurrencyLimit() => LIMIT;

        public async Task<decimal> GetExchangeRateAsync()
        {
            var usdCurrency = new UsdCurrency(_httpClient, _logger);

            return await usdCurrency.GetExchangeRateAsync() / 4m;
        }
    }
}
