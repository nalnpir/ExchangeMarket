using Dapper;
using ExchangeMarket.Infrastructure.Helper;
using ExchangeMarket.Infrastructure.Helper.Currencies;
using ExchangeMarket.Infrastructure.Repositories.Abstract;
using ExchangeMarket.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Net.Http;
using System.Threading.Tasks;

namespace ExchangeMarket.Infrastructure.Repositories
{
    public class ExchangeRepository : IExchangeRepository
    {
        private readonly ExchangeSettings _settings;
        private readonly ILogger<ExchangeRepository> _logger;
        private readonly HttpClient _httpClient;
        public ExchangeRepository(IOptions<ExchangeSettings> settings, ILogger<ExchangeRepository> logger, HttpClient httpClient)
        {
            _settings = settings.Value ?? throw new ArgumentException(nameof(settings));
            _logger = logger ?? throw new ArgumentException(nameof(logger));
            _httpClient = httpClient ?? throw new ArgumentException(nameof(httpClient));
        }

        /// <summary>
        /// Inserts a transaction into the database
        /// </summary>
        /// <param name="UserId">User that made the transaction</param>
        /// <param name="Currency">ISO Formatted Currency</param>
        /// <param name="Amount">Amount already converted to the currency from argentian pesos</param>
        public async Task InsertTransaction(string UserId, string Currency, decimal Amount)
        {
            using (var conn = new SqlConnection(_settings.ConnectionString))
            {
                try
                {
                    conn.Open();
                    await conn.QueryAsync<int>(
                        @"INSERT INTO [dbo].[Transactions] (Id, UserId, Currency, Amount, Date) values(NEWID(), @UserId, @Currency, @Amount, GETDATE())",
                        new { UserId, Currency, Amount });
                }
                catch (SqlException exception)
                {
                    _logger.LogCritical(exception, "Database connections could not be opened: {Message}", exception.Message);
                    throw;
                }
            }
        }

        /// <summary>
        /// Get the amount purchased of a currency this month
        /// </summary>
        /// <param name="userId">User to see his current purchase</param>
        /// <param name="currency">ISO Formatted Currency to check upon</param>
        /// <returns>The amount that was spent int the month by an user expressed as decimal</returns>
        public async Task<decimal> GetCurrentSpentAmountByUser(string userId, string currency)
        {
            decimal result;
            using (var conn = new SqlConnection(_settings.ConnectionString))
            {
                try
                {
                    conn.Open();
                    var p = new DynamicParameters();
                    p.Add("@userId", userId, dbType: DbType.String, direction: ParameterDirection.Input);
                    p.Add("@currency", currency, dbType: DbType.String, direction: ParameterDirection.Input);
                    p.Add("@out", dbType: DbType.Decimal, direction: ParameterDirection.Output, precision: 13, scale: 4);
                    await conn.ExecuteAsync("[dbo].[Get_Monthly_Spent_Amount_By_User]", p, commandType: CommandType.StoredProcedure);
                    result = p.Get<decimal>("@out");
                }
                catch (SqlException exception)
                {
                    _logger.LogCritical(exception, "Database connections could not be opened: {Message}", exception.Message);
                    throw;
                }
            }

            return result;
        }

        public decimal GetExchangeLimit(SupportedCurrencies parsedCurrency) => GetExchangeHelperByCurrency(parsedCurrency).GetExchangeLimit();

        public Task<decimal> GetExchangeRateAsync(SupportedCurrencies parsedCurrency) => GetExchangeHelperByCurrency(parsedCurrency).GetExchangeRateAsync();

        /// <summary>
        /// Provided a normalized currency, it returns instance of ExchangeHelper in order to
        /// get things such as Current Rate or Currency Purchase Limit
        /// </summary>
        /// <param name="parsedCurrency">Enum SupportedCurrencies</param>
        /// <returns>An instance of one strategy of currencies</returns>
        private ExchangeHelper GetExchangeHelperByCurrency(SupportedCurrencies parsedCurrency)
        {
            ExchangeHelper exchangeRate;

            switch (parsedCurrency)
            {
                case SupportedCurrencies.USD:
                    exchangeRate = new ExchangeHelper(new UsdCurrency(_httpClient, _logger));
                    break;
                case SupportedCurrencies.BRL:
                    exchangeRate = new ExchangeHelper(new BrlCurrency(_httpClient, _logger));
                    break;
                default:
                    _logger.LogCritical("There is an invalid currency in the supported currencies");
                    throw new NotImplementedException("Unexpected error: check the GetValidCurrencyFromISO method, there is a not supported currency");
            }

            return exchangeRate;
        }
    }
}
