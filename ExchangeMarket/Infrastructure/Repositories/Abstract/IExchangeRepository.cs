using ExchangeMarket.Model;
using System.Threading.Tasks;

namespace ExchangeMarket.Infrastructure.Repositories.Abstract
{
    public interface IExchangeRepository
    {
        Task<decimal> GetCurrentSpentAmountByUser(string userId, string currency);
        Task InsertTransaction(string UserId, string Currency, decimal Amount);
        decimal GetExchangeLimit(SupportedCurrencies parsedCurrency);
        Task<decimal> GetExchangeRateAsync(SupportedCurrencies parsedCurrency);
    }
}
