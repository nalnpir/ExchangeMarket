using ExchangeMarket.Infrastructure.Repositories.Abstract;
using ExchangeMarket.Model;
using ExchangeMarket.Model.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeMarket.UnitTests.Mocks
{
    public class MockedExchangeRepository : IExchangeRepository
    {
        public Dictionary<string, List<Transaction>> TransactionsUser = new Dictionary<string, List<Transaction>>();
        public decimal USDLimit { get; private set; }
        public decimal BRLLimit { get; private set; }
        public decimal USDRate { get; private set; }
        public decimal BRLRate { get { return USDRate / 4; } }

        public MockedExchangeRepository(
            decimal USDLimit = 200, 
            decimal BRLLimit = 300, 
            decimal USDRate = 100
            )
        {
            this.USDLimit = USDLimit;
            this.BRLLimit = BRLLimit;
            this.USDRate = USDRate;
        }
        public Task<decimal> GetCurrentSpentAmountByUser(string userId, string currency)
        {
            //we dont need to validate the currency in the insert since its already validated
            
            List<Transaction> aux;
            if (TransactionsUser.TryGetValue(userId, out aux))
            {
                return Task.FromResult(aux.Where(x => x.Date.Year == DateTime.Today.Year 
                                                && x.Date.Month == DateTime.Today.Month 
                                                && x.Currency == currency.ToUpper()).Sum(x => x.Amount));
            }

            return Task.FromResult(0m);
        }

        public decimal GetExchangeLimit(SupportedCurrencies parsedCurrency)
        {
            return parsedCurrency switch
            {
                SupportedCurrencies.USD => USDLimit,
                SupportedCurrencies.BRL => BRLLimit,
                _ => throw new InvalidCurrencyException("mocked"),
            };
        }

        public Task<decimal> GetExchangeRateAsync(SupportedCurrencies parsedCurrency)
        {
            return parsedCurrency switch
            {
                SupportedCurrencies.USD => Task.FromResult(USDRate),
                SupportedCurrencies.BRL => Task.FromResult(BRLRate),
                _ => throw new InvalidCurrencyException("mocked"),
            };
        }

        public Task InsertTransaction(string userId, string currency, decimal amount)
        {
            //we dont need to validate the currency in the insert since its already validated
            Transaction transaction = new Transaction()
            {
                Id = Guid.NewGuid().ToString(),
                Amount = amount,
                Currency = currency.ToUpper(),
                Date = DateTime.Today,
                UserId = userId
            };

            InsertTransactionToDic(TransactionsUser, userId, transaction);

            return Task.CompletedTask;
        }

        private void InsertTransactionToDic(Dictionary<string, List<Transaction>> transactionsByUser, string userId, Transaction transaction)
        {
            if (transactionsByUser.ContainsKey(userId))
            {
                transactionsByUser[userId].Add(transaction);
            }
            else
            {
                transactionsByUser.Add(userId, new List<Transaction>() { transaction });
            }
        }
    }
}
