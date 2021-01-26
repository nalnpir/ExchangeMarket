using ExchangeMarket.Infrastructure.Repositories.Abstract;
using ExchangeMarket.Model;
using ExchangeMarket.Model.Exceptions;
using ExchangeMarket.Services.Abstract;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ExchangeMarket.Services
{
    public class ExchangeService : IExchangeService
    {
        private readonly ILogger<ExchangeService> _logger;
        private readonly IExchangeRepository _repo;
        public ExchangeService(ILogger<ExchangeService> logger, IExchangeRepository exchangeRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _repo = exchangeRepository ?? throw new ArgumentNullException(nameof(exchangeRepository));
        }

        /// <summary>
        /// Tries to buy currency if possible, making a transaction with the Model provided
        /// </summary>
        /// <param name="model">Provides the UserId, Amount and Currency input by the user</param>
        /// <returns></returns>
        public async Task BuyCurrency(BuyCurrencyOrder model)
        {
            var parsedCurrency = GetValidCurrencyFromISO(model.Currency);

            var limit = _repo.GetExchangeLimit(parsedCurrency);
            var currentRate = await _repo.GetExchangeRateAsync(parsedCurrency);
            var currentMontlyPurchase = await _repo.GetCurrentSpentAmountByUser(model.UserId, model.Currency.ToUpper());

            if (currentMontlyPurchase < limit && (currentMontlyPurchase + model.Amount / currentRate) <= limit)
            {
                await _repo.InsertTransaction(model.UserId, model.Currency.ToUpper(), model.Amount / currentRate);
            }
            else
            {
                throw new MonthlyLimitException(model.Currency.ToUpper());
            }
        }

        /// <summary>
        /// Get the current exchange rate from a particular supported ISO Code
        /// </summary>
        /// <param name="ISOCode">ISO Currency code</param>
        /// <returns>Exchange rate expressed as decimal</returns>
        public async Task<decimal> GetExchangeRateByISOCodeAsync(string ISOCode)
        {
            var parsedCurrency = GetValidCurrencyFromISO(ISOCode);

            return await _repo.GetExchangeRateAsync(parsedCurrency);
        }


        /// <summary>
        /// Get Normalized currency from string
        /// </summary>
        /// <param name="ISOCode">input iso code</param>
        /// <returns>Enum SupportedCurrencies</returns>
        private SupportedCurrencies GetValidCurrencyFromISO(string ISOCode)
        {
            SupportedCurrencies parsedCurrency;

            if (!Enum.TryParse(ISOCode, true, out parsedCurrency))
            {
                _logger.LogInformation($"User provided invalid ISO Code: {ISOCode}");
                throw new InvalidCurrencyException(ISOCode);
            }

            return parsedCurrency;
        }
    }
}
