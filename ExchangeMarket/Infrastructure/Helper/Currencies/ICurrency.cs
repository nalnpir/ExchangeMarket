using System.Threading.Tasks;

namespace ExchangeMarket.Infrastructure.Helper.Currencies
{
    /// <summary>
    /// Interface in order to use strategy with different exchange rates providers
    /// NOTE: The exchange rates are relative to Argentinian Pesos
    /// </summary>
    public interface ICurrency
    {
        public Task<decimal> GetExchangeRateAsync();
        public decimal CurrencyLimit();
    }
}
