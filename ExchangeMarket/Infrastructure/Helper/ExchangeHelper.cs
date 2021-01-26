using ExchangeMarket.Infrastructure.Helper.Currencies;
using System;
using System.Threading.Tasks;

namespace ExchangeMarket.Infrastructure.Helper
{
    /// <summary>
    /// Strategy class
    /// </summary>
    public class ExchangeHelper
    {
        private ICurrency _currency;
        public ExchangeHelper(ICurrency currency) => _currency = currency ?? throw new ArgumentNullException(nameof(currency));
        public decimal GetExchangeLimit() => _currency.CurrencyLimit();
        public async Task<decimal> GetExchangeRateAsync() => await _currency.GetExchangeRateAsync();
    }
}
