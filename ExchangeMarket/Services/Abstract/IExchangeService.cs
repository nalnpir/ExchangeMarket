using ExchangeMarket.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExchangeMarket.Services.Abstract
{
    public interface IExchangeService
    {
        public Task<decimal> GetExchangeRateByISOCodeAsync(string ISOCode);
        public Task BuyCurrency(BuyCurrencyOrder model);
    }
}
